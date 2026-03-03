using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassCodePanel : MonoBehaviour
{
    [SerializeField] private List<PassCodeButton> buttons;

    private int correctRequire;
    private int validPressTimes;
    public UnityEvent OnPasscodeCorrect;
    public UnityEvent OnPasscodeWrong;

    private void Start()
    {
        if (buttons == null || buttons.Count == 0)
        {
            Debug.LogError("PassCodePanel: No buttons assigned.");
            return;
        }

        correctRequire = buttons.FindAll(button => button.IsCorrect).Count;
        validPressTimes = correctRequire;

        foreach (var button in buttons)
        {
            button.SetPanel(this);
        }
    }

    public void PressButton(bool isCorrect)
    {
        validPressTimes--;
        if (isCorrect)
        {
            correctRequire--;
        }

        if (correctRequire == 0)
        {
            OnPasscodeCorrect?.Invoke();
            ResetPanel();
            return;
        }

        if (validPressTimes == 0)
        {
            OnPasscodeWrong?.Invoke();
            ResetPanel();
            return;
        }
    }

    public void ReleaseButton(bool isCorrect)
    {
        validPressTimes++;
        if (isCorrect)
        {
            correctRequire++;
        }
    }

    private void ResetPanel()
    {
        correctRequire = buttons.FindAll(button => button.IsCorrect).Count;
        validPressTimes = correctRequire;

        foreach (var button in buttons)
        {
            button.Reset();
        }
    }
}
