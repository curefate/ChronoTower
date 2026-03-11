using UnityEngine;
using UnityEngine.Events;

public class AlignPoint : MonoBehaviour
{
    [SerializeField] private bool isAlignable = true;
    public bool IsAlignable => isAlignable;
    public UnityEvent onAlign;
    public UnityEvent onExit;
}
