using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "Carrion", menuName = "Assets/Items/Carrion")]
public class Carrion : Item
{
    private void OnEnable()
    {
        itemName = "Carrion";
        itemDescription = "Increases HP by 6.";
    }
    public override void OnPickup(Unit unit)
    {
        unit.currentHP+= 6;
        unit.maxHP += 6;
    }

    public override void OnRemoved(Unit unit)
    {
        unit.currentHP -= 6;
        unit.maxHP -= 6;
    }
}
