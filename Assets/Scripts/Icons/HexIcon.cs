using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class HexIcon : EffectIcon
{

    new void Start()
    {
        iconName = "HEX";
        if(owner != null)
        {
            this.owner.damageAddend += 1f;
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
        description = $"The next time this unit takes damage, it takes 1 additional damage. Then remove <color=#6C00FF>Hex</color>.";
        return description;
    }

    public void RemoveBuff(Unit unit)
    {   
        unit.damageAddend -= 1f;
        unit.OnDamaged -= RemoveBuff;
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
    }
}
