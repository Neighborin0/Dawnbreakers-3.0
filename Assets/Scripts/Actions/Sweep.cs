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
        damage = 1;
        cost = 30f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        damageText = damage.ToString();
        description = $"Deals <color=#FF0000>{damageText}</color> DMG.";
    }

    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f), 1f));
        AudioManager.Instance.Play("slash_001");
        LabCamera.Instance.Shake(0.3f, 1f);
        targets.health.TakeDamage(damage + unit.attackStat);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

  
}
