using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "ShieldBash", menuName = "Assets/Actions/ShieldBash")]
public class ShieldBash : Action
{
    private void OnEnable()
    {
        ActionName = "ShieldBash";

        damage = 0;
        lightDamage = -2;
        heavyDamage = 2;


        cost = 30f;
        lightCost = 10f;
        heavyCost = 50f;

        statAmount = 2;
        lightStatAmount = 1;
        heavyStatAmount = 3;


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
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nApplies +{statAmount + unit.defenseStat} <sprite name=\"FORTIFY\"> to self";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nApplies +{statAmount + unit.defenseStat} <sprite name=\"FORTIFY\"> to self";
            }
            else
                description = $"Deals <color=#FF0000>0</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.\nApplies +{statAmount + unit.defenseStat} <sprite name=\"FORTIFY\"> to self";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Strike", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 1f));
        yield return new WaitForSeconds(0.01f);
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        //BattleSystem.Instance.SetTempEffect(targets, "STAGGER", true, duration);
        yield return new WaitForSeconds(0.5f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
