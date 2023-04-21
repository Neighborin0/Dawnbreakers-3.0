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
        accuracy = 1;
        cost = 30f;
        targetType = TargetType.ANY;
        actionType = ActionType.STATUS;
        description = "Decreases <color=#0000FF>DEF</color> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.SetStatChanges(Stat.DEF, -2, false, targets);
        this.Done = true;
        yield break;
    }
}
