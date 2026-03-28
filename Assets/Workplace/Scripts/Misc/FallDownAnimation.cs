using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FallDownAnimation : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform[] bundles;
    [SerializeField] private int[] bundleBatchSizes;
    [SerializeField] private UnityEvent onFallComplete;
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioSource audioSource;

    private bool _isStarted = false;

    private const float _acceleration = 9.8f;
    private static WaitForSeconds _waitForSeconds0_1 = new(0.1f);

    public void Execute()
    {
        if (!_isStarted)
        {
            _isStarted = true;
            root.gameObject.SetActive(true);
            var camTrans = Camera.main.transform;
            root.transform.position = new Vector3(camTrans.position.x, 0, camTrans.position.z) + Vector3.ProjectOnPlane(camTrans.forward, Vector3.up).normalized * 2f;
            StartCoroutine(StartFallDown());
        }
    }

    private IEnumerator StartFallDown()
    {
        int index = 0;
        foreach (var size in bundleBatchSizes)
        {
            for (int i = 0; i < size; i++)
            {
                if (index >= bundles.Length) break;

                var bundle = bundles[index];
                StartCoroutine(FallDown(bundle));
                index++;
                yield return _waitForSeconds0_1; // Stagger the start of each bundle's fall
            }

            float delay = Random.Range(.5f, 1.5f);
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(2f);
        onFallComplete.Invoke();
    }

    private IEnumerator FallDown(Transform transform)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = Vector3.zero;

        float speed = 1f;

        while (Vector3.Distance(transform.position, endPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, speed);
            speed += _acceleration * Time.deltaTime; // Accelerate the fall
            yield return null;
        }
        transform.position = endPos;
        audioSource.PlayOneShot(fallSound);
    }
}
