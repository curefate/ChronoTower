using UnityEngine;

public class SignifierHoverTester : MonoBehaviour
{
    private SignifierManager signifier;

    void Start()
    {
        // Find SignifierManager in children (SignifierBounding prefab)
        signifier = GetComponentInChildren<SignifierManager>();

        if (signifier == null)
        {
            Debug.LogWarning("SignifierManager not found in children of " + gameObject.name);
        }
    }

    void OnMouseEnter()
    {
        if (signifier != null)
        {
            Debug.Log("Hover Enter");
            signifier.ActivateSignifier();
        }
    }

    void OnMouseExit()
    {
        if (signifier != null)
        {
            Debug.Log("Hover Exit");
            signifier.DeactivateSignifier();
        }
    }
}