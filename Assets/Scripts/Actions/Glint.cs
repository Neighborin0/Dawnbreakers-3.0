using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Glint", menuName = "Assets/Actions/Glint")]
public class Glint : Action
{
    private void OnEnable()
    {
        ActionName = "Push";
        cost = 25f;
        targetType = TargetType.ENEMY;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
        duration = 5f;
    }

    public override string GetDescription()   
    {
        description = $"Applies <sprite name=\"STAGGER\"> for {duration} seconds";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "StaggerParticles", new Color(156, 14, 207), new Color(156, 14, 207), new Vector3(0, 0, -2f), 1f));
        BattleSystem.Instance.SetTempEffect(targets, "STAGGER", true, duration);
        yield return new WaitForSeconds(0.5f);
        Tools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
