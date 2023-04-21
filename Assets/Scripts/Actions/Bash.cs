using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bash", menuName = "Assets/Actions/Bash")]
public class Bash : Action
{
    private void OnEnable()
    {
        ActionName = "Bash";
        damage = 20;
        accuracy = 1;
        cost = 50f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        description = $"Deals 20 base damage.";
    }

    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.Shake(0.2f, 0.7f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash" ,Color.yellow, new Vector3(0, 0, -2f)));
        targets.health.TakeDamage(damage + unit.attackStat);
        this.Done = true;
    }

  
}
