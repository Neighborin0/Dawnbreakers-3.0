using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EffectIcon : MonoBehaviour
{
    public Image icon;
    public Unit owner;
    public TextMeshProUGUI timerText;
    public Action action;
    public bool isPaused;
    public bool ForceEnd = false;
    public string description;
    public string iconName;

    public void OnDestroy()
    {
        if (action != null)
            action.OnEnded(owner);
    }

    public void DisplayDescription()
    {
        BattleLog.SetBattleText("");
        BattleLog.SetBattleText($"{iconName}\n{description}");
    }

    public void RemoveDescription()
    {
        BattleSystem.Instance.ResetBattleLog();
    }
}
