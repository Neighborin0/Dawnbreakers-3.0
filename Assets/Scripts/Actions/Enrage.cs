using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enrage", menuName = "Assets/Actions/Enrage")]
public class Enrage : Action
{
    private void OnEnable()
    {
        ActionName = "Enrage";
        damage = 3;
        accuracy = 1;
        cost = 25f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "Raises <color=#FF0000>ATK</color> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 2, false, targets);
        this.Done = true;
        yield break;
    }
}
