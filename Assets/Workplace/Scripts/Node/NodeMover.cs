using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMover : MonoBehaviour
{
    public float moveSpeed;
    public Node currentNode;
    public Vector3 offset;

    private NodeGraph nodeGraph;
    private bool _isMoving;
    private const int _moveTowardBatch = 2;

    private void Start()
    {
        nodeGraph = FindFirstObjectByType<NodeGraph>();
    }

    void Update()
    {
        if (_isMoving) return;

        // Align the current node's position when not moving
        transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position + offset, 10 * Time.deltaTime);
    }

    public void MoveTo(Node target)
    {
        if (_isMoving || target == null) return;

        if (!nodeGraph.FindPath(currentNode, target, out List<Node> path))
        {
            Debug.LogWarning("No valid path found.");
            return;
        }

        StartCoroutine(MoveAlongPath(path));
    }

    public void MoveTowardTo(Node direction)
    {
        if (_isMoving || direction == null || !currentNode.neighbors.Contains(direction)) return;

        List<Node> path = new() { direction };
        var prev = currentNode;
        var target = direction;
        int i = 0;
        while (target.neighbors.Count == 2 && i++ < _moveTowardBatch)
        {
            var next = target.neighbors[0] == prev ? target.neighbors[1] : target.neighbors[0];
            path.Add(next);
            prev = target;
            target = next;
        }

        StartCoroutine(MoveAlongPath(path, true));
    }

    private IEnumerator MoveAlongPath(List<Node> path, bool keepGoing = false)
    {
        _isMoving = true;

        for (int i = 0; i < path.Count; i++)
        {
            currentNode.Exit(this);
            Node nextNode = path[i];
            yield return StartCoroutine(MoveToPosition(nextNode.transform.position));
            currentNode = nextNode;
            currentNode.Enter(this);
        }

        _isMoving = false;

        if (keepGoing && path[^1].neighbors.Count == 2)
        {
            var next = path[^1].neighbors[0] == path[^2] ? path[^1].neighbors[1] : path[^1].neighbors[0];
            MoveTowardTo(next);
        }
    }

    private IEnumerator MoveToPosition(Vector3 pos)
    {
        pos += offset;
        while (Vector3.Distance(transform.position, pos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = pos;
    }
}
