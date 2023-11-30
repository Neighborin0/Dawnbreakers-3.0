using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Strike", menuName = "Assets/Actions/Strike")]
public class Strike : Action
{
    private void OnEnable()
    {
        ActionName = "Strike";
        damage = 5;
        damageText = damage.ToString();
        cost = 50f;
        actionType = ActionType.ATTACK;
        targetType = TargetType.ENEMY;


    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + Tools.DetermineTrueActionValue(this)}</color> DMG.";
        }
        else
        {
            if (Tools.DetermineTrueActionValue(this) + unit.attackStat - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{Tools.DetermineTrueActionValue(this) + unit.attackStat - targets.defenseStat}</color> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        if (targets != null)
        {
            unit.PlayUnitAction("Attack", unit);
            yield return new WaitUntil(() => unit.Execute);
            LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
            yield return new WaitForSeconds(0.3f);
            AudioManager.Instance.Play("strike_001");
            BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Strike", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), 1f));
            targets.health.TakeDamage(Tools.DetermineTrueActionValue(this) + unit.attackStat, unit);
            LabCamera.Instance.Shake(0.3f, 1.5f);
            yield return new WaitForSeconds(0.5f);
            Tools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
        }
    }

}
