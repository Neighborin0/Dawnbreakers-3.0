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
        cost = 100f;
        targetType = TargetType.ALL_ENEMIES;
        damageText = damage.ToString();
        actionType = ActionType.ATTACK;
    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + damage}</color> DMG to ALL enemies..";
        }
        else
        {
            if (damage + unit.attackStat - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{damage + unit.attackStat - targets.defenseStat}</color> DMG to ALL enemies.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG to ALL enemies.";
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        foreach (var x in Tools.DetermineEnemies(unit))
        {
            AudioManager.Instance.Play("slash_001");
            BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(x.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f)));
            x.health.TakeDamage(damage + unit.attackStat, unit);
        }
        Director.Instance.StartCoroutine(Tools.StopTime(0.13f));
        this.Done = true;
    }

  
}
