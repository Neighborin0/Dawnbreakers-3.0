using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "UmbralTome", menuName = "Assets/Items/UmbralTome")]
public class UmbralTome : Item
{
    private void OnEnable()
    {
        itemName = "Umbral Tome";
        itemDescription = "Applies <color=#6A00FF>HEX</color> to a random enemy unit at the start of battle.";
        CanBeTransfered = false;
        ExcludedFromLootPools = true;
    }
    public override void OnPickup(Unit unit)
    {
        if (unit == null)
            return;

        // Prevent duplicate event subscriptions.
        unit.BattleStarted -= ApplyEffect;
        unit.BattleStarted += ApplyEffect;
    }

    private void ApplyEffect(Unit unit)
    {
        if (unit == null)
            return;

        if(BattleSystem.Instance == null)
            return;


        Unit targetUnit = BattleSystem.Instance.enemyUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.enemyUnits.Count)];
        BattleSystem.Instance.SetTempEffect(targetUnit,"HEX", false, 0, 0, 1);
    }

    public override void OnRemoved(Unit unit)
    {
        unit.BattleStarted -= ApplyEffect;
    }
}
