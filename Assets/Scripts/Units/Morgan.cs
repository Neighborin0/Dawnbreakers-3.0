using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morgan : Unit
{

    void Awake()
    {
        unitName = "Morgan";
        maxHP = 20;
        attackStat = 0;
        defenseStat = 0;
        currentHP = maxHP;
        IsPlayerControlled = true;
        resistances = new DamageType[] { DamageType.DARK };
        weaknesses = new DamageType[] { DamageType.SLASH };
        Tools.AddItemToInventory(this, "Umbral Tome");
        if (!Director.Instance.DevMode)
        {
            actionList.Clear();
            Tools.AddNewActionToUnit(this, "Frost", false);
            Tools.AddNewActionToUnit(this, "Expose", false);
            Tools.AddNewActionToUnit(this, "Bind", false);
        }
    }


}
