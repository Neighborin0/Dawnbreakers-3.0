using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Slash", menuName = "Assets/Actions/Slash")]
public class Slash : Action
{
     private void OnEnable()
    {
        ActionName = "Slash";

        damage = 5;
        lightDamage = 3;
        heavyDamage = 8;

        cost = 40f;
        heavyCost = 60;
        lightCost = 20;

        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.SLASH;
        damageText = damage.ToString();

        Done = false;
    }

    public override string GetDescription()   
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))} </color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color><sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);  
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Slash", Color.yellow, Color.yellow ,new Vector3(0, 0, -2f), Quaternion.identity, 1f, 0, true, 0, 10));
        yield return new WaitForSeconds(0.01f);
        LabCamera.Instance.Shake(0.4f, 1.5f);
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);  
        yield return new WaitForSeconds(0.9f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
