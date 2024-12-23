﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "TatteredCape", menuName = "Assets/Items/TatteredCape")]
public class TatteredCape : Item
{
    private void OnEnable()
    {
        itemName = "Tattered Cape";
        itemDescription = "Restores 10% <sprite name=\"HP\"> after battle.";
        CanBeTransfered = false;
        ExcludedFromLootPools = true;
    }
    public override void OnPickup(Unit unit)
    {
        unit.BattleEnded += DoHeal;
    }

    public void DoHeal(Unit unit)
    {
        BattleSystem.Instance.SetStatChanges(Stat.HP, (int)Mathf.Round(unit.maxHP * 0.1f), false, unit);
        var Light = unit.GetComponentInChildren<Light>();
        Light.color = Color.green;
        BattleLog.Instance.StartCoroutine(Tools.ChangeLightIntensityTimed(Light, 150, 15, 0.04f, 1f));
    }
    public override void OnRemoved(Unit unit)
    {
        unit.BattleEnded -= DoHeal;
    }
}
