using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Expose", menuName = "Assets/Actions/Expose")]
public class Expose : Action
{
    private void OnEnable()
    {
        ActionName = "Expose";

        cost = 20f;
        lightCost = 0;
        heavyCost = 40f;

        targetType = TargetType.ENEMY;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
        duration = 2;
        CanBeStyled = false;


        Done = false;
    }

    public override string GetDescription()   
    {
        description = $"Applies <color=#FFFFFF>EXPOSE</color> for {duration} round";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "StaggerParticles", Color.white, Color.white, new Vector3(0, 0, -2f), Quaternion.identity, 1f));
        BattleSystem.Instance.SetTempEffect(targets, "EXPOSE", true, duration);
        yield return new WaitForSeconds(0.5f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
