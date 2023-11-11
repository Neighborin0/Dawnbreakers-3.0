using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.AI;


public class ItemText : MonoBehaviour
{
    public Item item;
    public TextMeshProUGUI textMesh;
    public bool isSelected = false;
    public Unit unit;


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
            BattleLog.Instance.itemText.text = "";
            BattleLog.Instance.itemText.text = item.itemDescription;
        }
        else
        {
            foreach(var itemText in GameObject.FindObjectsOfType<ItemText>()) 
            {
                itemText.isSelected = false;
            }
            BattleLog.Instance.ambientText.gameObject.SetActive(true);
            BattleLog.Instance.ambientText.text = "";
            BattleLog.Instance.ambientText.text = $"{item.itemName}\n{item.itemDescription}";
            if(!item.CanBeTransfered)
            {
                BattleLog.Instance.ambientText.text += "\n<color=#FF0000>Can't be transferred.</color>";
            }
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
            isSelected = true;
        }
    }




}
