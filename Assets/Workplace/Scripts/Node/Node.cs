using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public List<Node> neighbors;

    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

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
        if (neighbors == null || neighbors.Count == 0)
        {
            Debug.LogWarning($"Node '{name}' has no neighbors assigned.");
        }
        foreach (Node neighbor in neighbors)
        {
            if (neighbor == null)
            {
                Debug.LogWarning($"Node '{name}' has a null neighbor reference.");
            }
        }

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
        else
        {
            Gizmos.color = Color.green;
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
