using System.Collections;
using UnityEngine;

public class PushToMoveButton : MonoBehaviour
{
    [SerializeField] private NodeMover nodeMover;
    [SerializeField] private bool reverseDirection;

    public void OnPressed()
    {
        Vector3 direction = reverseDirection ? -transform.forward : transform.forward;
        float minAngle = 60f;
        Node target = null;
        foreach (Node neighbor in nodeMover.currentNode.neighbors)
        {
            Vector3 toNeighbor = neighbor.transform.position - nodeMover.currentNode.transform.position;
            toNeighbor = Vector3.ProjectOnPlane(toNeighbor, Vector3.up).normalized;
            float angle = Vector3.Angle(direction, toNeighbor);
            if (angle < minAngle)
            {
                minAngle = angle;
                target = neighbor;
            }
        }
        if (target != null)
        {
            nodeMover.MoveTowardTo(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 direction = reverseDirection ? -transform.forward : transform.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction * 5f);
    }
}