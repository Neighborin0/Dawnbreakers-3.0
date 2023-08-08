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
        cost = 30f;
        damageText = damage.ToString();
        targetType = TargetType.ANY;
        actionType = ActionType.STATUS;
        statAmount = 2;
    }

    public override string GetDescription()
    {
        description = $"Decreases <sprite name=\"DEF BLUE2\"> by {statAmount}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, 0, 8, -50, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", new Color(0, 0, 1, 0.1f), new Color(0, 0, 1, 0.1f),  new Vector3(0, 0, -2f), 0.2f));
        yield return new WaitForSeconds(0.1f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", new Color(0, 0, 1, 0.1f), new Color(0, 0, 1, 0.1f), new Vector3(0, 0, -2f), 0.2f));
        yield return new WaitForSeconds(0.4f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, -statAmount, false, targets);
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
