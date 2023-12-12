using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dusty : Unit
{

    void Awake()
    {
        unitName = "Dusty";
        maxHP = 40;
        attackStat = 5;
        //attackStat = 999;
        defenseStat = 8;
        //speedStat = 7;
        currentHP = maxHP;
        IsPlayerControlled = true;
        resistances = new DamageType[] { DamageType.SLASH };
        weaknesses = new DamageType[] { DamageType.STRIKE };
        Tools.AddItemToInventory(this, "Iron Shield");
        if (!Director.Instance.DevMode)
        {
            actionList.Clear();
            Tools.AddNewActionToUnit(this, "Stab", false);
            Tools.AddNewActionToUnit(this, "Guard", false);
            Tools.AddNewActionToUnit(this, "War Cry", false);
            Tools.AddNewActionToUnit(this, "Whack", false);
        }
    }


}
