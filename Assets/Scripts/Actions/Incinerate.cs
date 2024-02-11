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
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
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
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(10));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        var Light = targets.spotLight;
        Light.color = new Color(255, 74, 0) * 0.01f;
        Light.intensity = 10f;
        AudioManager.QuickPlay("incinerate_buildup_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateParticles", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, 0), Quaternion.identity, 5f, 0, false, 8, 10, 0.0001f, "incinerate_cackle_001"));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateTornado", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, -0), new Quaternion(0, -180, 0, 0), 8f, 0, true, 6, 10, 0.0001f)); 
        yield return new WaitForSeconds(0.8f);
        AudioManager.QuickPlay("incinerate_001");
        yield return new WaitForSeconds(0.7f);
        LabCamera.Instance.Shake(3.2f, 1f);
        yield return new WaitForSeconds(2.4f);
        Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "incinerate_001", 0.6f, true));
        AudioManager.QuickPlay("fire_impact_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "FireImpact", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, 0, -2f), Quaternion.identity, 1f, 0, true, 0, 10));
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle, true);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
        targets.ChangeUnitsLight(Light, 0, 15, new Color(255, 74, 0) * 0.01f, 0.04f, 0.001f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }


}
