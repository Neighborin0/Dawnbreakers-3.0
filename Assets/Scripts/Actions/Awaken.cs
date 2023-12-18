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
        lightCost = 80f;
        heavyCost = 100f;


        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "Applies +12 <sprite name=\"FORTIFY\"> to self.";
    }

    public override string GetDescription()
    {
        description = "Applies +12 <sprite name=\"FORTIFY\"> to self.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.Shake(1f, 0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ARMOR, 12f, false, targets);
        targets.DoesntLoseArmorAtStartOfRound = true;
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        yield return new WaitForSeconds(0.5f);
        this.Done = true;
    }

}
