using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;



public class Aurelia : Unit
{
    private string[] summons =
   {
        "Husk",
   };
    void Awake()
    {
        unitName = "Aurelia";
        maxHP = 32;
        attackStat = 7;
        //attackStat = 999;
        defenseStat = 6;
        //defenseStat = -2000;
        speedStat = 10;
        //StartingStamina = UnityEngine.Random.Range(85, 95);
        StartingStamina = 95;
        //speedStat = 100;
        currentHP = maxHP;
        summonables = summons;
        IsPlayerControlled = true;
        Tools.AddItemToInventory(this, "Tattered Cape");
        if (!Director.Instance.DevMode)
        {
            actionList.Clear();
            Tools.AddNewActionToUnit(this, "Slash");
        }
    }

    public static List<LabLine> AureliaDeath1 = new List<LabLine>
    {
         new LabLine { expression = "neutral", text = "You suck ass bruh.",  unit = "Aurelia", textSpeed = 0.02f,},
         new LabLine { expression = "neutral", text = "It is so over.", unit = "Aurelia", textSpeed = 0.03f, }
    };
}
