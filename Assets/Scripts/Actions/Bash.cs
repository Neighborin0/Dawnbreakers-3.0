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
        damageText = damage.ToString();
        accuracy = 1;
        cost = 50f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        description = $"Deals  <color=#FF0000>{damageText}</color> damage.";
    }

    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false);
        yield return new WaitForSeconds(0.3f);     
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash" ,Color.yellow, new Vector3(0, 0, -2f)));
        targets.health.TakeDamage(damage + unit.attackStat);
        LabCamera.Instance.Shake(0.2f, 0.7f);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

  
}
