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
        accuracy = 1;
        cost = 50f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        description = $"Deals 5 base damage.";
    }

    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 7.8f, 0, 0);
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.Shake(0.2f, 0.7f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f), 1f));
        targets.health.TakeDamage(damage + unit.attackStat);
        Done = true;
        LabCamera.Instance.ResetPosition();
    }

  
}
