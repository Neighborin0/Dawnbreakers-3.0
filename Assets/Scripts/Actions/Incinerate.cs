using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Incinerate", menuName = "Assets/Actions/Incinerate")]
public class Incinerate : Action
{
    private void OnEnable()
    {
        ActionName = "Incinerate";

        damage = 50;
        lightDamage = 50;
        heavyDamage = 50;



        cost = 75f;
        lightCost = 80;
        heavyCost = 100;


        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.HEAT;

        Done = false;
    }
    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color>   <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"DEF BLUE\">.";
        }
        else
        {
            if((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color>   <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"DEF BLUE\">.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"DEF BLUE\">."; 
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        var Light = targets.spotLight;
        Light.color = new Color(255, 74, 0) * 0.01f;
        Light.intensity = 10f;
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateParticles", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, 0), Quaternion.identity, 2f, 0, false, 6));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateTornado", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, -0),new Quaternion(0, -90, 0, 0), 8f, 0, true, 6));
        yield return new WaitForSeconds(1.5f);
        LabCamera.Instance.Shake(3.2f, 1f);
        yield return new WaitForSeconds(3f);
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle, true);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        targets.ChangeUnitsLight(Light, 0, 15, new Color(255, 74, 0) * 0.01f, 0.04f, 0.001f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
