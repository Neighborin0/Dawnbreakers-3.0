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
        accuracy = 1;
        cost = 75f;
        targetType = TargetType.ANY;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        description = "Decreases <sprite name=\"SPD YLW\"> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.SPD, -2, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
