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

        maxHP = 22;
        attackStat = 0;
        defenseStat = 0;

        resistances = new DamageType[] { DamageType.LIGHT};
        weaknesses = new DamageType[] { DamageType.DARK };
        currentHP = maxHP;
        IsPlayerControlled = true;
        Tools.AddItemToInventory(this, "TatteredCapeNew");
        if (!Director.Instance.DevMode)
        {
            attackStat = 7;
            actionList.Clear();
            Tools.AddNewActionToUnit(this, "Slash", false);
            Tools.AddNewActionToUnit(this, "Shine", false);
            Tools.AddNewActionToUnit(this, "BlazingStrike", false);
        }
    }
}
