using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Growl", menuName = "Assets/Actions/Growl")]
public class Growl : Action
{
    private void OnEnable()
    {
        ActionName = "Growl";
        damage = 2;
        cost = 30f;
        damageText = damage.ToString();
        targetType = TargetType.ANY;
        actionType = ActionType.STATUS;
        statAmount = 2;
    }

    public override string GetDescription()
    {
        description = $"Decreases <sprite name=\"DEF BLUE2\"> by {statAmount}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, -statAmount, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
