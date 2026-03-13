using UnityEngine;

/// <summary>
/// 跟随目标Transform的旋转，支持自定义旋转比例和方向
/// 适用于表盘指针、装饰性齿轮等需要联动旋转的物体
/// </summary>
public class FollowRotation : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("要跟随的目标Transform")]
    public Transform targetTransform;
    
    [Header("Rotation Settings")]
    [SerializeField]
    [Tooltip("目标物体的旋转轴方向（在目标的局部空间中）")]
    private Vector3 targetRotationAxis = Vector3.up;  // 默认为 Y 轴 (0, 1, 0)
    
    [SerializeField]
    [Tooltip("自身的旋转轴方向（在自身的局部空间中）")]
    private Vector3 selfRotationAxis = Vector3.up;  // 默认为 Y 轴 (0, 1, 0)
    
    [SerializeField]
    [Tooltip("旋转比例（1 = 同步，0.5 = 半速，2 = 双速，60 = 秒针比分针快60倍）")]
    private float rotationRatio = 1f;
    
    [SerializeField]
    [Tooltip("旋转方向（1 = 同向，-1 = 反向）")]
    private float rotationDirection = 1f;
    
    [Header("Update Settings")]
    [SerializeField]
    [Tooltip("更新模式：何时计算跟随旋转")]
    private UpdateMode updateMode = UpdateMode.LateUpdate;
    
    [SerializeField]
    [Tooltip("是否在Start时同步到目标的初始旋转")]
    private bool syncInitialRotation = true;
    
    private enum UpdateMode 
    { 
        Update,         // 每帧更新
        LateUpdate,     // 在所有Update之后更新（推荐）
        FixedUpdate     // 固定时间间隔更新
    }
    
    private Quaternion initialSelfRotation;     // 自身的初始旋转
    private Quaternion initialTargetRotation;   // 目标的初始旋转
    private Quaternion previousTargetRotation;  // 上一帧的目标旋转
    private float accumulatedAngle = 0f;        // 累积的旋转角度（支持多圈）
    private bool initialized = false;           // 是否已初始化

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (updateMode == UpdateMode.Update)
        {
            UpdateRotation();
        }
    }

    void LateUpdate()
    {
        if (updateMode == UpdateMode.LateUpdate)
        {
            UpdateRotation();
        }
    }

    void FixedUpdate()
    {
        if (updateMode == UpdateMode.FixedUpdate)
        {
            UpdateRotation();
        }
    }

    /// <summary>
    /// 初始化：记录初始状态
    /// </summary>
    private void Initialize()
    {
        if (targetTransform == null)
        {
            Debug.LogWarning($"FollowRotation on {gameObject.name}: targetTransform is not assigned!", this);
            return;
        }

        initialTargetRotation = targetTransform.localRotation;
        initialSelfRotation = transform.localRotation;
        previousTargetRotation = targetTransform.localRotation;
        accumulatedAngle = 0f;
        initialized = true;

        // 如果需要同步初始旋转
        if (syncInitialRotation)
        {
            UpdateRotation();
        }
    }

    /// <summary>
    /// 更新跟随旋转
    /// </summary>
    private void UpdateRotation()
    {
        if (!initialized || targetTransform == null)
            return;

        // 计算目标从上一帧到当前帧的旋转变化（增量）
        Quaternion deltaRotation = targetTransform.localRotation * Quaternion.Inverse(previousTargetRotation);
        
        // 从目标的旋转中提取绕目标旋转轴的角度
        float deltaAngle = GetAngleAroundAxis(deltaRotation, targetRotationAxis);
        
        // 累积角度（支持超过360度的多圈旋转）
        accumulatedAngle += deltaAngle;
        
        // 应用旋转比例和方向
        float adjustedAngle = accumulatedAngle * rotationRatio * rotationDirection;
        
        // 基于初始旋转，绕自身旋转轴应用调整后的旋转（避免四元数累积误差）
        Quaternion adjustedRotation = Quaternion.AngleAxis(adjustedAngle, selfRotationAxis);
        transform.localRotation = initialSelfRotation * adjustedRotation;
        
        // 更新记录
        previousTargetRotation = targetTransform.localRotation;
    }

    /// <summary>
    /// 获取四元数绕指定轴的旋转角度（支持超过180度）
    /// </summary>
    /// <param name="rotation">要分析的旋转四元数</param>
    /// <param name="axis">旋转轴</param>
    /// <returns>绕指定轴的旋转角度（-180到180度）</returns>
    private float GetAngleAroundAxis(Quaternion rotation, Vector3 axis)
    {
        // 选择一个垂直于旋转轴的参考向量
        Vector3 forward = Vector3.forward;
        Vector3 perpendicular = Vector3.Cross(axis, forward);
        
        // 如果轴与forward平行，使用另一个向量
        if (perpendicular.sqrMagnitude < 0.001f)
        {
            perpendicular = Vector3.Cross(axis, Vector3.up);
        }
        perpendicular.Normalize();
        
        // 旋转这个垂直向量
        Vector3 rotatedPerpendicular = rotation * perpendicular;
        
        // 投影到垂直于轴的平面上
        Vector3 projectedOriginal = Vector3.ProjectOnPlane(perpendicular, axis).normalized;
        Vector3 projectedRotated = Vector3.ProjectOnPlane(rotatedPerpendicular, axis).normalized;
        
        // 计算有符号角度（使用SignedAngle可以正确处理超过180度的情况）
        float angle = Vector3.SignedAngle(projectedOriginal, projectedRotated, axis);
        
        return angle;
    }

    /// <summary>
    /// 重置到初始状态（可在运行时调用）
    /// </summary>
    public void ResetToInitial()
    {
        if (initialized)
        {
            accumulatedAngle = 0f;
            transform.localRotation = initialSelfRotation;
            previousTargetRotation = initialTargetRotation;
        }
    }

    /// <summary>
    /// 重新同步初始状态（可在运行时调用以重新设定基准）
    /// </summary>
    public void ResyncInitialState()
    {
        Initialize();
    }

    /// <summary>
    /// 设置新的跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
        Initialize();
    }

    /// <summary>
    /// 获取当前累积的旋转角度（可用于调试）
    /// </summary>
    public float GetAccumulatedAngle()
    {
        return accumulatedAngle;
    }

    // 编辑器中实时预览（仅在编辑模式下）
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // 确保旋转轴被归一化
        if (targetRotationAxis != Vector3.zero)
        {
            targetRotationAxis = targetRotationAxis.normalized;
        }
        
        if (selfRotationAxis != Vector3.zero)
        {
            selfRotationAxis = selfRotationAxis.normalized;
        }
    }
    #endif
}
