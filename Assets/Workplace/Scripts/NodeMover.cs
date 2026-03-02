using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMover : MonoBehaviour
{
    public float moveSpeed;
    public Node currentNode;

    private NodeGraph nodeGraph;
    protected bool _isMoving;

    private void Start()
    {
        nodeGraph = FindFirstObjectByType<NodeGraph>();
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

    private IEnumerator MoveAlongPath(List<Node> path)
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
    }

    private IEnumerator MoveToPosition(Vector3 pos)
    {
        while (Vector3.Distance(transform.position, pos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = pos;
    }
}
