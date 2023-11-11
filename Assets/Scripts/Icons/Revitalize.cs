using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class Revitalize : EffectIcon
{
    void Start()
    {
        iconName = "revitalize";
        if (owner != null)
        {
            this.owner.OnDamaged += RestoreStamina;
        }
        else
        {
            Debug.LogError("OWNER IS NULL");
        }

    }
    public override string GetDescription()
    {
        description = $"Restores <color=#FFEA29>STM</color> when hit.";
        return description;
    }

    public void RestoreStamina(Unit unit)
    {
        unit.ActionEnded += SetupRevitalize;
        unit.OnDamaged -= RestoreStamina;
    }

    private void SetupRevitalize(Unit obj)
    {

        BattleSystem.Instance.DoTextPopup(obj, "Revitalize", Color.yellow);
        obj.BattlePhaseEnd += RefillStamina;
        obj.ActionEnded -= SetupRevitalize;
        owner.statusEffects.Remove(owner.statusEffects.Where(obj => obj.iconName == iconName).SingleOrDefault());
        Destroy(gameObject);
    }

    private void RefillStamina(Unit obj)
    {
        obj.stamina.slider.value = 100;
        obj.BattlePhaseEnd -= RefillStamina;
    }

}
