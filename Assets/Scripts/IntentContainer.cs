﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class IntentContainer : MonoBehaviour
{
    public Action action;
    public TextMeshProUGUI textMesh;
    public TextMeshProUGUI damageNums;
    public TextMeshProUGUI costNums;
    public GameObject damageParent;
    public Unit unit;


    public void DisplayIntentInfo()
    {
        if (BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
        {
            LabCamera.Instance.MoveToUnit(unit);
            action.targets.IsHighlighted = true;
            BattleLog.DisplayEnemyIntentInfo(action.targets.unitName, action.description);
        }
    }

    public void DisplayIntentInfoWithoutCameraMoveYeaThisVariableNameIsKindaLongButWhatever()
    {
        if (BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
        {
            //LabCamera.Instance.MoveToUnit(unit);
            AudioManager.Instance.Play("ButtonHover");
            action.targets.IsHighlighted = true;
            BattleLog.DisplayEnemyIntentInfo(action.targets.unitName, action.description);
        }
    }

    void Update()
    {
        if (textMesh != null && damageNums.IsActive() && unit != null && action != null && action.targets != null)
        {
            if (action.damage + unit.attackStat - action.targets.defenseStat > 0)
                damageNums.text = (action.damage + unit.attackStat - action.targets.defenseStat).ToString();
            else
                damageNums.text = "0";
        }
    }
    public void RemoveDescription()
    {
        action.targets.IsHighlighted = false;
        BattleSystem.Instance.ResetBattleLog();
    }


}