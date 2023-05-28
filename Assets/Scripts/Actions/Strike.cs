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
        accuracy = 1;
        damageText = damage.ToString();
        cost = 50f;
        actionType = ActionType.ATTACK;
        targetType = TargetType.ANY;
        description = $"Deals <color=#FF0000>{damageText}</color> damage.";


    }
    public override IEnumerator ExecuteAction()
    {
        if (targets != null)
        {
            unit.PlayAction("Attack", unit);
            yield return new WaitUntil(() => unit.Execute);
            LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false);
            yield return new WaitForSeconds(0.3f);
            AudioManager.Instance.Play("strike_001");
            BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Strike", Color.yellow, new Vector3(0, 0, -2f), 1f));
            targets.health.TakeDamage(damage + unit.attackStat);
            LabCamera.Instance.Shake(0.25f, 1f);
            yield return new WaitForSeconds(0.5f);
            LabCamera.Instance.ResetPosition();
            this.Done = true;
        }
    }

}
