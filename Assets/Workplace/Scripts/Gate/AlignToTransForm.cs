using System.Collections;
using UnityEngine;

public class AlignToTransForm : MonoBehaviour
{
    public Transform targetTransform;

    private Coroutine alignCoroutine;

    public void AlignTo(float duration)
    {
        if (alignCoroutine != null)
        {
            StopCoroutine(alignCoroutine);
        }
        alignCoroutine = StartCoroutine(Align(duration));
    }

    public void AlignBySpeed(float speed)
    {
        if (alignCoroutine != null)
        {
            StopCoroutine(alignCoroutine);
        }
        alignCoroutine = StartCoroutine(Align_Speed(speed));
    }

    private IEnumerator Align(float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = targetTransform.position;
        Quaternion endRot = targetTransform.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置和旋转完全对齐
        transform.position = endPos;
        transform.rotation = endRot;
    }

    private IEnumerator Align_Speed(float speed)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = targetTransform.position;
        Quaternion endRot = targetTransform.rotation;

        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / speed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置和旋转完全对齐
        transform.position = endPos;
        transform.rotation = endRot;
    }
}