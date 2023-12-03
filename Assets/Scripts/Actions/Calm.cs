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
        damageText = damage.ToString();

        statAmount = 2;
        lightStatAmount = 1;
        lightStatAmount = 3;



        cost = 20f;
        lightCost = 0f;
        heavyCost = 40;
       
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
    }

    public override string GetDescription()
    {
        description = $"Increases <sprite name=\"DEF BLUE\"> by {CombatTools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, -8, 40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, CombatTools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
