using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Awaken", menuName = "Assets/Actions/Awaken")]
public class Awaken : Action
{
    private void OnEnable()
    {
        ActionName = "Awaken";
        cost = 100f;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "Massively raises both <color=#0000FF>DEF</color> <sprite name=\"DEF BLUE\"> and <color=#FFEA29>SPD</color>  <sprite name=\"SPD YLW\">.";
    }

    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false);
        yield return new WaitForSeconds(0.3f);
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        var Light = targets.spotLight;
        Light.color = Color.blue;
        targets.ChangeUnitsLight(Light, 150, 15, 0.04f, 1.6f);
        LabCamera.Instance.Shake(1f, 0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, 2f, false, targets);
        yield return new WaitForSeconds(1f);
        Light.color = Color.yellow;
        LabCamera.Instance.Shake(1f, 0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.SPD, 15f, false, targets);
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

}
