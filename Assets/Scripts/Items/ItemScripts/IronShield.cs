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
        itemDescription = "Grants 2 <sprite name=\"DEF BLUE2\">.";
        CanBeTransfered = false;
        ExcludedFromLootPools = true;
    }
    public override void OnPickup(Unit unit)
    {
        unit.defenseStat += 2;
    }

    public override void OnRemoved(Unit unit)
    {
        unit.defenseStat -= 2;
    }
}
