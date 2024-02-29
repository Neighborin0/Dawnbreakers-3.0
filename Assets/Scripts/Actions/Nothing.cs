using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Nothing", menuName = "Assets/Actions/Nothing")]
public class Nothing : Action
{
    private void OnEnable()
    {
        ActionName = "...";
        cost = 60f;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
        targetType = TargetType.SELF;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Intent is unknown...";
        return description;
    }

    public override IEnumerator ExecuteAction()
    {
        this.Done = true;
        yield break;
    }
}
