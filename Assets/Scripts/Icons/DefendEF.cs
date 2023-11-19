using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class DefendEF : EffectIcon
{
    public override string GetDescription()
    {
        iconName = "DEF";
        description = $"+{storedValue} <sprite name=\"DEF BLUE\">";
        TimedEffect = true;
        return description;
    }

    public override IEnumerator End()
    {
        print($"ICON IS BEING DESTROYED");
        if (DoFancyStatChanges)
        {
            BattleSystem.Instance.SetStatChanges(Stat.DEF, -storedValue, false, owner);
        }
        else
            owner.defenseStat -= (int)storedValue;
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
        yield break;
    }
}
