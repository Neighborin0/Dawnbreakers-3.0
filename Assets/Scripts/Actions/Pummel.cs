﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pummel", menuName = "Assets/Actions/Pummel")]
public class Pummel : Action
{
    private void OnEnable()
    {
        ActionName = "Pummel";

        damage = 0;
        lightDamage = -2;
        heavyDamage = 2;

        cost = 40f;
        heavyCost = 60;
        lightCost = 20;


        damageText = damage.ToString();
        actionType = ActionType.ATTACK;
        targetType = TargetType.ENEMY;
        damageType = DamageType.STRIKE;

        Done = false;
    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color>  <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color>  <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        if (targets != null)
        {
            unit.PlayUnitAction("Attack", unit);
            yield return new WaitUntil(() => unit.Execute);
            LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
            yield return new WaitForSeconds(0.3f);
            //AudioManager.Instance.Play("strike_001");
            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Strike", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 1f));
            targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);
            LabCamera.Instance.Shake(0.3f, 1.5f);
            yield return new WaitForSeconds(0.5f);
            CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
        }
    }

}