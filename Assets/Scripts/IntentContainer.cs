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
            LabCamera.Instance.MoveToUnit(unit);
            action.targets.IsHighlighted = true;
            var newAction = Instantiate(action);
            newAction.unit = unit;
            newAction.damage += newAction.unit.attackStat;
            BattleLog.SetBattleText(newAction.description);
            BattleLog.DisplayEnemyIntentInfo(newAction.description, unit);
        }
    }

    public void DisplayIntentInfoWithoutCameraMoveYeaThisVariableNameIsKindaLongButWhatever()
    {
        if (BattleSystem.Instance.state != BattleStates.BATTLE)
        {
            //LabCamera.Instance.MoveToUnit(unit);
            AudioManager.Instance.Play("ButtonHover");
            action.targets.IsHighlighted = true;
            var newAction = Instantiate(action);
            newAction.damage += newAction.unit.attackStat;
            BattleLog.SetBattleText(newAction.description);
            BattleLog.DisplayEnemyIntentInfo(newAction.description, unit);
        }
    }

    void Update()
    {
        if (textMesh != null && damageNums.IsActive() && unit != null && action != null && action.targets != null)
        {
            if (action.damage + unit.attackStat - action.targets.defenseStat > 0)
            {
                damageNums.text = "<sprite name=\"ATK\">" + (action.damage + unit.attackStat - action.targets.defenseStat).ToString();
                action.damageText = (unit.attackStat + action.damage).ToString();
            }
            else
                damageNums.text = "<sprite name=\"ATK\">" + "0";
        }
      
    }
    public void RemoveDescription()
    {
        action.targets.IsHighlighted = false;
        BattleLog.Instance.itemText.gameObject.SetActive(false);
    }


}
