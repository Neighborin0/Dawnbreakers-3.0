using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Bash", menuName = "Assets/Actions/Bash")]
public class Bash : Action
{
    private void OnEnable()
    {
        ActionName = "Bash";
        damage = 5;
        lightDamage = 3;
        heavyDamage = 7;


        cost = 40f;
        lightCost = 20f;
        heavyCost = 60f;

        damageText = damage.ToString();


        targetType = TargetType.ENEMY;
        damageType = DamageType.STRIKE;
        actionType = ActionType.ATTACK;
    }
    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        else
        {
            if((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color><sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG."; 
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        unit.PlayUnitAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);     
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Strike" ,Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "SmokeBurst", Color.white, Color.white, new Vector3(0, 0, -2f), Quaternion.identity, 1, 0, false, 0, 2));
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);
        LabCamera.Instance.Shake(0.2f, 1.5f);
        yield return new WaitForSeconds(0.5f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
