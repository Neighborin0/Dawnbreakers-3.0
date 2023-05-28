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
        damageText = damage.ToString();
        cost = 25f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "Raises <color=#FF0000>ATK</color><sprite name=\"ATK RED2\"> by 2.";
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 2, false, targets);
        yield return new WaitForSeconds(0.5f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
