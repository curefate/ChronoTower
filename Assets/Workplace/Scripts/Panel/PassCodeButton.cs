using System.Collections;
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
    private Coroutine recoilCoroutine;

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
            if (recoilCoroutine != null)
            {
                StopCoroutine(recoilCoroutine);
            }
            recoilCoroutine = StartCoroutine(Recoil(.5f, originalVisualPosition));
            buttonRotator.ResetRotation();
            passCodePanel.ReleaseButton(isCorrect);
            Debug.Log("Button Released: " + gameObject.name);
        }
        // Press
        else
        {
            isPressed = true;
            pokeInteractableVisual.enabled = false;
            buttonVisual.transform.localPosition = Vector3.zero;
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

    private IEnumerator Recoil(float duration, Vector3 recoilPosition)
    {
        float elapsed = 0f;
        Vector3 startingPosition = buttonVisual.transform.localPosition;

        while (elapsed < duration)
        {
            buttonVisual.transform.localPosition = Vector3.Lerp(startingPosition, recoilPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        buttonVisual.transform.localPosition = recoilPosition;
    }
}
