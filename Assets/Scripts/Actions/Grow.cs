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


        cost = 45f;
        lightCost = 25f;
        heavyCost = 65f;


        statAmount = 2;
        lightStatAmount = 1;
        heavyStatAmount = 3;



        damageText = damage.ToString();
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Raises <sprite name=\"ATK RED2\"> by {CombatTools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, -8, 40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, CombatTools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
