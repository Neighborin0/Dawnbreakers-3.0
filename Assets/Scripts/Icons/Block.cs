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

    new void Start()
    {
        iconName = "BLOCK";
        if(owner != null)
        {
            this.owner.OnPreDamaged += ApplyBuff;
            this.owner.OnDamaged += RemoveBuff;
        }
        else
        {
            Debug.LogError("OWNER IS NULL");
        }
        canBeStacked = true;

    }
    public override string GetDescription()
    {
        description = $"Reduces the DMG of \nthe next unarmored hit by 50%.";
        return description;
    }

    public void ApplyBuff(Unit unit)
    {
        this.owner.DamageModifier = 0.5f;
    }

    public void RemoveBuff(Unit unit)
    {
        Director.Instance.StartCoroutine(CombatTools.PlayVFX(owner.gameObject, "BlockBreak", new Color32(65, 38, 243, 255), new Color(65, 38, 243), new Vector3(0, 0.5f, -2f), Quaternion.identity, 0.5f, 0, false, 0, 2));AudioManager.Instance.Play("armor_hit_001", 0, false, 0.6f);
        unit.DamageModifier = 1f;
        unit.OnDamaged -= RemoveBuff;
        unit.OnPreDamaged -= ApplyBuff;
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
    }
}
