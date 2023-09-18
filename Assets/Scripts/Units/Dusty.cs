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
        speedStat = 7;
        StartingStamina = UnityEngine.Random.Range(80, 90);
        currentHP = maxHP;
        IsPlayerControlled = true;
        Tools.AddItemToInventory(this, "Iron Shield");
        if (!Director.Instance.DevMode)
        {
            actionList.Clear();
            Tools.AddNewActionToUnit(this, "Slash");
            Tools.AddNewActionToUnit(this, "Guard");
        }
    }

    public static List<LabLine> DustyDeath1 = new List<LabLine>
    {
        new LabLine { expression = "neutral", text = "Prostate Cancer got me", unit = "Dusty", textSpeed = 0.03f, }
    };


}
