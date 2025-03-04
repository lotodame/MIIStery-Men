using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxPartySlotUI : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] Image image;

    public void SetData(Pokemon pokemon)
    {
        nameTxt.text = pokemon.Base.name;
        lvlTxt.text = "" + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
        image.color = new Color(255, 255, 255, 100);
    }

    public void ClearData()
    {
        nameTxt.text = "";
        lvlTxt.text = "";
        image.sprite = null;
        image.color = new Color(255, 255, 255, 0);
    }
}
