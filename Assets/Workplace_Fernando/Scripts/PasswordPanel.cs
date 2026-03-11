using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PasswordPanel : MonoBehaviour
{
    //Buttons
    public List<Transform> buttons;

    // Animation Values
    public float pressAngle = -45f;
    public float animationSpeed = 8f;

    // AudioSource
    public AudioSource audioSource;
    public AudioClip pressSound;

    // Dictionary that stores the ORIGINAL rotation of every button.
    // This allows us to restore them during reset.
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    // Dictionary that stores whether each button is currently pressed.
    // This prevents pressing the same button multiple times.
    private Dictionary<Transform, bool> pressedState = new Dictionary<Transform, bool>();

    // Start runs once when the object is created.
    void Start()
    {
        // Loop through every button in the list
        foreach (Transform button in buttons)
        {
            // Save the original rotation of the button
            originalRotations[button] = button.localRotation;

            // Initialize the pressed state as false
            pressedState[button] = false;
        }
    }

    //For testing only
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetPressed(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetPressed(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetPressed(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SetPressed(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SetPressed(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SetPressed(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SetPressed(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SetPressed(7);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SetPressed(8);

        if (Keyboard.current.rKey.wasPressedThisFrame) ResetPanel();
    }

    // Function that presses a button based on its index in the list.
    // Example: SetPressed(3) presses the 4th button.
    public void SetPressed(int index)
    {
        Debug.Log("Button pressed: "+index);
        
        // Safety check to prevent invalid index access
        if (index < 0 || index >= buttons.Count) return;

        // Get the button transform
        Transform button = buttons[index];

        // If this button is already pressed, do nothing
        if (pressedState[button]) return;

        // Mark the button as pressed
        pressedState[button] = true;

        // Calculate the target rotation
        Quaternion targetRot = originalRotations[button] * Quaternion.Euler(pressAngle, 0, 0);

        // Start the animation coroutine
        StartCoroutine(RotateButton(button, targetRot));

        // Play the button press sound
        audioSource.PlayOneShot(pressSound);
    }

    // Resets all buttons back to their original rotation
    public void ResetPanel()
    {
        // Loop through every button
        foreach (Transform button in buttons)
        {
            // Reset its pressed state
            pressedState[button] = false;

            // Animate it back to its original rotation
            StartCoroutine(RotateButton(button, originalRotations[button]));
        }

        // Play reset sound
        audioSource.PlayOneShot(pressSound);
    }

    // Coroutine used to animate the button rotation smoothly
    IEnumerator RotateButton(Transform button, Quaternion targetRot)
    {
        // Continue rotating until the difference is very small
        while (Quaternion.Angle(button.localRotation, targetRot) > 0.1f)
        {
            // Smoothly interpolate rotation toward target
            button.localRotation = Quaternion.Lerp(
                button.localRotation,
                targetRot,
                Time.deltaTime * animationSpeed
            );

            // Wait one frame before continuing
            yield return null;
        }

        // Snap exactly to the target rotation at the end
        button.localRotation = targetRot;
    }
}