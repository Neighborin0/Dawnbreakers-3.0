using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "CrackedShield", menuName = "Assets/Items/CrackedShield")]
public class CrackedShield : Item
{
    private void OnEnable()
    {
        itemName = "Cracked Shield";
        itemDescription = "Increases <sprite name=\"DEF BLUE\"> by 3.";
    }
    public override void OnPickup(Unit unit)
    {
        unit.defenseStat += 3;
    }

    public override void OnRemoved(Unit unit)
    {
        unit.defenseStat -= 3;
    }
}
