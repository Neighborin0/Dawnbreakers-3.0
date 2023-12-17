using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Slam", menuName = "Assets/Actions/Slam")]
public class Slam : Action
{
    private void OnEnable()
    {
        ActionName = "Slam";

        damage = 10;
        lightDamage = 7;
        damage = 13;


        cost = 75f;
        lightCost = 55f;
        heavyCost = 95;


        targetType = TargetType.ALL_ENEMIES;
        damageType = DamageType.STRIKE;
        damageText = damage.ToString();
        actionType = ActionType.ATTACK;
    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG to ALL enemies..";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG to ALL enemies.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG to ALL enemies.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        unit.PlayUnitAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        foreach (var x in CombatTools.DetermineEnemies(unit))
        {
            AudioManager.Instance.Play("slash_001");
            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(x.gameObject, "Slash", Color.yellow, Color.white, new Vector3(0, 0, -2f), Quaternion.identity));
            x.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType, actionStyle);
        }
        Director.Instance.StartCoroutine(Tools.StopTime(0.13f));
        this.Done = true;
    }

  
}
