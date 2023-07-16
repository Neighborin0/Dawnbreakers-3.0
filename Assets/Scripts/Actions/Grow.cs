using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "Grow", menuName = "Assets/Actions/Grow")]
public class Grow : Action
{
    private void OnEnable()
    {
        ActionName = "Empower";
        damage = 3;
        cost = 45f;
        statAmount = 2;
        damageText = damage.ToString();
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
    }

    public override string GetDescription()
    {
        description = $"Raises <sprite name=\"ATK RED2\"> by {statAmount}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, statAmount, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
