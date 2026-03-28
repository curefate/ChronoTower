using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VineCrash : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private AudioClip crashSound;
    [SerializeField] private AudioSource audioSource;

    private const float shake_distance = .005f;

    private Coroutine currentCoroutine;
    private Vector3 originalPosition;

    public void Crash()
    {
        originalPosition = transform.position;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 2f)
        {
            var sin = Mathf.Sin(elapsedTime * 20f);
            transform.position = originalPosition + shake_distance * sin * direction.normalized;
            if (Math.Abs(sin - 1) < 0.1f)
            {
                audioSource.PlayOneShot(crashSound);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
