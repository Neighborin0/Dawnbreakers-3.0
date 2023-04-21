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
        accuracy = 1;
        cost = 45f;
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        description = "Raises <color=#FF0000>ATK</color> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 2, false, targets);
        this.Done = true;
        yield break;
    }
}
