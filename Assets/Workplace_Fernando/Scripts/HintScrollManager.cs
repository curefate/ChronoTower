using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class HintScrollManager : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    private bool playButtonUsed = false;

    public GameObject playButton;

    [System.Serializable]
    public class Hint
    {
        public string title;
        public string desc;
    }
    Hint[] hints;

    public Transform rightTube;
    public Transform paper;

    [Header("Settings")]
    public float openDistance = 1.0f;
    public float duration = 0.3f;

    [Header("Testing Only")]
    public bool autoLoop = false;
    public float loopInterval = 10f;

    private Vector3 rightTubeClosedPos;
    private Vector3 rightTubeOpenPos;

    private Vector3 paperClosedScale;
    private Vector3 paperOpenScale;

    private bool isOpen = false;
    private Coroutine currentRoutine;

    void Start()
    {
        hints = new Hint[]
        {
            new Hint { title = "WELCOME", desc = "" }, //0
            new Hint { title = "HINT", desc = "Look inside the tower." }, //1
            new Hint { title = "WALK", desc = "Lil Charlie moves where he is pushed." }, //2
            new Hint { title = "PLATFORMS", desc = "Not every path is fixed. Try pulling the red pipes." }, //3
            new Hint { title = "CENTRAL\nBRIDGE", desc = "If a path doesn’t exist yet, the central cage might help." }, //4
            new Hint { title = "TIME\nCONTROL", desc = "You got the power of time by the buttons below..." }, //5
            new Hint { title = "PASSWORD", desc = "The tower leaves 3 clues inside the tower." }, //6
            new Hint { title = "BLOCKED", desc = "Vines can't grow through blocked paths."}, // 7
            new Hint { title = "CONGRATULATIONS", desc = "Thank you for playing!"} // 8 mother fucker!!!
        };

        SetHint(0);

        // Cache positions
        rightTubeClosedPos = rightTube.localPosition;
        rightTubeOpenPos = rightTubeClosedPos + new Vector3(openDistance, 0, 0);

        // Paper scale
        paperClosedScale = new Vector3(0.1f, paper.localScale.y, paper.localScale.z);
        paperOpenScale = new Vector3(1f, paper.localScale.y, paper.localScale.z);

        // Force CLOSED state
        rightTube.localPosition = rightTubeClosedPos;
        paper.localScale = paperClosedScale;
        isOpen = false;

        // UI states
        playButton.SetActive(false);
    }

    void Update()
    {
        // Debug controls
        if (Keyboard.current.digit1Key.wasPressedThisFrame) OpenScroll();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) CloseScroll();
    }

    public void OpenScroll()
    {
        if (isOpen) return;

        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(OpenRoutine());
    }

    public void CloseScroll()
    {
        if (!isOpen) return;

        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(CloseRoutine());
    }

    public void CloseScrollImmediately()
    {
        if (!isOpen) return;

        playButton.SetActive(false);
    }

    IEnumerator OpenRoutine()
    {
        isOpen = true;

        float t = 0;

        Vector3 startTubePos = rightTube.localPosition;
        Vector3 startScale = paper.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            float eased = 1 - Mathf.Pow(1 - progress, 3);

            rightTube.localPosition = Vector3.Lerp(startTubePos, rightTubeOpenPos, eased);
            paper.localScale = Vector3.Lerp(startScale, paperOpenScale, eased);

            yield return null;
        }

        if (!playButtonUsed)
            playButton.SetActive(true);
    }

    IEnumerator CloseRoutine()
    {
        isOpen = false;

        playButton.SetActive(false);

        float t = 0;

        Vector3 startTubePos = rightTube.localPosition;
        Vector3 startScale = paper.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            float eased = 1 - Mathf.Pow(1 - progress, 3);

            rightTube.localPosition = Vector3.Lerp(startTubePos, rightTubeClosedPos, eased);
            paper.localScale = Vector3.Lerp(startScale, paperClosedScale, eased);

            yield return null;
        }
    }

    public void SetHint(int index)
    {
        if (index >= 0 && index < hints.Length)
        {
            titleText.text = hints[index].title;
            descriptionText.text = hints[index].desc;
        }
    }
}
