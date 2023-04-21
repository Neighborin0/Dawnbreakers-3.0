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
        cost = 50f;
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        description = "Increases <color=#FFEA29>SPD</color> by 5";
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.SetStatChanges(Stat.SPD, 5, false, targets);
        this.Done = true;
        yield break;

    }
}
