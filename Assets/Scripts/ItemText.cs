using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.AI;
using static System.Collections.Specialized.BitVector32;


public class ItemText : MonoBehaviour
{
    public Item item;
    public TextMeshProUGUI textMesh;
    public bool isSelected = false;
    public Unit unit;
    private GameObject currentEffectPopup;


    private void Start()
    {
        if(BattleSystem.Instance == null)
        unit = this.GetComponentInParent<CharacterTab>().unit;
        if(!item.CanBeTransfered)
            this.GetComponent<DraggableObject>().CanBeDragged = false;
    }
    public void DisplayDescription()
    {
        if (BattleSystem.Instance != null)
        {
            if (currentEffectPopup == null)
            {
                var EP = Instantiate(Director.Instance.EffectPopUp, Director.Instance.canvas.transform);
                EP.transform.localScale = new Vector3(1.5f, 1.5f, -25f);
                currentEffectPopup = EP;
            }
            else
            {
                currentEffectPopup.SetActive(true);
            }
            var rect = transform.position;
            currentEffectPopup.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(rect.x - 686, rect.y - 460);
            var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
            EPtext.text = $"{item.itemDescription}";
            StartCoroutine(Tools.UpdateParentLayoutGroup(EPtext.gameObject));
        }
        else
        {
            foreach(var itemText in GameObject.FindObjectsOfType<ItemText>()) 
            {
                itemText.isSelected = false;
            }
            if (currentEffectPopup == null)
            {
                var EP = Instantiate(Director.Instance.EffectPopUp, Director.Instance.canvas.transform);
                EP.transform.localScale = new Vector3(1.5f, 1.5f, -25f);
                currentEffectPopup = EP;
            }
            else
            {
                currentEffectPopup.SetActive(true);
            }
            var rect = transform.position;
            currentEffectPopup.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(rect.x - 686, rect.y - 460);
            var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
            EPtext.text = $"{item.itemDescription}";
            StartCoroutine(Tools.UpdateParentLayoutGroup(EPtext.gameObject));
            if (!item.CanBeTransfered)
            {
                EPtext.text += "\n<color=#FF0000>Can't be transferred.</color>";
            }
            isSelected = true;
        }
    }

    public void RemoveDescription()
    {
        if (currentEffectPopup != null)
            currentEffectPopup.SetActive(false);
    }



}
