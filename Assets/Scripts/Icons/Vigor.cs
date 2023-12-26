using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class Vigor : EffectIcon
{
    
    public void Start()
    {
        iconName = "Vigor";
        TimedEffect = true;
    }
    public override string GetDescription()
    {
        description = $"+{storedValue} <sprite name=\"ATK RED2\">";
        return description;
    }

    public override IEnumerator End()
    {
        if(DoFancyStatChanges)
        {
            BattleSystem.Instance.SetStatChanges(Stat.ATK, -storedValue, false, owner);
        }
        else
        {
            owner.attackStat -= (int)storedValue;
        }
       
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
        yield break;
    }
}
