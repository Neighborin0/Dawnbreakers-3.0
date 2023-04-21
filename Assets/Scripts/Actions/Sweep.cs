using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Sweep", menuName = "Assets/Actions/Sweep")]
public class Sweep : Action
{
    private void OnEnable()
    {
        ActionName = "Sweep";
        damage = 2;
        cost = 25f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        description = $"Deals 2 base damage.";
    }

    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f)));
        AudioManager.Instance.Play("slash_001");
        Director.Instance.StartCoroutine(Tools.StopTime(0.1f));
        LabCamera.Instance.Shake(0.2f, 0.7f);
        targets.health.TakeDamage(damage + unit.attackStat);
        this.Done = true;
    }

  
}
