using UnityEngine;
using UnityEngine.Events;

public class WristDetector : MonoBehaviour
{
    [SerializeField] private Transform wristTransform;
    [SerializeField] private float angleThreshold;
    [SerializeField] private UnityEvent onWristUp;
    [SerializeField] private UnityEvent onWristDown;

    private enum WristState { Up, Down }
    private WristState _currentState = WristState.Down;
    private Transform _camera;

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (wristTransform == null) return;

        if (Vector3.Angle(wristTransform.up, _camera.forward) < angleThreshold)
        {
            if (_currentState == WristState.Down)
            {
                _currentState = WristState.Up;
                onWristUp.Invoke();
            }
        }
        else
        {
            if (_currentState == WristState.Up)
            {
                _currentState = WristState.Down;
                onWristDown.Invoke();
            }
        }
    }
}
