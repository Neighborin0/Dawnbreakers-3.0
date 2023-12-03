using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.ProBuilder.Shapes;


public class IntentContainer : MonoBehaviour
{
    public Action action;
    public TextMeshProUGUI textMesh;
    public GameObject damageParent;
    public TextMeshProUGUI damageNums;
    public TextMeshProUGUI costNums;
    public Unit unit;


    public void DisplayIntentInfo()
    {
        if (BattleSystem.Instance.state != BattleStates.BATTLE)
        {
            action.targets.IsHighlighted = true;
            action.unit.timelinechild.HighlightedIsBeingOverwritten = true;
            this.unit.timelinechild.HighlightedIsBeingOverwritten = true;
            this.unit.timelinechild.Shift(this.unit);
            var newAction = Instantiate(action);
            newAction.unit = unit;
            BattleLog.Instance.DoBattleText($"{action.ActionName}\n{newAction.GetDescription()}");
            BattleLog.Instance.DisplayEnemyIntentInfo($"{action.ActionName}\n{newAction.GetDescription()}", unit);
        }
    }
    void Update()
    {
        if (textMesh != null && damageNums.IsActive() && unit != null && action != null && action.targets != null)
        {
            if ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType)) - action.targets.defenseStat > 0)
            {
                damageNums.text = "<sprite name=\"ATK\">" + ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType)) - action.targets.defenseStat).ToString();
                action.damageText = ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType)) - action.targets.defenseStat).ToString();
            }
            else
                damageNums.text = "<sprite name=\"ATK\">" + "0";
        }
      
    }
    public void RemoveDescription()
    {
        action.targets.IsHighlighted = false;
        this.unit.timelinechild.Return();
        this.unit.timelinechild.HighlightedIsBeingOverwritten = false;
        BattleLog.Instance.itemText.gameObject.SetActive(false);
    }

    public void CheckTarget(Action action, Unit unit)
    {
        if(action.targets == null)
        {
            switch (action.targetType)
            {
                case Action.TargetType.ENEMY:
                    action.targets = BattleSystem.Instance.numOfUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.numOfUnits.Count)];
                    break;
                case Action.TargetType.SELF:
                    action.targets = unit;
                    break;
                case Action.TargetType.ALL_ENEMIES:
                    action.targets = unit;
                    break;
                case Action.TargetType.ALLY:
                    action.targets = unit;
                    break;
            }
        }
    }


}
