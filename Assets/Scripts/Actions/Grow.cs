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
        damageText = damage.ToString();
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        description = "Raises <color=#FF0000>ATK</color><sprite name=\"ATK\"> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 2, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
