using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class DialogManagerA : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManagerA Instance { get; private set; }

    private void Awake() { Instance = this; }

    DialogA dialog;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialog(DialogA dialog)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        yield return TypeLocalizedDialog(dialog.Lines[0]);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeLocalizedDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    private IEnumerator TypeLocalizedDialog(LocalizedString localizedLine)
    {
        isTyping = true;
        dialogText.text = "";

        // Fetch the localized string asynchronously
        var stringOperation = localizedLine.GetLocalizedStringAsync();
        yield return stringOperation;

        string line = stringOperation.Result;

        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        isTyping = false;
    }
}
