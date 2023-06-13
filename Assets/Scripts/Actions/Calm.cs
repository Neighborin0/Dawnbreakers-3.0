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
        accuracy = 1;
        damageText = damage.ToString();
        cost = 50f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "Increases <sprite name=\"DEF BLUE\"> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, 2, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
