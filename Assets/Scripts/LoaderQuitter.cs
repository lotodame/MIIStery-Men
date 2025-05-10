using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderQuitter : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit called — will only work in a build.");
    }
}
