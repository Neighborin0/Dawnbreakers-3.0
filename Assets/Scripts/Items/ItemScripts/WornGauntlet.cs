using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "WornGauntlet", menuName = "Assets/Items/WornGauntlet")]
public class WornGauntlet : Item
{
    private void OnEnable()
    {
        itemName = "Worn Gauntlet";
        itemDescription = "Increases ATK by 3.";
    }
    public override void OnPickup(Unit unit)
    {
        unit.attackStat += 3;
    }

    public override void OnRemoved(Unit unit)
    {
        unit.attackStat -= 3;
    }
}
