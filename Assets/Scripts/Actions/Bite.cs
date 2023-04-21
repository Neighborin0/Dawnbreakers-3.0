using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bite", menuName = "Assets/Actions/Bite")]
public class Bite : Action
{
    private void OnEnable()
    {
        ActionName = "Strike";
        damage = 5;
        accuracy = 1;
        cost = 50f;
        actionType = ActionType.ATTACK;
        targetType = TargetType.ANY;
        description = $"Deals {(unit != null ? unit.attackStat + damage : damage)} damage";


    }
    public override IEnumerator ExecuteAction()
    {
        if (targets != null)
        {
            LabCamera.Instance.MoveToUnit(targets, 7.8f, 0, 0);
            unit.PlayAction("Attack", unit);
            yield return new WaitUntil(() => unit.Execute);
            LabCamera.Instance.Shake(0.2f, 0.7f);
            AudioManager.Instance.Play("strike_001");
            BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Strike", Color.yellow, new Vector3(0, 0, -2f), .2f));
            targets.health.TakeDamage(damage + unit.attackStat);
            LabCamera.Instance.ResetPosition();
            this.Done = true;
        }
    }

}
