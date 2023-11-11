using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class DisplaySwitcher : MonoBehaviour
{
    private CharacterTab parentTab;
    public void Start()
    {
        parentTab = GetComponentInParent<CharacterTab>();
    }
    public void DetermineIconAndSetupTextBox()
    {

        if (this.GetComponent<Image>().sprite.name == "ATK")
        {
            parentTab.popup.text.text = "Actions";
        }
        else if(this.GetComponent<Image>().sprite.name == "levelupIcon")
        {
            parentTab.popup.text.text = "Level Up";
        }
        else if(this.GetComponent<Image>().sprite.name == "itemIcon")
        {
            parentTab.popup.text.text = "Items";
        }
        parentTab.popup.gameObject.SetActive(true);
    }

    public void DisablePopup()
    {
        parentTab.popup.gameObject.SetActive(false);
    }


}
