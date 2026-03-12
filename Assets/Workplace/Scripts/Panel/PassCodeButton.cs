using Oculus.Interaction;
using UnityEngine;

public class PassCodeButton : MonoBehaviour
{
    [SerializeField] private bool isCorrect;
    public bool IsCorrect => isCorrect;
    [SerializeField] private bool isPressed;
    public bool IsPressed => isPressed;

    [SerializeField] private PokeInteractableVisual pokeInteractableVisual;
    [SerializeField] private GameObject buttonVisual;
    [SerializeField] private ButtonRotator buttonRotator;

    private PassCodePanel passCodePanel;
    private Vector3 originalVisualPosition;

    private void Start()
    {
        originalVisualPosition = buttonVisual.transform.localPosition;
        if (buttonRotator == null)
        {
            buttonRotator = GetComponentInChildren<ButtonRotator>();
        }
    }

    public void SetPanel(PassCodePanel panel)
    {
        passCodePanel = panel;
    }

    // Callback
    public void OnPress()
    {
        // Release
        if (isPressed)
        {
            isPressed = false;
            pokeInteractableVisual.enabled = true;
            buttonVisual.transform.localPosition = originalVisualPosition;
            buttonRotator.ResetRotation();
            passCodePanel.ReleaseButton(isCorrect);
            Debug.Log("Button Released: " + gameObject.name);
        }
        // Press
        else
        {
            isPressed = true;
            pokeInteractableVisual.enabled = false;
            buttonVisual.transform.localPosition = originalVisualPosition / 2;
            buttonRotator.Rotate();
            passCodePanel.PressButton(isCorrect);
            Debug.Log("Button Pressed: " + gameObject.name);
        }
    }

    public void Reset()
    {
        isPressed = false;
        pokeInteractableVisual.enabled = true;
        buttonVisual.transform.localPosition = originalVisualPosition;
        buttonRotator.ResetRotation();
    }
}
