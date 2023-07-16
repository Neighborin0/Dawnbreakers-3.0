using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

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
        Destroy(gameObject);
    }

    private void RefillStamina(Unit obj)
    {
        obj.stamina.slider.value = 100;
        obj.BattlePhaseEnd -= RefillStamina;
    }

}
