using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayToDo : MonoBehaviour
{
    public UnityEvent onDelayComplete;

    private Coroutine currentCoroutine;

    public void StartDelay(float delayDuration)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DelayCoroutine(delayDuration));
    }

    private IEnumerator DelayCoroutine(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        onDelayComplete.Invoke();
    }
}
