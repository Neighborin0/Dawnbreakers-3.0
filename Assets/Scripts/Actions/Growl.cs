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
        damageText = damage.ToString();
        targetType = TargetType.ANY;
        actionType = ActionType.STATUS;
        description = "Decreases <color=#0000FF>DEF</color>  <sprite name=\"DEF BLUE\">by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, -2, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
