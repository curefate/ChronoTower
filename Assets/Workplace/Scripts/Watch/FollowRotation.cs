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
    [Tooltip("旋转轴方向（在局部空间中）")]
    private Vector3 rotationAxis = Vector3.up;  // 默认为 Y 轴 (0, 1, 0)
    
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

        // 计算目标相对于初始状态的旋转变化
        Quaternion targetDelta = targetTransform.localRotation * Quaternion.Inverse(initialTargetRotation);
        
        // 提取旋转角度和轴向
        float angle;
        Vector3 axis;
        targetDelta.ToAngleAxis(out angle, out axis);
        
        // 归一化角度到 -180 到 180 范围
        if (angle > 180f)
            angle -= 360f;
        
        // 应用旋转比例和方向
        float adjustedAngle = angle * rotationRatio * rotationDirection;
        
        // 基于初始旋转应用调整后的旋转
        Quaternion adjustedRotation = Quaternion.AngleAxis(adjustedAngle, rotationAxis);
        transform.localRotation = initialSelfRotation * adjustedRotation;
    }

    /// <summary>
    /// 重置到初始状态（可在运行时调用）
    /// </summary>
    public void ResetToInitial()
    {
        if (initialized)
        {
            transform.localRotation = initialSelfRotation;
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

    // 编辑器中实时预览（仅在编辑模式下）
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // 确保旋转轴被归一化
        if (rotationAxis != Vector3.zero)
        {
            rotationAxis = rotationAxis.normalized;
        }
    }
    #endif
}
