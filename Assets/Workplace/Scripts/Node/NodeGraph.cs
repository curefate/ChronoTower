using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    private List<Node> allNodes;

    void Start()
    {
        if (allNodes == null)
        {
            allNodes = new List<Node>();
        }
    }

    public bool IsNodeInGraph(Node node)
    {
        return allNodes.Contains(node);
    }

    public void AddNode(Node node)
    {
        if (node == null)
        {
            Debug.LogWarning("Cannot add a null node to the graph.");
            return;
        }

        if (!allNodes.Contains(node))
        {
            allNodes.Add(node);
        }
    }

    public void ConnectNode(Node from, Node to, bool bidirectional = false)
    {
        if (!IsNodeInGraph(from) || !IsNodeInGraph(to))
        {
            Debug.LogWarning("One or both nodes are not part of the graph.");
            return;
        }

        from.neighbors ??= new List<Node>();
        if (!from.neighbors.Contains(to))
        {
            from.neighbors.Add(to);
        }

        if (!bidirectional) return;

        to.neighbors ??= new List<Node>();
        if (!to.neighbors.Contains(from))
        {
            to.neighbors.Add(from);
        }
    }

    public void DisconnectNode(Node from, Node to, bool bidirectional = false)
    {
        if (!IsNodeInGraph(from) || !IsNodeInGraph(to))
        {
            Debug.LogWarning("One or both nodes are not part of the graph.");
            return;
        }

        from.neighbors?.Remove(to);

        if (!bidirectional) return;

        to.neighbors?.Remove(from);
    }

    public bool FindPath(Node start, Node end, out List<Node> path)
    {
        if (start == null || !allNodes.Contains(start) || end == null || !allNodes.Contains(end))
        {
            path = null;
            return false;
        }

        var queue = new Queue<Node>();
        var visited = new HashSet<Node>();
        var parent = new Dictionary<Node, Node>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == end)
            {
                path = BuildPath(parent, start, end);
                return true;
            }

            foreach (var neighbor in current.neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    parent[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        path = null;
        return false;
    }


    private List<Node> BuildPath(Dictionary<Node, Node> parent, Node start, Node goal)
    {
        var path = new List<Node>();
        Node current = goal;

        while (current != start)
        {
            path.Add(current);
            current = parent[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

}
