using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource musicSource;
    public float targetVolume = 0.05f; // The volume after fading out

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        musicSource = GetComponent<AudioSource>();
        musicSource.volume = 1f; // Full volume for title screen
    }

    public void FadeToTargetVolume(float duration)
    {
        StartCoroutine(FadeCoroutine(duration));
    }

    private IEnumerator FadeCoroutine(float duration)
    {
        float startVolume = musicSource.volume;
        float endVolume = targetVolume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = endVolume;
    }
}
