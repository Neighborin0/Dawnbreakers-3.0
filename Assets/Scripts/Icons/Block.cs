using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class Block : EffectIcon
{
   
    void Start()
    {
        iconName = "BLOCK";
        if(owner != null)
        {
            this.owner.health.DamageModifier = 0.5f;
            this.owner.OnDamaged += RemoveBuff;
        }
        else
        {
            Debug.LogError("OWNER IS NULL");
        }
       
    }
    public override string GetDescription()
    {
        description = $"Reduces the DMG of the next hit by 50%.";
        return description;
    }

    public void RemoveBuff(Unit unit)
    {
        Director.Instance.StartCoroutine(Tools.PlayVFX(owner.gameObject, "BlockBreak", new Color(53, 47, 191), new Color(53, 47, 191), new Vector3(0, 0.5f, -2f), 1, 0, false, 0, 0));
        unit.health.DamageModifier = 1f;
        unit.OnDamaged -= RemoveBuff;
        Destroy(gameObject);
    }
}
