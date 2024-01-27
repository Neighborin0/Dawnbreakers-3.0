using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morgan : Unit
{

    void Awake()
    {
        unitName = "Morgan";
        maxHP = 40;
        attackStat = 3;
        defenseStat = 4;
        currentHP = maxHP;
        IsPlayerControlled = true;
        resistances = new DamageType[] { DamageType.SLASH };
        weaknesses = new DamageType[] { DamageType.STRIKE };
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
