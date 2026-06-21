using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Bind", menuName = "Assets/Actions/Bind")]
public class Bind : Action
{
    private void OnEnable()
    {
        ActionName = "Bind";


        cost = 40f;

        damage = 2;
        lightDamage = 0;
        heavyDamage = 5;

        lightStatAmount = 10;
        statAmount = 15;
        heavyStatAmount = 20;

        cost = 50f;
        lightCost = 35f;
        heavyCost = 75f;



      

        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.DARK;
        damageText = damage.ToString();
        Done = false;
    }

    public override string GetDescription()
    {
        int timelineDelay = GetTimelineDelay();

        int displayedDamage = (int)(
            (CombatTools.DetermineTrueActionValue(this) + unit.attackStat) *
            CombatTools.ReturnTypeMultiplier(targets, damageType)
        );

        displayedDamage = Mathf.Max(displayedDamage, 0);

        description =
            $"Deals <color=#FF0000>{displayedDamage}</color> " +
            $"<sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG. " +
            $"<color=#FFFFFF>+{timelineDelay} Timeline Delay</color>.";

        return description;
    }

    private int GetTimelineDelay()
    {
        switch (actionStyle)
        {
            case ActionStyle.LIGHT:
                return lightStatAmount;

            case ActionStyle.HEAVY:
                return heavyStatAmount;

            case ActionStyle.STANDARD:
            default:
                return statAmount;
        }
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Strike", new Color(48, 25, 52), new Color(48, 25, 52), new Vector3(0, 0, -2f), Quaternion.identity, 1f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "WarCry3", new Color(48, 25, 52), new Color(48, 25, 52), new Vector3(0, 0, -2f), Quaternion.identity, 0.3f, 0, true, 0, 10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "BasicHitParticles_001", new Color(48, 25, 52), new Color(48, 25, 52), new Vector3(0, 0, -1f), Quaternion.identity, 1, 0, true, 0, 10));
        yield return new WaitForSeconds(0.01f);
        Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(0.01f));
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle, false, false, statAmount);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        yield return new WaitForSeconds(0.9f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
