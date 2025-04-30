using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class DialogA
{
    [SerializeField] private List<LocalizedString> lines;

    public List<LocalizedString> Lines
    {
        get { return lines; }
    }
}
