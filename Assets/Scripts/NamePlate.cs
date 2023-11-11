using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NamePlate : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public GridLayoutGroup IconGrid;
    public GameObject DEF_icon;
    public TextMeshProUGUI defText;
    public Unit unit;

     void Start()
    {
        nameText.outlineWidth = 0.2f;
        nameText.outlineColor = Color.black;
        if (!unit.IsPlayerControlled)
        {
            DEF_icon.gameObject.SetActive(true);
            defText.text = $"{unit.defenseStat}";
        }
        else
        {
            DEF_icon.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (DEF_icon != null)
          {
               defText.text = $"{unit.defenseStat}";
            defText.color = new Color(1, 0.8705882f, 0.7058824f);
        }
            
    }

}
