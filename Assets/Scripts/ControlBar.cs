using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class ControlBar : MonoBehaviour
{
    public GameObject Select;
    public GameObject Cancel;
    public GameObject Continue;
    public GameObject Pause;

    public void Update()
    {
        //Battle Controls
        if (BattleSystem.Instance != null)
        {
            this.gameObject.SetActive(true);
            this.GetComponent<Image>().enabled = true;
            //Talking
            if (BattleSystem.Instance.state == BattleStates.TALKING)
            {
                Continue.SetActive(true);
                Select.SetActive(false);
                Cancel.SetActive(false);
                Pause.SetActive(false);
            }
            else
            {
                //Decision Phase
                if (CombatTools.CheckIfAnyUnitIsTargetting())
                {
                    Select.SetActive(true);
                    Continue.SetActive(false);
                    Cancel.SetActive(true);
                }
                else
                {
                    Select.SetActive(true);
                    Continue.SetActive(false);
                    Cancel.SetActive(false);
                }
                //Pause
                if(BattleSystem.Instance.state == BattleStates.IDLE)
                {
                    Pause.SetActive(true);
                }
                else
                {
                    Pause.SetActive(false);
                }
             
            }
            
        }
        //Outside Battle
        else if(BattleLog.Instance.state == BattleLogStates.TALKING)
        {
            this.GetComponent<Image>().enabled = true;
            Continue.SetActive(true);
            Select.SetActive(false);
            Cancel.SetActive(false);
            Pause.SetActive(false);
        }
        else
        {
            this.GetComponent<Image>().enabled = false;
            Select.SetActive(false);
            Continue.SetActive(false);
            Cancel.SetActive(false);
            Pause.SetActive(false);
        }
    }
}
