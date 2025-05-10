using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components; // Required for LocalizedAsset<T>

[System.Serializable]
public class DialogA
{
    [SerializeField] private List<LocalizedString> lines;
    [SerializeField] private List<AudioClip> soundEffects;
    [SerializeField] private List<LocalizedAsset<Sprite>> localizedImages;

    public List<LocalizedString> Lines => lines;
    public List<AudioClip> SoundEffects => soundEffects;
    public List<LocalizedAsset<Sprite>> LocalizedImages => localizedImages;
}
