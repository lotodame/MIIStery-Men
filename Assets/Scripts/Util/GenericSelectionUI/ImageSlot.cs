using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlot : MonoBehaviour, ISelectableItem
{
    Image bgImage;
    void Awake()
    {
        bgImage = GetComponent<Image>();
    }

    Color originalColor;
    public void Init()
    {
        originalColor = bgImage.color;
    }

    public void Clear()
    {
        bgImage.color = originalColor;
    }

    public void OnSelectionChanged(bool selected)
    {
        bgImage.color = (selected) ? GlobalSettings.i.BgHighlightColor : originalColor;
    }
}
