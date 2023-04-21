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
    private string[] TutorialSummons = new string[] { "Husk" };
    void Awake()
    {
        unitName = "Aurelia";
        maxHP = 30;
        attackStat = 7;
        //attackStat = 1;
        defenseStat = 5;
        speedStat = 10;
        //speedStat = 100;
        currentHP = maxHP;
        IsPlayerControlled = true;
        summonables = TutorialSummons;
        Tools.AddItemToInventory(this, "Tattered Cape");
        //Tools.AddItemToInventory(this, "Worn Gauntlet");
    }

}
