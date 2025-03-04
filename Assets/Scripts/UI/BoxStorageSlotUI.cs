using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxStorageSlotUI : MonoBehaviour
{
    [SerializeField] Image image;

    public void SetData(Pokemon pokemon)
    {
        image.sprite = pokemon.Base.FrontSprite;
        image.color = new Color(255, 255, 255, 100);
    }

    public void ClearData()
    {
        image.sprite = null;
        image.color = new Color(255, 255, 255, 0);
    }
}
