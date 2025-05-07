using UnityEngine;
using System.Collections;

public class SceneIntroController : MonoBehaviour
{
    public DialogA introDialog;
    public ScreenFader screenFader;

    private bool dialogFinished = false;

    void Start()
    {
        StartCoroutine(HandleSceneIntro());
    }

    IEnumerator HandleSceneIntro()
    {
        // Show dialog immediately
        DialogManagerA.Instance.OnCloseDialog += OnDialogFinished;
        yield return DialogManagerA.Instance.ShowDialog(introDialog);

        // Wait until player finishes dialog
        yield return new WaitUntil(() => dialogFinished);

        // Now fade the screen in
        if (screenFader != null)
        {
            yield return screenFader.FadeToClear(2f);
        }
    }

    void OnDialogFinished()
    {
        dialogFinished = true;
        DialogManagerA.Instance.OnCloseDialog -= OnDialogFinished;
    }
}
