using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharleChecker : MonoBehaviour
{
    [SerializeField] private NodeMover nodeMover;
    [SerializeField] private List<Node> nodesToCheck;
    [SerializeField] private Collider dragHandle;

    void Start()
    {
        if (nodeMover == null)
        {
            nodeMover = FindFirstObjectByType<NodeMover>();
        }
        if (nodesToCheck != null)
        {
            nodesToCheck = nodesToCheck.Distinct().Where(node => node != null).ToList();
        }
    }

    void Update()
    {
        if (CheckCharle())
        {
            dragHandle.enabled = false;
        }
        else
        {
            dragHandle.enabled = true;
        }
    }

    private bool CheckCharle()
    {
        if (nodesToCheck == null || nodesToCheck.Count == 0) return false;

        foreach (Node node in nodesToCheck)
        {
            if (nodeMover.currentNode == node)
            {
                return true;
            }
        }
        return false;
    }
}
