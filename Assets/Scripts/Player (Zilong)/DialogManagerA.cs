using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;


public class DialogManagerA : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond = 15;

    [Header("Audio")]
    [SerializeField] AudioClip dialogBlipSound;
    [SerializeField] AudioClip dialogAdvanceSound;

    [SerializeField] private Image dialogImageDisplay; // Assign in Inspector

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManagerA Instance { get; private set; }

    private AudioSource blipSource;
    private AudioSource sfxSource;

    private DialogA dialog;
    private int currentLine = 0;
    private bool isTyping;
    private bool skipTyping;

    private void Awake()
    {
        Instance = this;

        // Get two AudioSources attached to this GameObject
        var sources = GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            Debug.LogError("DialogManagerA requires two AudioSources on the same GameObject.");
        }

        blipSource = sources[0];
        sfxSource = sources[1];
    }

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
        // 🔁 Skip typing if already typing
        if (isTyping && Input.GetKeyDown(KeyCode.Z))
        {
            skipTyping = true;
            return;
        }

        // Advance if not typing
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            // 🔇 Fade out blip sound
            if (blipSource.isPlaying)
            {
                StartCoroutine(FadeOutBlip());
            }

            // 🔊 Play dialog advance sound
            if (dialogAdvanceSound != null)
            {
                sfxSource.PlayOneShot(dialogAdvanceSound);
            }

            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeLocalizedDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;

                if (blipSource.isPlaying)
                {
                    StartCoroutine(FadeOutBlip());
                }

                dialogBox.SetActive(false);

                // ✅ Instantly hide the image after the dialog ends
                if (dialogImageDisplay != null && dialogImageDisplay.gameObject.activeSelf)
                {
                    var color = dialogImageDisplay.color;
                    color.a = 0f;
                    dialogImageDisplay.color = color;
                    dialogImageDisplay.gameObject.SetActive(false);
                }

                OnCloseDialog?.Invoke();
            }
        }
    }


    private IEnumerator TypeLocalizedDialog(LocalizedString localizedLine)
    {
        isTyping = true;
        skipTyping = false;
        dialogText.text = "";

        var stringOperation = localizedLine.GetLocalizedStringAsync();
        yield return stringOperation;

        string line = stringOperation.Result;

        // 🔊 Start blip sound
        if (dialogBlipSound != null)
        {
            blipSource.Stop();
            blipSource.clip = dialogBlipSound;
            blipSource.volume = 1f;
            blipSource.Play();
        }

        // Typing each letter unless skipping
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;

            if (skipTyping)
            {
                dialogText.text = line;
                break;
            }

            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        // 🔇 Fade out blip when typing ends
        if (blipSource.isPlaying)
        {
            StartCoroutine(FadeOutBlip());
        }

        // 🔊 Play line-specific SFX if it exists
        if (dialog.SoundEffects != null && currentLine < dialog.SoundEffects.Count)
        {
            AudioClip clip = dialog.SoundEffects[currentLine];
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        // 🔳 Show post-line image if one exists
        if (dialog.LocalizedImages != null && currentLine < dialog.LocalizedImages.Count)
        {
            var localizedAsset = dialog.LocalizedImages[currentLine];
            if (localizedAsset != null && dialogImageDisplay != null)
            {
                var imageOp = localizedAsset.LoadAssetAsync();
                yield return imageOp;

                Sprite image = imageOp.Result;
                if (image != null)
                {
                    dialogImageDisplay.sprite = image;
                    StartCoroutine(FadeInImage(dialogImageDisplay, 1f));
                }
            }
        }


        isTyping = false;
    }

    private IEnumerator FadeOutBlip(float duration = 0.1f)
    {
        float startVolume = blipSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            blipSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        blipSource.Stop();
        blipSource.volume = startVolume;
        blipSource.clip = null;
    }

    private IEnumerator FadeInImage(Image image, float duration)
    {
        Color color = image.color;
        float time = 0f;

        color.a = 0f;
        image.color = color;
        image.gameObject.SetActive(true);

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / duration);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Ensure it ends fully visible
        color.a = 1f;
        image.color = color;
    }

    private IEnumerator FadeOutImage(Image image, float duration)
    {
        Color color = image.color;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (time / duration));
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        color.a = 0f;
        image.color = color;
        image.gameObject.SetActive(false);
    }

}