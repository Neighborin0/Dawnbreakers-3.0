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
        cost = 50f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        damageText = damage.ToString();
    }

    public override string GetDescription()   
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + damage}</color> DMG.";
        }
        else
        {
            if (damage + unit.attackStat - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{damage + unit.attackStat - targets.defenseStat}</color> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, Color.yellow ,new Vector3(0, 0, -2f), 1f));
        yield return new WaitForSeconds(0.01f);
        targets.health.TakeDamage(damage + unit.attackStat, unit);
        LabCamera.Instance.Shake(0.2f, 1.5f);
        yield return new WaitForSeconds(0.5f);
        Tools.CheckIfActionWasFatalAndResetCam(this, targets.currentHP);
    }

  
}
