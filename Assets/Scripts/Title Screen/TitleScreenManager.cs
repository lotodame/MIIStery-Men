using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string sceneToLoad;

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad); // Replace "GameScene" with your actual scene name
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting..."); // Won't actually quit in editor
    }
}
