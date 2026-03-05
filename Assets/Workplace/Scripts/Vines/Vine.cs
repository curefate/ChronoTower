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
    [SerializeField] private UnityEvent onBlocked;
    [SerializeField] private bool ifCanGrow = true;
    public void SetCanGrow(bool value) => ifCanGrow = value;
    [SerializeField] private bool ifCanShrink = true;
    public void SetCanShrink(bool value) => ifCanShrink = value;

    private Coroutine _transformCoroutine;

    private void OnGrow()
    {
        if (!ifCanGrow)
        {
            onBlocked?.Invoke();
            return;
        }

        if (_transformCoroutine != null)
        {
            StopCoroutine(_transformCoroutine);
        }
        _transformCoroutine = StartCoroutine(ControlGrowth(true));
    }

    private void OnShrink()
    {
        if (!ifCanShrink)
        {
            onBlocked?.Invoke();
            return;
        }

        if (_transformCoroutine != null)
        {
            StopCoroutine(_transformCoroutine);
        }
        _transformCoroutine = StartCoroutine(ControlGrowth(false));
    }

    private IEnumerator ControlGrowth(bool isGrow)
    {
        var targetTransform = isGrow ? growTransform : shrinkTransform;
        var onComplete = isGrow ? onGrow : onShrink;

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
        transform.SetPositionAndRotation(targetPosition, targetRotation);
        transform.localScale = targetScale;

        onComplete?.Invoke();
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

    /*     private void OnEnable()
        {
            TimePublisher.Instance.RegisterListener(this);
        }

        private void OnDisable()
        {
            TimePublisher.Instance.UnregisterListener(this);
        } */

    private void OnDestroy()
    {
        TimePublisher.Instance.UnregisterListener(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (shrinkTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shrinkTransform.position, 0.01f);
        }
        if (growTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(growTransform.position, 0.01f);
        }
    }
}
