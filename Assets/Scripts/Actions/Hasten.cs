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
        damageText = damage.ToString();
        cost = 50f;
        targetType = TargetType.SELF;
        statAmount = 5;
        actionType = ActionType.STATUS;
    }

    public override string GetDescription()
    {
        description = $"Increases <sprite name=\"SPD YLW\"> by {Tools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, -8, 40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.SPD, Tools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;

    }
}
