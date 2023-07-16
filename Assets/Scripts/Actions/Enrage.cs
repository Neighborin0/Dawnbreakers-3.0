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
        damageText = damage.ToString();
        cost = 25f;
        statAmount = 2;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
    }

    public override string GetDescription()
    {
        description = $"Raises <sprite name=\"ATK RED2\"> by {statAmount}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, statAmount, false, targets);
        yield return new WaitForSeconds(0.5f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
