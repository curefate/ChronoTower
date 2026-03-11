using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public List<Node> neighbors;

    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    private const float DistanceThreshold = 0.1f;

    public void ConnectTo(Node other)
    {
        if (other == null || other == this) return;

        neighbors ??= new List<Node>();
        if (!neighbors.Contains(other))
        {
            neighbors.Add(other);
        }
    }

    public void ConnectToBoth(Node other)
    {
        if (other == null || other == this) return;

        ConnectTo(other);
        other.ConnectTo(this);
    }

    public void ConnectToNear(Node other)
    {
        if (other == null || other == this) return;

        if (Vector3.Distance(transform.position, other.transform.position) <= DistanceThreshold &&
            Mathf.Abs(transform.position.y - other.transform.position.y) <= DistanceThreshold)
        {
            ConnectTo(other);
        }
    }

    public void ConnectToNearBoth(Node other)
    {
        if (other == null || other == this) return;

        ConnectToNear(other);
        other.ConnectToNear(this);
    }

    public void DisConnectTo(Node other)
    {
        if (other == null || other == this) return;

        neighbors?.Remove(other);
    }

    public void DisconnectToBoth(Node other)
    {
        if (other == null || other == this) return;

        DisConnectTo(other);
        other.DisConnectTo(this);
    }

    public virtual void Enter(NodeMover mover)
    {
        onEnter?.Invoke();
    }

    public virtual void Exit(NodeMover mover)
    {
        onExit?.Invoke();
    }

    void Start()
    {
        neighbors ??= new List<Node>();
        neighbors = neighbors.Distinct().Where(n => n != null && n != this).ToList();

        var nodeGraph = FindFirstObjectByType<NodeGraph>();
        if (nodeGraph == null)
        {
            Debug.LogWarning($"Node '{name}' could not find a NodeGraph.");
            return;
        }
        nodeGraph.AddNode(this);
    }

    void OnDrawGizmos()
    {
        List<Node> neighbors = this.neighbors.Distinct().Where(n => n != null).ToList();

        if (neighbors == null)
            return;

        if (neighbors.Count == 0)
        {
            Gizmos.color = Color.red;
        }
        else if (neighbors.Count == 1)
        {
            Gizmos.color = Color.yellow;
        }
        else if (neighbors.Count == 2)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.blueViolet;
        }
        Gizmos.DrawSphere(transform.position, 0.015f);

        foreach (Node neighbor in neighbors)
        {
            if (neighbor.neighbors.Contains(this))
            {
                Gizmos.color = Color.cyan;
            }
            else
            {
                Gizmos.color = Color.magenta;
                var arrowPos = (transform.position + neighbor.transform.position) / 2;
                arrowPos = (arrowPos + neighbor.transform.position) / 2;
                Gizmos.DrawSphere(arrowPos, 0.01f);
            }
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}
