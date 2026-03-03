using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrabbableAligner : MonoBehaviour
{
    public float AlignSpeed;
    [SerializeField] private List<AlignPoint> alignPoints;

    private static readonly float _alignmentThreshold = 0.005f;

    public void OnGrab()
    {
        StopAllCoroutines();
    }

    public void OnRelease()
    {
        StartCoroutine(AlignTo(GetClosestAlignPoint()));
    }

    private IEnumerator AlignTo(AlignPoint target)
    {
        var originalDistance = Vector3.Distance(transform.position, target.transform.position);
        var distance = originalDistance;
        while (distance > _alignmentThreshold)
        {
            var speed = AlignSpeed * Time.deltaTime * Mathf.Max(distance / originalDistance, 0.5f);
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, target.transform.position, speed), Quaternion.Slerp(transform.rotation, target.transform.rotation, speed));
            distance = Vector3.Distance(transform.position, target.transform.position);
            yield return null;
        }
    }

    private AlignPoint GetClosestAlignPoint()
    {
        AlignPoint closestPoint = null;
        float closestDistance = float.MaxValue;

        foreach (var point in alignPoints)
        {
            float distance = Vector3.Distance(transform.position, point.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    private void OnDrawGizmosSelected()
    {
        if (alignPoints == null || alignPoints.Count == 0) return;

        Gizmos.color = Color.rosyBrown;
        foreach (var point in alignPoints)
        {
            if (point == null) continue;
            Gizmos.DrawSphere(point.transform.position, 0.01f);
        }
    }
}
