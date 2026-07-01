using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "IronShield", menuName = "Assets/Items/IronShield")]
public class IronShield : Item
{
    private void OnEnable()
    {
        itemName = "Iron Shield";
        itemDescription = "Applies <color=#4126F4>BLOCK</color> to a lowest <color=#FF0000>HP</color> unit at the start of battle.";
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

        List<Unit> playerUnits = BattleSystem.Instance.playerUnits;

        playerUnits.OrderBy(currentHP => currentHP);
        Unit targetUnit = playerUnits.Where(playerUnit =>playerUnit != null && playerUnit.currentHP > 0).OrderBy(playerUnit => playerUnit.currentHP).FirstOrDefault();
        BattleSystem.Instance.SetTempEffect(targetUnit,"BLOCK", false, 0, 0, 1);
    }

    public override void OnRemoved(Unit unit)
    {
        unit.BattleStarted -= ApplyEffect;
    }
}
