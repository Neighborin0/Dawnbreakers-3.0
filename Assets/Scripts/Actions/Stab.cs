﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Stab", menuName = "Assets/Actions/Stab")]
public class Stab : Action
{
     private void OnEnable()
    {
        ActionName = "Stab";

        damage = 5;
        lightDamage = 3;
        heavyDamage = 7;

        cost = 40f;
        heavyCost = 60;
        lightCost = 20;

        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.PIERCE;
        damageText = damage.ToString();
    }

    public override string GetDescription()   
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> DMG.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) - targets.defenseStat}</color> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Slash", Color.yellow, Color.yellow ,new Vector3(0, 0, -2f), 1f));
        yield return new WaitForSeconds(0.01f);
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType);
        LabCamera.Instance.Shake(0.2f, 1.5f);
        yield return new WaitForSeconds(0.5f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}