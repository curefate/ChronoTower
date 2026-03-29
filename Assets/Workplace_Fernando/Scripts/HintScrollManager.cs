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
    public GameObject logo;
    public GameObject instructions;

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
            new Hint { title = "", desc = "" },
            new Hint { title = "HINT", desc = "Look inside the tower." },
            new Hint { title = "WALK", desc = "Lil Charlie moves where he is pushed." },
            new Hint { title = "PLATFORMS", desc = "Some paths move. Pull the red pipes." },
            new Hint { title = "CENTRAL BRIDGE", desc = "No path yet? Try the central cage." },
            new Hint { title = "TIME CONTROL", desc = "Things grow with time. Some shrink." },
            new Hint { title = "PASSWORD", desc = "The tower leaves clues for those who observe." }
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
        logo.SetActive(true);
        instructions.SetActive(true);

        // Optional test loop
        if (autoLoop)
            StartCoroutine(AutoScrollLoop());
    }

    IEnumerator AutoScrollLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(loopInterval);
            OpenScroll();

            yield return new WaitForSeconds(loopInterval);
            CloseScroll();
        }
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

    public void startGame()
    {
        playButtonUsed = true;

        playButton.SetActive(false);
        logo.SetActive(false);
        instructions.SetActive(false);

        SetHint(1);

        Debug.Log("Game Started");
    }
}
