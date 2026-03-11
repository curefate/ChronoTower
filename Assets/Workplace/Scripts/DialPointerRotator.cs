using Oculus.Interaction;
using UnityEngine;

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

    private bool isDragging = false;            // 是否正在拖动
    private float startAngle;                   // 初始角度
    private Quaternion startDialRotation;       // 表针开始拖动时的旋转

    /// <summary>
    /// PointerEventWrapper 的 WhenSelect / WhenUnselect 绑定到这两个方法
    /// </summary>
    public void OnPointerDown(PointerEvent evt)
    {
        isDragging = true;

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

        // 应用旋转方向系数
        float rotationDelta = delta * rotationDirection;

        // 基于初始旋转应用增量旋转（使用四元数避免欧拉角问题）
        Quaternion deltaRotation = Quaternion.AngleAxis(rotationDelta, rotationAxis);
        dialNeedle.localRotation = startDialRotation * deltaRotation;
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