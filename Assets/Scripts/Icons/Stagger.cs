﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class Stagger : EffectIcon
{
     void Start()
    {
        iconName = "STAGGER";
        TimedEffect = true;
    }
    public override string GetDescription()
    {
        description = $"Triggers certain effects when consumed...";
        return description;
    }

    public override IEnumerator End()
    {
        Director.Instance.StartCoroutine(CombatTools.PlayVFX(owner.gameObject, "StatDownVFX", new Color(156, 14, 207), new Color(156, 14, 207), new Vector3(0, 15, 0), Quaternion.identity, 1f, 0, false, 1f, 1));
        owner.ChangeUnitsLight(owner.spotLight, 20, 2, new Color(156, 14, 207), 0.04f, 0.1f);
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
        yield break;
    }
}
