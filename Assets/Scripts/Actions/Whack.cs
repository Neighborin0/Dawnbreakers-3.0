﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
[CreateAssetMenu(fileName = "Whack", menuName = "Assets/Actions/Whack")]
public class Whack : Action
{
    private void OnEnable()
    {
        ActionName = "Whack";
        damage = 2;
        cost = 40f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        damageText = damage.ToString();
        duration = 5f;
    }

    public override string GetDescription()   
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + damage}</color> DMG. Applies <sprite name=\"STAGGER\"> for {duration} seconds.";
        }
        else
        {
            if (damage + unit.attackStat - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{damage + unit.attackStat - targets.defenseStat}</color> DMG. Applies <sprite name=\"STAGGER\"> for {duration} seconds";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG. Applies <sprite name=\"STAGGER\"> for {duration} seconds";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, 8, -50, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Color(156, 14, 207), new Vector3(0, 0, -2f), 1f));
        yield return new WaitForSeconds(0.01f);
        targets.health.TakeDamage(damage + unit.attackStat, unit);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        BattleSystem.Instance.SetTempEffect(targets, "STAGGER", true, duration);
        yield return new WaitForSeconds(0.5f);
        Done = true;
        LabCamera.Instance.ResetPosition();
    }

  
}