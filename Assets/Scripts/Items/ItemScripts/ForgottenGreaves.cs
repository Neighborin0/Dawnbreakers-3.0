using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
[CreateAssetMenu(fileName = "ForgottenGreaves", menuName = "Assets/Items/ForgottenGreaves")]
public class ForgottenGreaves : Item
{
    private void OnEnable()
    {
        itemName = "Forgotten Greaves";
        itemDescription = "Increases <sprite name=\"SPD YLW\"> by 3.";
    }
    public override void OnPickup(Unit unit)
    {
        unit.speedStat += 3;
    }

    public override void OnRemoved(Unit unit)
    {
        unit.speedStat -= 3;
    }
}
