using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class ControlBar : MonoBehaviour
{
    public GameObject DecisionControls;
    public GameObject DecisionControlsCancel;
    public GameObject WaitingControls;

    public void Update()
    {
        if (BattleSystem.Instance != null)
        {
            this.gameObject.SetActive(true);
            switch (BattleSystem.Instance.state)
            {
                case BattleStates.DECISION_PHASE:
                    DecisionControls.SetActive(true);
                    WaitingControls.SetActive(false);
                    break;
                case BattleStates.IDLE:
                    DecisionControls.SetActive(false);
                    WaitingControls.SetActive(true);
                    break;
            }
        }
    }
}
