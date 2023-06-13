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
        damageText = damage.ToString();
        description = $"Deals <color=#FF0000>{damageText}</color> DMG.";
    }

    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f), 1f));
        yield return new WaitForSeconds(0.01f);
        targets.health.TakeDamage(damage + unit.attackStat);
        LabCamera.Instance.Shake(0.3f, 1.5f);
        yield return new WaitForSeconds(0.5f);
        Done = true;
        LabCamera.Instance.ResetPosition();
    }

  
}
