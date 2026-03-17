using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMover : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public Node currentNode;
    public Vector3 offset;

    private NodeGraph nodeGraph;
    private Animator _animator;
    private bool _isMoving;
    private float _originMoveSpeed;

    private void Start()
    {
        nodeGraph = FindFirstObjectByType<NodeGraph>();
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        _originMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (_isMoving) return;
        transform.position = currentNode.transform.position + offset;
    }

    public void SetMoveState(int state)
    {
        if (_animator != null)
        {
            _animator.SetInteger("MoveState", state);
        }
        if (state == 2 || state == 3)
        {
            moveSpeed = _originMoveSpeed / 2.5f;
        }
        else
        {
            moveSpeed = _originMoveSpeed;
        }
    }

    public void MoveTo(Node target)
    {
        if (_isMoving || target == null) return;

        if (!nodeGraph.FindPath(currentNode, target, out List<Node> path))
        {
            Debug.LogError("No valid path found.");
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
        while (target.neighbors.Count == 2)
        {
            var next = target.neighbors[0] == prev ? target.neighbors[1] : target.neighbors[0];
            path.Add(next);
            prev = target;
            target = next;
        }

        StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<Node> path)
    {
        _isMoving = true;
        _animator.SetInteger("MoveState", 1);

        for (int i = 0; i < path.Count; i++)
        {
            Node nextNode = path[i];
            if (nextNode == currentNode) continue;
            if (nextNode == null || !currentNode.neighbors.Contains(nextNode))
            {
                Debug.LogWarning($"Invalid path segment detected: {currentNode.name} -> {nextNode?.name}");
                _isMoving = false;
                _animator.SetInteger("MoveState", 0);
                yield break;
            }
            currentNode.Exit(this);
            currentNode = nextNode;
            yield return StartCoroutine(MoveToPosition(nextNode.transform.position));
            currentNode.Enter(this);
        }

        _isMoving = false;
        _animator.SetInteger("MoveState", 0);
    }

    private IEnumerator MoveToPosition(Vector3 pos)
    {
        pos += offset;
        while (Vector3.Distance(transform.position, pos) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
            var direction = (transform.position - pos).normalized;
            if (Vector3.ProjectOnPlane(direction, Vector3.up).magnitude > 0.001f)
            {
                direction.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
            yield return null;
        }
        transform.position = pos;
    }
}
