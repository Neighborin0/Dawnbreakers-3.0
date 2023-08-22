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
        Director.Instance.StartCoroutine(Tools.PlayVFX(owner.gameObject, "StatDownVFX", new Color(156, 14, 207), new Color(156, 14, 207), new Vector3(0, 15, 0), 1f, 0, false, 1f, 4));
        Destroy(gameObject);
        yield break;
    }
}
