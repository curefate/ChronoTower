using Oculus.Interaction;
using UnityEngine;

public class DialPointerRotator : MonoBehaviour
{
    [Header("Dial Settings")]
    public Transform dialNeedle;                // 你的表针 transform
    public Vector3 rotationAxis = Vector3.forward; // 绕哪个轴旋转（例如表针绕Z轴旋转）
    public Transform dialCenter;                // 表盘中心点 (表针旋转中心)

    private bool isDragging = false;            // 是否正在拖动
    private float startAngle;                   // 初始角度
    private float baseDialAngle;                // 表针开始拖动时的角度偏移

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
        baseDialAngle = dialNeedle.localEulerAngles.z;
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

        float newAngle = baseDialAngle + delta;

        // 这里假设表针是绕 Z 轴旋转
        dialNeedle.localRotation = Quaternion.Euler(0, 0, newAngle);
    }

    /// <summary>
    /// 从手指位置推断其在表盘平面上的角度
    /// </summary>
    float GetAngleOnDial(Vector3 fingerWorldPos)
    {
        Vector3 dir = fingerWorldPos - dialCenter.position;

        // 将方向转换到 dialCenter 的本地空间，使得旋转轴统一为 Z
        Vector3 localDir = dialCenter.InverseTransformDirection(dir);

        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
        return angle;
    }
}