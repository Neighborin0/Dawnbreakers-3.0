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
        BattleSystem.Instance.SetStatChanges(Stat.ATK, -3f, false, owner);
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
        yield break;
    }
}
