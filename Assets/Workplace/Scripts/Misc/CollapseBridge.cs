using System;
using System.Collections;
using UnityEngine;

public class CollapseBridge : MonoBehaviour
{
    [SerializeField] private Vector3 collapseRotation;
    private Quaternion _initialRotation;
    private Coroutine _collapseCoroutine;

    private void Start()
    {
        _initialRotation = transform.localRotation;
    }

    public void Collapse()
    {
        if (_collapseCoroutine != null)
        {
            StopCoroutine(_collapseCoroutine);
        }
        _collapseCoroutine = StartCoroutine(Collapse(Quaternion.Euler(collapseRotation) * _initialRotation, .8f));
    }

    public void Back()
    {
        if (_collapseCoroutine != null)
        {
            StopCoroutine(_collapseCoroutine);
        }
        _collapseCoroutine = StartCoroutine(Collapse(_initialRotation, .8f));
    }

    private IEnumerator Collapse(Quaternion targetRotation, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(_initialRotation, targetRotation, elapsed / duration);
            yield return null;
        }
        transform.localRotation = targetRotation;
    }
}
