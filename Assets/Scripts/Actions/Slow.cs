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
        actionType = ActionType.STATUS;
        description = "Decreases <color=#FFEA29>SPD</color> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.SetStatChanges(Stat.SPD, -2, false, targets);
        this.Done = true;
        yield break;
    }
}
