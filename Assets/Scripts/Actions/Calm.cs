using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Calm", menuName = "Assets/Actions/Calm")]
public class Calm : Action
{
    private void OnEnable()
    {
        ActionName = "Fortify";
        damage = 3;
        damageText = damage.ToString();
        cost = 50f;
        statAmount = 2;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
    }

    public override string GetDescription()
    {
        description = $"Increases <sprite name=\"DEF BLUE\"> by {Tools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, -8, 40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, Tools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
