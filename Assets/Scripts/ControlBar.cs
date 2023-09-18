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
    //public GameObject WaitingControls;

    public void Update()
    {
        if (BattleSystem.Instance != null)
        {
            this.gameObject.SetActive(true);
            this.GetComponent<Image>().enabled = true;
            if (BattleSystem.Instance.state == BattleStates.TALKING)
            {
                Continue.SetActive(true);
                Select.SetActive(false);
                Cancel.SetActive(false);
            }
            else
            {
                if (Tools.CheckIfAnyUnitIsDeciding())
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
            }
            
        }
        else
        {
            this.GetComponent<Image>().enabled = false;
            Select.SetActive(false);
            Continue.SetActive(false);
            Cancel.SetActive(false);
        }
    }
}
