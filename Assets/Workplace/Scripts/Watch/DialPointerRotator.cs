using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialPointerRotator : MonoBehaviour
{
    [Header("Dial Settings")]
    public Transform dialNeedle;                // 你的表针 transform
    public Transform dialCenter;                // 表盘中心点 (表针旋转中心)

    [Header("Rotation Settings")]
    [SerializeField]
    [Tooltip("旋转轴方向 (在 dialNeedle 的局部空间中)")]
    private Vector3 rotationAxis = Vector3.up;  // 默认为 Y 轴 (0, 1, 0)

    [SerializeField]
    [Tooltip("旋转方向系数 (1 = 正向, -1 = 反向)")]
    private float rotationDirection = -1f;       // 旋转方向，可以是 1 或 -1

    /* [Header("Auto Complete Settings")]
    [SerializeField]
    [Tooltip("是否启用自动补全功能")] */
    private bool enableAutoComplete = true;

    [SerializeField]
    [Tooltip("触发自动补全的最小旋转角度")]
    private float autoCompleteThreshold = 60f;

    [SerializeField]
    [Tooltip("自动补全的目标角度（度）")]
    private float autoCompleteTargetAngle = 360f;

    [SerializeField]
    [Tooltip("自动补全旋转的速度（度/秒）")]
    private float autoCompleteSpeed = 180f;

    /* [Header("Spring Back Settings")]
    [SerializeField]
    [Tooltip("是否启用回弹功能（未达到阈值时回到起始位置）")] */
    private bool enableSpringBack = false;

    [SerializeField]
    [Tooltip("触发回弹的最小旋转角度（小于此值不会回弹）")]
    private float springBackMinRotation = 5f;

    [SerializeField]
    [Tooltip("回弹速度（度/秒）")]
    private float springBackSpeed = 360f;

    [Header("Events")]
    [SerializeField]
    [Tooltip("顺时针自动补全时触发")]
    private UnityEvent OnClockwise;

    [SerializeField]
    [Tooltip("逆时针自动补全时触发")]
    private UnityEvent OnCounterClockwise;

    private bool isDragging = false;            // 是否正在拖动
    private float startAngle;                   // 初始角度
    private Quaternion startDialRotation;       // 表针开始拖动时的旋转
    private float totalRotation = 0f;           // 追踪本次拖动的累积旋转角度
    private Coroutine autoRotationCoroutine;    // 自动旋转协程引用

    /// <summary>
    /// PointerEventWrapper 的 WhenSelect / WhenUnselect 绑定到这两个方法
    /// </summary>
    public void OnPointerDown(PointerEvent evt)
    {
        // 如果正在自动旋转，停止它
        if (autoRotationCoroutine != null)
        {
            StopCoroutine(autoRotationCoroutine);
            autoRotationCoroutine = null;
        }

        isDragging = true;
        totalRotation = 0f;  // 重置累积旋转

        // 手指在世界中的位置
        Vector3 fingerPos = evt.Pose.position;

        // 转换到表盘平面的角度
        startAngle = GetAngleOnDial(fingerPos);

        // 保存表针当前的旋转（使用四元数避免欧拉角问题）
        startDialRotation = dialNeedle.localRotation;
    }

    public void OnPointerUp(PointerEvent evt)
    {
        isDragging = false;

        // 将旋转角度归一化到 -autoCompleteTargetAngle 到 autoCompleteTargetAngle 范围内（取模）
        float normalizedRotation = totalRotation;
        if (Mathf.Abs(normalizedRotation) > autoCompleteTargetAngle)
        {
            normalizedRotation = normalizedRotation % autoCompleteTargetAngle;
        }

        float absRotation = Mathf.Abs(normalizedRotation);

        // 判断是否触发自动补全或回弹
        if (enableAutoComplete && absRotation >= autoCompleteThreshold && absRotation < autoCompleteTargetAngle)
        {
            // 自动补全到目标角度
            float remainingRotation = (autoCompleteTargetAngle - absRotation) * Mathf.Sign(normalizedRotation);
            autoRotationCoroutine = StartCoroutine(AutoRotateToTarget(remainingRotation, autoCompleteSpeed));

            // 根据旋转方向触发对应的事件
            // 注意：需要考虑 rotationDirection 参数来判断实际的旋转方向
            float actualRotationDirection = normalizedRotation * rotationDirection;
            if (actualRotationDirection > 0)
            {
                // 顺时针旋转
                OnClockwise?.Invoke();
            }
            else if (actualRotationDirection < 0)
            {
                // 逆时针旋转
                OnCounterClockwise?.Invoke();
            }
        }
        else if (enableSpringBack && absRotation > springBackMinRotation && absRotation < autoCompleteThreshold)
        {
            // 回弹到起始位置
            autoRotationCoroutine = StartCoroutine(AutoRotateToTarget(-normalizedRotation, springBackSpeed));
        }
    }

    /// <summary>
    /// PointerEventWrapper 的 WhenMove 绑定到这个方法
    /// </summary>
    public void OnPointerMove(PointerEvent evt)
    {
        if (!isDragging) return;

        Vector3 fingerPos = evt.Pose.position;

        float currentAngle = GetAngleOnDial(fingerPos);
        float delta = Mathf.DeltaAngle(startAngle, currentAngle);

        // 累积旋转角度（用于判断是否触发自动补全或回弹）
        totalRotation = delta;

        // 应用旋转方向系数
        float rotationDelta = delta * rotationDirection;

        // 基于初始旋转应用增量旋转（使用四元数避免欧拉角问题）
        Quaternion deltaRotation = Quaternion.AngleAxis(rotationDelta, rotationAxis);
        dialNeedle.localRotation = startDialRotation * deltaRotation;
    }

    /// <summary>
    /// 自动旋转到目标角度的协程
    /// </summary>
    /// <param name="targetRotation">目标旋转角度（相对于当前位置）</param>
    /// <param name="speed">旋转速度（度/秒）</param>
    private IEnumerator AutoRotateToTarget(float targetRotation, float speed)
    {
        Quaternion startRot = dialNeedle.localRotation;
        float rotated = 0f;
        float absTarget = Mathf.Abs(targetRotation);
        float direction = Mathf.Sign(targetRotation);

        while (rotated < absTarget)
        {
            float step = speed * Time.deltaTime;
            rotated = Mathf.Min(rotated + step, absTarget);

            float currentRotation = rotated * direction * rotationDirection;
            Quaternion deltaRotation = Quaternion.AngleAxis(currentRotation, rotationAxis);
            dialNeedle.localRotation = startRot * deltaRotation;

            yield return null;
        }

        autoRotationCoroutine = null;
    }

    /// <summary>
    /// 从手指位置推断其在表盘平面上的角度
    /// </summary>
    float GetAngleOnDial(Vector3 fingerWorldPos)
    {
        Vector3 dir = fingerWorldPos - dialCenter.position;

        // 将方向转换到 dialCenter 的本地空间，使得旋转轴统一为 Y轴
        Vector3 localDir = dialCenter.InverseTransformDirection(dir);

        float angle = Mathf.Atan2(localDir.z, localDir.x) * Mathf.Rad2Deg;
        return angle;
    }
}