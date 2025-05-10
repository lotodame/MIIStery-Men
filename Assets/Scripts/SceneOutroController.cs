using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneOutroController : MonoBehaviour
{
    public ScreenFader screenFader;
    public DialogA finalDialog;
    public string nextSceneName;

    private bool shouldEnd = false;
    private bool dialogFinished = false;

    public void PrepareForSceneEnd()
    {
        if (!shouldEnd)
        {
            shouldEnd = true;
            DialogManagerA.Instance.OnCloseDialog += HandleFinalItemDialogFinished;
        }
    }

    private void HandleFinalItemDialogFinished()
    {
        DialogManagerA.Instance.OnCloseDialog -= HandleFinalItemDialogFinished;
        StartCoroutine(HandleSceneOutro());
    }

    private IEnumerator HandleSceneOutro()
    {
        if (screenFader != null)
        {
            yield return screenFader.FadeToBlack(2f);
        }

        if (finalDialog != null)
        {
            DialogManagerA.Instance.OnCloseDialog += OnFinalDialogClosed;
            yield return DialogManagerA.Instance.ShowDialog(finalDialog);
            yield return new WaitUntil(() => dialogFinished);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnFinalDialogClosed()
    {
        dialogFinished = true;
        DialogManagerA.Instance.OnCloseDialog -= OnFinalDialogClosed;
    }
}
