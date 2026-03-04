using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;
using System.Collections;

// F**k Meta

public class PathAlignTransformer : MonoBehaviour, ITransformer
{
    [SerializeField] private bool releaseToNearestPoint;
    [SerializeField] private float alignSpeed;
    [SerializeField] private List<AlignPoint> alignPoints;
    [SerializeField, Optional, Tooltip("Another transform will replace the grabbing object for rotation and scaling.")]
    private Transform alternateTransform;

    private IGrabbable _grabbable;
    private Transform _grabbableTransform;
    private Coroutine _alignCoroutine;
    private Vector3 _grabOffset;

    private static readonly float _alignmentThreshold = 0.002f;

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
        _grabbableTransform = grabbable.Transform;

        if (alignPoints == null || alignPoints.Count < 2)
            Debug.LogWarning("PathAlignTransformer requires at least 2 align points.");
    }

    public void BeginTransform()
    {
        if (_alignCoroutine != null)
        {
            StopCoroutine(_alignCoroutine);
            _alignCoroutine = null;
        }
        _grabOffset = _grabbableTransform.position - _grabbable.GrabPoints[0].position;
        FindClosestAlignPoint().onExit?.Invoke();
    }

    public void UpdateTransform()
    {
        if (alignPoints == null || alignPoints.Count < 2)
            return;

        Vector3 desiredPosition = _grabbable.GrabPoints[0].position + _grabOffset;

        // 找到最近的线段以及在此线段上的插值 t
        int segmentIndex = FindClosestSegment(desiredPosition, out float t);

        Transform a = alignPoints[segmentIndex].transform;
        Transform b = alignPoints[segmentIndex + 1].transform;

        // 位置插值（约束在路径上）
        Vector3 constrainedPos = Vector3.Lerp(a.position, b.position, t);

        // Rotation & Scale 插值
        Quaternion constrainedRot = Quaternion.Slerp(a.rotation, b.rotation, t);
        Vector3 constrainedScale = Vector3.Lerp(a.localScale, b.localScale, t);

        // 应用到真实物体
        _grabbableTransform.position = constrainedPos;
        if (alternateTransform != null)
        {
            alternateTransform.rotation = constrainedRot;
            alternateTransform.localScale = constrainedScale;
        }
        else
        {
            _grabbableTransform.rotation = constrainedRot;
            _grabbableTransform.localScale = constrainedScale;
        }
    }

    public void EndTransform()
    {
        var closestPoint = FindClosestAlignPoint();
        if (releaseToNearestPoint)
        {
            _alignCoroutine = StartCoroutine(AlignTo(closestPoint));
        }
        closestPoint.onAlign?.Invoke();
    }

    private int FindClosestSegment(Vector3 point, out float bestT)
    {
        float minDistance = float.MaxValue;
        int bestIndex = 0;
        bestT = 0f;

        for (int i = 0; i < alignPoints.Count - 1; i++)
        {
            Vector3 a = alignPoints[i].transform.position;
            Vector3 b = alignPoints[i + 1].transform.position;

            float t = ProjectionFactor(point, a, b);
            Vector3 projectedPoint = Vector3.Lerp(a, b, t);

            float dist = Vector3.Distance(point, projectedPoint);

            if (dist < minDistance)
            {
                minDistance = dist;
                bestIndex = i;
                bestT = t;
            }
        }

        return bestIndex;
    }

    private float ProjectionFactor(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float lengthSq = ab.sqrMagnitude;
        if (lengthSq < 1e-6f) return 0f;

        float t = Vector3.Dot(point - a, ab) / lengthSq;
        return Mathf.Clamp01(t);
    }

    private AlignPoint FindClosestAlignPoint()
    {
        AlignPoint closestPoint = null;
        float closestDistance = float.MaxValue;

        foreach (var point in alignPoints)
        {
            float distance = Vector3.Distance(_grabbableTransform.position, point.transform.position);
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
        for (int i = 0; i < alignPoints.Count - 1; i++)
        {
            if (alignPoints[i] == null || alignPoints[i + 1] == null) continue;
            Gizmos.DrawLine(alignPoints[i].transform.position, alignPoints[i + 1].transform.position);
            Gizmos.DrawSphere(alignPoints[i].transform.position, 0.01f);
        }
    }

    private IEnumerator AlignTo(AlignPoint target)
    {
        Vector3 positionVelocity = Vector3.zero;
        Vector3 scaleVelocity = Vector3.zero;

        var modifyTransform = alternateTransform != null ? alternateTransform : _grabbableTransform;

        while (Vector3.Distance(_grabbableTransform.position, target.transform.position) > _alignmentThreshold)
        {
            _grabbableTransform.position = Vector3.SmoothDamp(
                _grabbableTransform.position,
                target.transform.position,
                ref positionVelocity,
                1f / alignSpeed);

            modifyTransform.localScale = Vector3.SmoothDamp(
                modifyTransform.localScale,
                target.transform.localScale,
                ref scaleVelocity,
                1f / alignSpeed);

            modifyTransform.rotation = Quaternion.RotateTowards(
                modifyTransform.rotation,
                target.transform.rotation,
                alignSpeed * 90f * Time.deltaTime);

            yield return null;
        }

        _grabbableTransform.SetPositionAndRotation(target.transform.position, alternateTransform == null ? target.transform.rotation : _grabbableTransform.rotation);
        modifyTransform.localScale = target.transform.localScale;
    }
}
