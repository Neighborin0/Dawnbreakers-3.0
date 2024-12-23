﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Slow", menuName = "Assets/Actions/Slow")]
public class Slow : Action
{
    private void OnEnable()
    {
        ActionName = "Slow";
        damage = 2;
        cost = 75f;
        statAmount = 2;
        targetType = TargetType.ENEMY;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Decreases <sprite name=\"SPD YLW\"> by {CombatTools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, -8, 40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);
        //BattleSystem.Instance.SetStatChanges(Stat.SPD, -CombatTools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
