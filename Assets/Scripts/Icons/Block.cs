using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

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
        Director.Instance.StartCoroutine(CombatTools.PlayVFX(owner.gameObject, "BlockBreak", new Color32(65, 38, 243, 255), new Color(65, 38, 243), new Vector3(0, 0.5f, -2f), Quaternion.identity, 1, 0, false, 0, 2));
        unit.health.DamageModifier = 1f;
        unit.OnDamaged -= RemoveBuff;
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
    }
}
