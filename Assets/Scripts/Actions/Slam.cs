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
        description = $"Deals <color=#FF0000>{damageText}</color> damage to ALL enemies.";
    }

    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.Shake(0.2f, 0.7f);
        foreach (var x in Tools.DetermineEnemies(unit))
        {
            AudioManager.Instance.Play("slash_001");
            BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(x.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f)));
            x.health.TakeDamage(damage + unit.attackStat);
        }
        Director.Instance.StartCoroutine(Tools.StopTime(0.13f));
        this.Done = true;
    }

  
}
