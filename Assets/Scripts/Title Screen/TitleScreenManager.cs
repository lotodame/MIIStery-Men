using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenManager : MonoBehaviour
{
    public string sceneToLoad;
    public ScreenFader screenFader;
    public BackgroundMusic music;

    public void StartGame()
    {
        StartCoroutine(StartGameWithFade());
    }

    IEnumerator StartGameWithFade()
    {
        float fadeDuration = 2f;

        if (music != null)
            music.FadeToTargetVolume(fadeDuration); // fades music to targetVolume

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeToBlack(fadeDuration)); // fades screen

        SceneManager.LoadScene(sceneToLoad);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
