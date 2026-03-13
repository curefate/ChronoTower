using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PasswordPanel : MonoBehaviour
{
    // Buttons
    public List<Transform> buttons;

    // Animation Values
    public float pressAngle = 25f;
    public float animationSpeed = 4f;

    // AudioSource
    public AudioSource audioSource;
    public AudioClip pressSound;

    // Store original rotations
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    // Store pressed states
    private Dictionary<Transform, bool> pressedState = new Dictionary<Transform, bool>();

    // Track running animations
    private Dictionary<Transform, Coroutine> activeAnimations = new Dictionary<Transform, Coroutine>();

    void Start()
    {
        foreach (Transform button in buttons)
        {
            originalRotations[button] = button.localRotation;
            pressedState[button] = false;
        }
    }

    // For testing only
    void Update()
    {
        /* if (Keyboard.current.digit1Key.wasPressedThisFrame) SetPressed(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetPressed(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetPressed(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SetPressed(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SetPressed(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SetPressed(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SetPressed(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SetPressed(7);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SetPressed(8);

        if (Keyboard.current.rKey.wasPressedThisFrame) ResetPanel(); */
    }

    public void SetPressed(int index)
    {
        Debug.Log("Button Animated: " + index);

        if (index < 0 || index >= buttons.Count) return;

        Transform button = buttons[index];

        if (pressedState[button]) return;

        pressedState[button] = true;

        Quaternion targetRot = originalRotations[button] * Quaternion.Euler(0, 0, pressAngle);

        StartButtonAnimation(button, targetRot);

        audioSource.PlayOneShot(pressSound);
    }

    public void ResetPanel()
    {
        foreach (Transform button in buttons)
        {
            pressedState[button] = false;
            StartButtonAnimation(button, originalRotations[button]);
        }

        audioSource.PlayOneShot(pressSound);
    }

    void StartButtonAnimation(Transform button, Quaternion targetRot)
    {
        if (activeAnimations.ContainsKey(button) && activeAnimations[button] != null)
        {
            StopCoroutine(activeAnimations[button]);
        }

        activeAnimations[button] = StartCoroutine(RotateButton(button, targetRot));
    }
    IEnumerator RotateButton(Transform button, Quaternion targetRot)
    {
        while (Quaternion.Angle(button.localRotation, targetRot) > 0.1f)
        {
            
            // button.localRotation = Quaternion.Lerp(
            //     button.localRotation,
            //     targetRot,
            //     Time.deltaTime * animationSpeed
            // );

            button.localRotation = Quaternion.RotateTowards(
                button.localRotation,
                targetRot,
                animationSpeed * 200f * Time.deltaTime
            );

            yield return null;
        }

        button.localRotation = targetRot;

        activeAnimations.Remove(button);
        activeAnimations[button] = null;

    }
}