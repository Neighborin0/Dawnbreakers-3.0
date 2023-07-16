using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

[CreateAssetMenu(fileName = "Bash", menuName = "Assets/Actions/Bash")]
public class Bash : Action
{
    private void OnEnable()
    {
        ActionName = "Bash";
        damage = 5;
        damageText = damage.ToString();
        cost = 50f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
    }
    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + damage}</color> DMG.";
        }
        else
        {
            if(damage + unit.attackStat - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{damage + unit.attackStat - targets.defenseStat}</color> DMG.";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG."; 
        }
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);     
        AudioManager.Instance.Play("slash_001");
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash" ,Color.yellow, new Vector3(0, 0, -2f)));
        targets.health.TakeDamage(damage + unit.attackStat, unit);
        LabCamera.Instance.Shake(0.2f, 0.7f);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

  
}
