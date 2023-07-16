using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class Vigor : EffectIcon
{
    public void Start()
    {
        iconName = "Vigor";
    }
    public override string GetDescription()
    {
        description = $"+{storedValue} <sprite name=\"ATK RED2\">";
        return description;
    }

    public override IEnumerator End()
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, -3f, false, owner);
        Destroy(gameObject);
        yield break;
    }
}
