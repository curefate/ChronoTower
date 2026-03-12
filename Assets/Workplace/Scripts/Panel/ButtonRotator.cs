using System.Collections;
using UnityEngine;

public class ButtonRotator : MonoBehaviour
{
    private const float Degree = 10;
    private const float Speed = 100f;

    private float _originalRotationZ;
    private Coroutine _rotationCoroutine;

    private void Start()
    {
        _originalRotationZ = transform.localEulerAngles.z;
    }

    public void Rotate()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }
        _rotationCoroutine = StartCoroutine(RotateMe(_originalRotationZ + Degree));
    }

    public void ResetRotation()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }
        _rotationCoroutine = StartCoroutine(RotateMe(_originalRotationZ));
    }

    private IEnumerator RotateMe(float targetDegree)
    {
        Debug.Log($"Rotating to {targetDegree} degrees");
        var currentRotationZ = transform.localEulerAngles.z;
        while (Mathf.Abs(currentRotationZ - targetDegree) > 0.01f)
        {
            currentRotationZ = Mathf.MoveTowards(currentRotationZ, targetDegree, Speed * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentRotationZ);
            yield return null;
        }
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, targetDegree);
    }
}
