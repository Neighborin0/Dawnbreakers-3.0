using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Hasten", menuName = "Assets/Actions/Hasten")]
public class Hasten : Action
{
    private void OnEnable()
    {
        ActionName = "Hasten";
        damage = 5;
        accuracy = 1;
        damageText = damage.ToString();
        cost = 50f;
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        description = "Increases <sprite name=\"SPD YLW\"> by 5.";
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.SPD, 5, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;

    }
}
