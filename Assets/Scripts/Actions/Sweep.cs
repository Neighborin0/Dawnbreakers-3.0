using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Sweep", menuName = "Assets/Actions/Sweep")]
public class Sweep : Action
{
    private void OnEnable()
    {
        ActionName = "Sweep";
        damage = -1;
        lightDamage = -3;
        heavyDamage = 2;

        cost = 25f;
        lightCost = 5;
        heavyCost = 40;


        targetType = TargetType.ENEMY;
        actionType = ActionType.ATTACK;
        damageType = DamageType.STRIKE;
        damageText = damage.ToString();

        Done = false;
    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color> " +
                $"<sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        else
        {
            if ((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType)) > 0)
            {
                description = $"Deals <color=#FF0000>{(int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(targets, damageType))}</color>  <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color>  <sprite name=\"{Tools.ReturnDamageTypeSpriteName(damageType)}\"> DMG.";
        }
        return description;
    }

    public override IEnumerator ExecuteAction()
    {
        int AdditionalDMG = 0;
        unit.PlayUnitAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.Play("slash_001");
        /*if(targets.statusEffects.Contains(targets.statusEffects.Where(obj => obj.iconName == "STAGGER").SingleOrDefault()))
        {
            var Stagger = targets.statusEffects.Where(obj => obj.iconName == "STAGGER").SingleOrDefault();
            Stagger.DestoryEffectIcon();
            AdditionalDMG += 2;
            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Color(156, 14, 207), new Vector3(0, 0, -2f),    Quaternion.identity, 1f));
        }
        else
        {
            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Slash", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 1f));
        }
        */
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Slash", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), Quaternion.identity, 1f));
        LabCamera.Instance.Shake(0.1f, 1.5f);
        targets.health.TakeDamage((int)((CombatTools.DetermineTrueActionValue(this) + unit.attackStat + AdditionalDMG) * CombatTools.ReturnTypeMultiplier(targets, damageType)), unit, damageType,actionStyle, false);
        yield return new WaitForSeconds(0.5f);
        CombatTools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
