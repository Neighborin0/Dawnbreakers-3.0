using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Incinerate", menuName = "Assets/Actions/Incinerate")]
public class Incinerate : Action
{
    private void OnEnable()
    {
        ActionName = "Incinerate";
        damage = 50;
        cost = 50f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
    }
    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + damage}</color> DMG.\nIgnores <sprite name=\"DEF BLUE\">";
        }
        else
        {
            if(damage + unit.attackStat > 0)
            {
                description = $"Deals <color=#FF0000>{damage + unit.attackStat}</color> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG."; 
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -50, 0.5f);
        yield return new WaitForSeconds(0.3f);
        var Light = targets.spotLight;
        Light.color = new Color(191, 21, 0);
        targets.ChangeUnitsLight(Light, 150, 15, 0.04f, 0.1f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "IncinerateParticles2", new Color(191, 21, 0), new Color(191, 21, 0), new Vector3(0, 0, -2f), 2.5f, 0, true, 0));
        yield return new WaitForSeconds(0.1f);
        targets.health.TakeDamage(damage + unit.attackStat, unit, true);
        LabCamera.Instance.Shake(1f, 1.3f);
        yield return new WaitForSeconds(0.5f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        Tools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
