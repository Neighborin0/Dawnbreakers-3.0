using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BlazingStrike", menuName = "Assets/Actions/BlazingStrike")]
public class BlazingStrike : Action
{
     private void OnEnable()
    {
        ActionName = "Blazing Strike";

        damage = 6;
        lightDamage = 4;
        heavyDamage = 8;

        cost = 65f;
        heavyCost = 80f;
        lightCost = 40;

        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.HEAT;
        damageText = damage.ToString();

        Done = false;
    }

    public override string GetDescription()
    {
        string damageTypeSprite =
            Tools.ReturnDamageTypeSprite(damageType);

        if (unit == null)
        {
            description =
                $"Deals {damageTypeSprite} DMG.";

            return description;
        }

        int baseDamage =
            CombatTools.DetermineTrueActionValue(this) +
            unit.attackStat;

        float typeMultiplier =
            targets != null
                ? CombatTools.ReturnTypeMultiplier(
                    targets,
                    damageType
                )
                : 1f;

        int finalDamage =
            Mathf.Max(
                0,
                Mathf.RoundToInt(
                    baseDamage * typeMultiplier
                )
            );

        bool showDamageNumber =
            unit.IsPlayerControlled ||
            finalDamage > 0;

        if (showDamageNumber)
        {
            description =
                $"Deals <color=#FF0000>{finalDamage}</color> " +
                $"{damageTypeSprite} DMG.";
        }
        else
        {
            description =
                $"Deals {damageTypeSprite} DMG.";
        }

        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);  
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Slash", Color.red, Color.red, new Vector3(0, 0, -2f), Quaternion.identity, 1f, 0, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "WarCry3", Color.red, Color.red, new Vector3(0, 0, -2f), Quaternion.identity, 1f, 0, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "BasicHitParticles_001", Color.red, Color.red, new Vector3(0, 0, -1f), Quaternion.identity, 1f, 0, true, 0, 10));
        yield return new WaitForSeconds(0.01f);
        LabCamera.Instance.Shake(0.4f, 1.5f);
        Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(0.01f));
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);  
        yield return new WaitForSeconds(0.9f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
