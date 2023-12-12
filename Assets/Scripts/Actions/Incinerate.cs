using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Incinerate", menuName = "Assets/Actions/Incinerate")]
public class Incinerate : Action
{
    private void OnEnable()
    {
        ActionName = "Incinerate";

        damage = 50;
        lightDamage = 50;
        heavyDamage = 50;



        cost = 75f;
        lightCost = 80;
        heavyCost = 100;


        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.HEAT;
    }
    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"DEF BLUE\">";
        }
        else
        {
            if((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG."; 
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        var Light = targets.spotLight;
        targets.ChangeUnitsLight(Light, 150, 15, new Color(191, 21, 0), 0.04f, 0.1f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateParticles2", new Color32(191, 21, 0, 255), new Color32(191, 21, 0, 255), new Vector3(0, 0, -2f), 2.5f, 0, true, 6));
        yield return new WaitForSeconds(0.1f);
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, true);
        LabCamera.Instance.Shake(1f, 1.3f);
        yield return new WaitForSeconds(0.5f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
