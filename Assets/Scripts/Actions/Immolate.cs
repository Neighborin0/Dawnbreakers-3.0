﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Immolate", menuName = "Assets/Actions/Immolate")]
public class Immolate : Action
{
    private void OnEnable()
    {
        ActionName = "Immolate";

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
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color>   <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"FORTIFY\">.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color>   <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"FORTIFY\">.";
            }
            else
                description = $"Deals <color=#FF0000>0</color>   <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nIgnores <sprite name=\"FORTIFY\">.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(10));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, -3, 10, -50, 0.5f, false, true);
        yield return new WaitForSeconds(0.1f);
        LabCamera.Instance.DoSlowHorizontalSweep(unit.IsPlayerControlled, 0.0001f, -0.01f);
     
        yield return new WaitForSeconds(0.3f);
        var Light = targets.spotLight;
        Light.color = new Color(255, 74, 0) * 0.01f;
        Light.intensity = 10f;
        AudioManager.QuickPlay("incinerate_buildup_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "ImmolateParticles_002", new Color(255, 91, 0), new Color(255, 91, 0), new Vector3(0, 0, 0), Quaternion.identity, 1.8f, 0, false, 8, 10, 0.0001f, "incinerate_cackle_001"));
       // BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateParticles", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, 0), Quaternion.identity, 5f, 0, false, 8, 10, 0.0001f, "incinerate_cackle_001"));
       // BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateParticles_002", new Color(255, 74, 0), new Color(255, 100, 0), new Vector3(0, -3, 0), Quaternion.identity, 10, 0, false, 8, 10, 0.0001f, "incinerate_cackle_001"));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IgniteSmoke", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, 0), Quaternion.identity, 2, 0, false, 2.5f, 0.01f, 0.0001f, "incinerate_cackle_001"));
        //BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "IncinerateTornado_002", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -9, -0), new Quaternion(0, 90, 0, 90), 8f, 0, true, 6, 10, 0.0001f)); 
        yield return new WaitForSeconds(2f);
        Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "incinerate_001", 0.6f, true));
        AudioManager.QuickPlay("fire_impact_001");
        targets.ChangeUnitsLight(Light, 100, 15, Light.color, 0.001f);
        //BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "SmokeBurst_003", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, -3, 0), Quaternion.identity, 0, 0.8f, false, 8, 0.01f, 0.0001f, "incinerate_cackle_001"));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "ImmolateHit_001", new Color(255, 140, 0), new Color(255, 74, 0), new Vector3(0, 0, -2f), Quaternion.identity, 3f, 5, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "FireImpact", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, 0, -2f), Quaternion.identity, 1f, 5, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "BasicHitParticles_001", new Color(255, 74, 0), new Color(255, 74, 0), new Vector3(0, 0, -1f), Quaternion.identity, 1f, 0, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "WarCry3", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 1f, 0, true, 0, 10));
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle, true);
        Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(0.01f));
        yield return new WaitForSeconds(0.01f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
        targets.ChangeUnitsLight(Light, 0, 15, new Color(255, 74, 0) * 0.01f, 0.04f, 0.001f);
        yield return new WaitForSeconds(2f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }


}
