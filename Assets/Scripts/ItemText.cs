﻿using System.Collections;
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
            BattleLog.Instance.battleText.gameObject.SetActive(true);
            BattleLog.Instance.battleText.text = "";
            BattleLog.Instance.battleText.text = $"{item.itemName}\n{item.itemDescription}";
            if(!item.CanBeTransfered)
            {
                BattleLog.Instance.battleText.text += "\nCan't be transferred.";
            }
            isSelected = true;
        }
    }




}