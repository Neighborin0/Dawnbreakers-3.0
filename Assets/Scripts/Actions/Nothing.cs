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
        cost = 0f;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
        targetType = TargetType.SELF;
        description = "Intent is unknown...";
    }


    
}
