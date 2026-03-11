using UnityEngine;

[RequireComponent(typeof(Node))]
public class CharleAvoidFall : MonoBehaviour, ITimeListener
{
    [SerializeField] private NodeMover charle;
    private Node node;

    private void Start()
    {
        node = GetComponent<Node>();
        TimePublisher.Instance.RegisterListener(this);
        if (charle == null)
        {
            charle = FindFirstObjectByType<NodeMover>();
        }
    }

    public void OnTimeChanged(TimeEventType ev)
    {
        if (charle.currentNode == node)
        {
            charle.MoveTo(node.neighbors[Random.Range(0, node.neighbors.Count)]);
        }
    }

    private void OnDestroy()
    {
        TimePublisher.Instance.UnregisterListener(this);
    }
}
