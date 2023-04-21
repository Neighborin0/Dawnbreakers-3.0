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
        cost = 50f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "Increases  <color=#0000FF>DEF</color> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.SetStatChanges(Stat.DEF, 2, false, targets);
        this.Done = true;
        yield break;
    }
}
