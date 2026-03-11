using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassCodePanel : MonoBehaviour
{
    [SerializeField] private List<PassCodeButton> buttons;

    private int correctRequire;
    private int validPressTimes;

    [SerializeField] private UnityEvent OnPasscodeCorrect;
    [SerializeField] private UnityEvent OnPasscodeWrong;
    [SerializeField] private Transform hideTransform;
    [SerializeField] private Transform showTransform;
    private Coroutine _transitionCoroutine;
    private const float TransitionDuration = 1f;

    public void ShowPanel()
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }
        _transitionCoroutine = StartCoroutine(AlignToTransforms(showTransform, TransitionDuration));
    }

    public void HidePanel()
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }
        _transitionCoroutine = StartCoroutine(AlignToTransforms(hideTransform, TransitionDuration));
    }

    private void Start()
    {
        if (buttons == null || buttons.Count == 0)
        {
            Debug.LogError("PassCodePanel: No buttons assigned.");
            return;
        }

        correctRequire = buttons.FindAll(button => button.IsCorrect).Count;
        validPressTimes = correctRequire;

        foreach (var button in buttons)
        {
            button.SetPanel(this);
        }

        _transitionCoroutine = StartCoroutine(AlignToTransforms(hideTransform, 0.01f));
    }

    public void PressButton(bool isCorrect)
    {
        validPressTimes--;
        if (isCorrect)
        {
            correctRequire--;
        }

        if (correctRequire == 0)
        {
            OnPasscodeCorrect?.Invoke();
            ResetPanel();
            return;
        }

        if (validPressTimes == 0)
        {
            OnPasscodeWrong?.Invoke();
            ResetPanel();
            return;
        }
    }

    public void ReleaseButton(bool isCorrect)
    {
        validPressTimes++;
        if (isCorrect)
        {
            correctRequire++;
        }
    }

    private void ResetPanel()
    {
        correctRequire = buttons.FindAll(button => button.IsCorrect).Count;
        validPressTimes = correctRequire;

        foreach (var button in buttons)
        {
            button.Reset();
        }
    }

    private IEnumerator AlignToTransforms(Transform target, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;
        Vector3 initialScale = transform.localScale;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.SetPositionAndRotation(Vector3.Lerp(initialPosition, target.position, t), Quaternion.Slerp(initialRotation, target.rotation, t));
            transform.localScale = Vector3.Lerp(initialScale, target.localScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(target.position, target.rotation);
        transform.localScale = target.localScale;
    }
}
