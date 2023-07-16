using System;
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
        targetType = TargetType.ANY;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
    }

    public override string GetDescription()
    {
        description = $"Decreases <sprite name=\"SPD YLW\"> by {statAmount}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.SPD, -statAmount, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
