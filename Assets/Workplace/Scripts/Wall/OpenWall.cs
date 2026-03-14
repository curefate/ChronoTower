using System.Collections;
using UnityEngine;

public class OpenWall : MonoBehaviour
{
    [SerializeField] private bool ifShouldOpen;
    public void SetIfShouldOpen(bool value)
    {
        ifShouldOpen = value;
        if (!value && _isOpen)
        {
            _isOpen = false;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(OpenOrCloseCoroutine(false));
        }
    }
    [SerializeField] private Transform centralPivot;

    private const float detectionAngle = 45f;
    private const float openSpeed = 18f;
    private const float openAngle = 90f;

    private Transform camPos;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private Vector3 forwardDirection;
    private Coroutine currentCoroutine;
    private bool _isOpen = false;

    private void Start()
    {
        camPos = Camera.main.transform;
        originalRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, openAngle, 0) * originalRotation;
        forwardDirection = transform.right;
    }

    private void Update()
    {
        if (!ifShouldOpen) return;

        Vector3 toCam = camPos.position - centralPivot.position;
        toCam = Vector3.ProjectOnPlane(toCam, Vector3.up).normalized;
        float angleToCam = Vector3.Angle(forwardDirection, toCam);
        if (angleToCam < detectionAngle && !_isOpen)
        {
            _isOpen = true;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(OpenOrCloseCoroutine(true));
        }
        else if (angleToCam >= detectionAngle && _isOpen)
        {
            _isOpen = false;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(OpenOrCloseCoroutine(false));
        }
    }

    private IEnumerator OpenOrCloseCoroutine(bool open)
    {
        var finalRotation = open ? targetRotation : originalRotation;
        while (Quaternion.Angle(transform.rotation, finalRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, openSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = finalRotation;
    }
}
