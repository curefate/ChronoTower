using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 在物体“开始移动”与“停止移动”时触发事件的组件。
/// 可通过 Transform 位移差或 Rigidbody 速度判断移动状态。
/// </summary>
[AddComponentMenu("Custom/Movement Events")]
public class MovementEvents : MonoBehaviour
{
    public enum DetectMode
    {
        TransformDelta,  // 通过上一帧位置与本帧位置的差值判断
        RigidbodyVelocity // 通过刚体速度判断（优先使用 Rigidbody/Rigidbody2D）
    }

    [Header("检测方式")]
    public DetectMode detectMode = DetectMode.TransformDelta;

    [Tooltip("若为 true，仅在 XZ 平面上判断移动（忽略 Y 轴变化）")]
    public bool horizontalOnly = false;

    [Header("阈值/延迟")]
    [Tooltip("低于该速度视为静止，单位：米/秒")]
    public float speedThreshold = 0.02f;

    [Tooltip("判定静止前需要持续低于阈值的时间（秒），用于抖动消除")]
    public float stopDelay = 0.1f;

    [Tooltip("判定开始移动前需要持续高于阈值的时间（秒），用于抖动消除")]
    public float startDelay = 0.05f;

    [Header("事件")]
    public UnityEvent onMoveStarted;
    public UnityEvent onMoveStopped;

    // 内部状态
    private bool _isMoving = false;
    private Vector3 _lastPos;
    private float _aboveThresholdTimer = 0f;
    private float _belowThresholdTimer = 0f;

    // 刚体（可选）
    private Rigidbody _rb3D;
    private Rigidbody2D _rb2D;

    private void Awake()
    {
        _rb3D = GetComponent<Rigidbody>();
        _rb2D = GetComponent<Rigidbody2D>();
        _lastPos = transform.position;
    }

    private void Update()
    {
        float speed = 0f;

        if (detectMode == DetectMode.RigidbodyVelocity && (_rb3D != null || _rb2D != null))
        {
            // 使用刚体速度
            if (_rb3D != null)
            {
                Vector3 v = _rb3D.linearVelocity;
                if (horizontalOnly) v = new Vector3(v.x, 0f, v.z);
                speed = v.magnitude;
            }
            else // _rb2D != null
            {
                Vector2 v = _rb2D.linearVelocity;
                // 2D 场景仅 X、Y，若 horizontalOnly=true 则忽略 Y（按需）
                if (horizontalOnly) v = new Vector2(v.x, 0f);
                speed = v.magnitude;
            }
        }
        else
        {
            // 使用 Transform 位移
            Vector3 current = transform.position;
            Vector3 delta = current - _lastPos;
            if (horizontalOnly) delta = new Vector3(delta.x, 0f, delta.z);
            // 由位移近似速度：位移 / deltaTime
            speed = delta.magnitude / Mathf.Max(Time.deltaTime, 1e-6f);
            _lastPos = current;
        }

        // 定时器更新（消抖）
        if (speed > speedThreshold)
        {
            _aboveThresholdTimer += Time.deltaTime;
            _belowThresholdTimer = 0f;
        }
        else
        {
            _belowThresholdTimer += Time.deltaTime;
            _aboveThresholdTimer = 0f;
        }

        // 状态机切换
        if (!_isMoving && _aboveThresholdTimer >= startDelay)
        {
            _isMoving = true;
            // 立即触发“开始移动”
            onMoveStarted?.Invoke();
        }
        else if (_isMoving && _belowThresholdTimer >= stopDelay)
        {
            _isMoving = false;
            // 立即触发“停止移动”
            onMoveStopped?.Invoke();
        }
    }

#if UNITY_EDITOR
    // 在编辑器中提供一个简单的可视化调试
    private void OnDrawGizmosSelected()
    {
        var col = _isMoving ? Color.green : Color.red;
        Gizmos.color = col;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.3f,
            _isMoving ? "Moving" : "Stopped");
    }
#endif
}