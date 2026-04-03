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

    private const float _speed_factor = 2f;
    private static WaitForSeconds _waitForSeconds2 = new(2f);
    private static WaitForSeconds _waitForSeconds0_1 = new(0.1f);

    public void Start()
    {
        root.gameObject.SetActive(false);
        foreach (var bundle in bundles)
        {
            bundle.localPosition = new Vector3(bundle.localPosition.x, 10f, bundle.localPosition.z);
        }
    }

    public void Execute()
    {
        if (!_isStarted)
        {
            _isStarted = true;
            root.gameObject.SetActive(true);
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

            float delay = Random.Range(.2f, .5f);
            yield return new WaitForSeconds(delay);
        }

        yield return _waitForSeconds2;
        onFallComplete.Invoke();
    }

    private IEnumerator FallDown(Transform transform)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = Vector3.zero;

        float speed = 0f;
        while (Vector3.Distance(transform.localPosition, endPos) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, speed);
            speed += Time.deltaTime * _speed_factor;
            yield return null;
        }
        transform.localPosition = endPos;
        audioSource.PlayOneShot(fallSound);
    }
}
