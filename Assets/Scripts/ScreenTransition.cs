using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitioner : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private string nextSceneName;
    
    // Reference to a UI Image that will be used for fading
    [SerializeField] private Image fadeImage;
    
    // Singleton pattern so it can be accessed from anywhere
    public static SceneTransitioner Instance;
    
    private void Awake()
    {
        // Simple singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Make sure we have a fade image
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image not assigned in SceneTransitioner!");
        }
        else
        {
            // Start with the image transparent
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }
    
    // Call this method when your dialogue ends
    public void TransitionToNextScene()
    {
        StartCoroutine(FadeAndLoadScene(nextSceneName));
    }
    
    // Overload to specify a different scene name
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }
    
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        
        // Load the new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Fade back in (optional - remove if you want to start the new scene black)
        yield return StartCoroutine(FadeFromBlack());
    }
    
    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        
        // Gradually increase alpha from 0 to 1
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeFromBlack()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        
        // Gradually decrease alpha from 1 to 0
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}