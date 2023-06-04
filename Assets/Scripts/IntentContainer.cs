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
            BattleLog.DisplayEnemyIntentInfo(action.description);
        }
    }

    public void DisplayIntentInfoWithoutCameraMoveYeaThisVariableNameIsKindaLongButWhatever()
    {
        if (BattleSystem.Instance.state != BattleStates.BATTLE)
        {
            //LabCamera.Instance.MoveToUnit(unit);
            AudioManager.Instance.Play("ButtonHover");
            action.targets.IsHighlighted = true;
            BattleLog.DisplayEnemyIntentInfo(action.description);
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
    }


}
