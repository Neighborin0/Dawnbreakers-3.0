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
        damage = -1;
        cost = 30f;
        targetType = TargetType.ANY;
        actionType = ActionType.ATTACK;
        damageText = damage.ToString();
    }

    public override string GetDescription()
    {
        if (unit.IsPlayerControlled)
        {
            description = $"Deals <color=#FF0000>{unit.attackStat + damage}</color> DMG.\n<color=#FF0000>+2</color> DMG when <sprite name=\"STAGGER\">.";
        }
        else
        {
            if (damage + unit.attackStat - targets.defenseStat > 0)
            {
                description = $"Deals <color=#FF0000>{damage + unit.attackStat - targets.defenseStat}</color> DMG. Deals an additional <color=#FF0000>+2</color> when <sprite name=\"STAGGER\">";
            }
            else
                description = $"Deals <color=#FF0000>0</color> DMG. Deals an additional <color=#FF0000>+2</color> when <sprite name=\"STAGGER\">";
        }
        return description;
    }

    public override IEnumerator ExecuteAction()
    {
        int AdditionalDMG = 0;
        unit.PlayAction("Attack", unit);
        yield return new WaitUntil(() => unit.Execute);
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Slash", Color.yellow, new Vector3(0, 0, -2f), 1f));
        AudioManager.Instance.Play("slash_001");
        LabCamera.Instance.Shake(0.3f, 1f);
        foreach (Transform icon in targets.namePlate.IconGrid.transform)
        {
            var EI = icon.gameObject.GetComponent<EffectIcon>();
            if (EI.GetComponent<Stagger>() != null)
            {
                EI.DestoryEffectIcon();
                AdditionalDMG += 2;
            }
        }
        targets.health.TakeDamage(damage + AdditionalDMG + unit.attackStat, unit);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

  
}
