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

    void Awake()
    {
        unitName = "Aurelia";
        maxHP = 32;
        attackStat = 7;
        defenseStat = 6;
        //defenseStat = -2000;
        speedStat = 10;
        //StartingStamina = UnityEngine.Random.Range(85, 95);
        //speedStat = 100;
        currentHP = maxHP;
        IsPlayerControlled = true;
        Tools.AddItemToInventory(this, "Tattered Cape");
        if (!Director.Instance.DevMode)
        {
            attackStat = 7;
            actionList.Clear();
            Tools.AddNewActionToUnit(this, "Slash");
        }
    }
}
