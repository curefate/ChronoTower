using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Vine : MonoBehaviour, ITimeListener
{
    [SerializeField] private float transformDuration = 1.5f;
    [SerializeField] private Transform shrinkTransform;
    [SerializeField] private Transform growTransform;
    [SerializeField] private UnityEvent onGrow;
    [SerializeField] private UnityEvent onShrink;

    private Coroutine _transformCoroutine;

    private void OnGrow()
    {
        if (_transformCoroutine != null)
        {
            StopCoroutine(_transformCoroutine);
        }
        _transformCoroutine = StartCoroutine(ApplyTransform(growTransform));
        onGrow?.Invoke();
    }

    private void OnShrink()
    {
        if (_transformCoroutine != null)
        {
            StopCoroutine(_transformCoroutine);
        }
        _transformCoroutine = StartCoroutine(ApplyTransform(shrinkTransform));
        onShrink?.Invoke();
    }

    private IEnumerator ApplyTransform(Transform targetTransform)
    {
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = targetTransform.position;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = targetTransform.localScale;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = targetTransform.rotation;

        float elapsedTime = 0f;
        while (elapsedTime < transformDuration)
        {
            float t = elapsedTime / transformDuration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            transform.SetPositionAndRotation(Vector3.Lerp(initialPosition, targetPosition, t), Quaternion.Slerp(initialRotation, targetRotation, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
        transform.rotation = targetRotation;
    }

    public void OnTimeChanged(TimeEventType eventType)
    {
        switch (eventType)
        {
            case TimeEventType.TimeProgressed:
                OnGrow();
                break;
            case TimeEventType.TimeReversed:
                OnShrink();
                break;
        }
    }

    private void Start()
    {
        TimePublisher.Instance.RegisterListener(this);
    }

    private void OnEnable()
    {
        TimePublisher.Instance.RegisterListener(this);
    }

    private void OnDisable()
    {
        TimePublisher.Instance.UnregisterListener(this);
    }

    private void OnDestroy()
    {
        TimePublisher.Instance.UnregisterListener(this);
    }
}
