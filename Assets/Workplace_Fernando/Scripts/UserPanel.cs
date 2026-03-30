using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class UserPanel : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    private string[] textsHints;

    public GameObject acessibilityButtons;
    public GameObject playButton;

    [System.Serializable]
    public class Hint
    {
        public string title;
        public string desc;
    }
    Hint[] hints;

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
    }

    //Tempo cod for testing
    void Update()
    {
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SetHint(0); //0 - Welcome Initial State
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetHint(1); //1 - Look Inside
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetHint(2); //2 - Push Lil Charlie
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetHint(3); //3 - Blue Interactive Platforms
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetHint(4); //4 - Bridge
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SetHint(5); //5 - Time Control
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SetHint(6); //6 - Password Gate
    }

    public void SetHint(int index)
    {
        if (index >= 0 && index < hints.Length)
        {
            titleText.text = hints[index].title;
            descriptionText.text = hints[index].desc;
        }
        else
        {
            Debug.LogWarning("Index out of range");
        }
    }

    private IEnumerator DelaySetHint(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetHint(index);
    }

    public void cleanHints()
    {
        titleText.text = "";
        descriptionText.text = "";
    }

    public void FadeButton()
    {
        StartCoroutine(FadePlayButton());
    }

    IEnumerator FadePlayButton()
    {
        CanvasGroup cg = playButton.GetComponent<CanvasGroup>();

        float duration = 0.15f;
        float t = 0;

        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(1, 0, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0;
        playButton.SetActive(false);
        SetHint(1);
        StartCoroutine(DelaySetHint(2, 5f));
    }

    public void ffTime()
    {
        Debug.Log("Should Fast Foward Time");
    }

    public void rwTime()
    {
        Debug.Log("Should Rewind Time");
    }

    public void showHideAccesbilityButtons(bool status)
    {
        acessibilityButtons.SetActive(status);
    }

}