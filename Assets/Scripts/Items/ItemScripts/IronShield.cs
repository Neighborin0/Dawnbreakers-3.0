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
        itemDescription = "Applies <sprite name=\"BLOCK\"> at the start of battle.";
        CanBeTransfered = false;
        ExcludedFromLootPools = true;
    }
    public override void OnPickup(Unit unit)
    {
        unit.BattlePostStarted += ApplyBlock;
    }

    public void ApplyBlock(Unit unit)
    {
        BattleSystem.Instance.SetTempEffect(unit, "BLOCK", false, 0, 0, 0);
    }
    public override void OnRemoved(Unit unit)
    {
        unit.BattlePostStarted -= ApplyBlock;
    }
}
