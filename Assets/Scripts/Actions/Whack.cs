using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Whack", menuName = "Assets/Actions/Whack")]
public class Whack : Action
{
    private void OnEnable()
    {
        ActionName = "Whack";


        cost = 40f;

        damage = 2;
        lightDamage = 0;
        heavyDamage = 5;


        cost = 30f;
        lightCost = 10f;
        heavyCost = 50f;

      

        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.STRIKE;
        damageText = damage.ToString();
        duration = 1;
        Done = false;
    }

    public override string GetDescription()   
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);
        //AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Strike", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 1f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "WarCry3", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 0.3f, 0, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "BasicHitParticles_001", Color.yellow, Color.yellow, new Vector3(0, 0, -1f), Quaternion.identity, 1, 0, true, 0, 10));
        yield return new WaitForSeconds(0.01f);
        Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(0.01f));
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        //BattleSystem.Instance.SetTempEffect(targets, "STAGGER", true, duration);
        yield return new WaitForSeconds(0.9f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
