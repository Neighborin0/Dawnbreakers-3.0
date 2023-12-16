using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NamePlate : MonoBehaviour
{
    //public TextMeshProUGUI nameText;
    public GridLayoutGroup IconGrid;
    public GameObject DEF_icon;
    public TextMeshProUGUI defText;
    public Unit unit;

    public void Start()
    {
        if(unit.IsPlayerControlled)
        {
            DEF_icon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(67.1f, DEF_icon.transform.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
    public void UpdateArmor()
    {
        if(!DEF_icon.activeSelf)
        {
            DEF_icon.SetActive(true);
        }
        defText.text = unit.armor.ToString();
        if(unit.armor <= 0)
        {
            DEF_icon.SetActive(false);
        }
    }
   

}
