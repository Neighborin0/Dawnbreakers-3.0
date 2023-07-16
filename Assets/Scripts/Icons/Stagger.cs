using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class Stagger : EffectIcon
{
     void Start()
    {
        iconName = "STAGGER";
    }
    public override string GetDescription()
    {
        description = $"Triggers certain effects when consumed...";
        return description;
    }

    public override IEnumerator End()
    {
        Destroy(gameObject);
        yield break;
    }
}
