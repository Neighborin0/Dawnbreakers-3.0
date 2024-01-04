using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Shriek", menuName = "Assets/Actions/Shriek")]
public class Shriek : Action
{
    private void OnEnable()
    {
        ActionName = "Shriek";

        damage = 10;
        lightDamage = 7;
        damage = 13;


        cost = 0f;
        lightCost = 0f;
        heavyCost = 20f;


        targetType = TargetType.ALL_ENEMIES;
        damageType = DamageType.DARK;
        damageText = damage.ToString();
        actionType = ActionType.ATTACK;

        AppliesStun = true;
        Done = false;
    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG to ALL enemies.\n Inflicts <sprite name=\"STUN\">.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG to ALL enemies.\n Inflicts <sprite name=\"STUN\">.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG to ALL enemies.\n Inflicts <sprite name=\"STUN\">.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCry", new Color(1, 0, 0, 0.1f), new Color(1, 0, 0, 0.1f), new Vector3(-0.4f, 0, -6f), Quaternion.identity, 5f));
        LabCamera.Instance.Shake(0.3f, 1.5f);
        yield return new WaitForSeconds(0.5f);
        foreach (var x in CombatTools.DetermineEnemies(unit))
        {
            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(x.gameObject, "Strike", new Color(156, 14, 207), new Color(156, 14, 207), new Vector3(0, 0, -2f), Quaternion.identity));
            x.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle, false, true);
        }
        yield return new WaitForSeconds(0.5f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

}
