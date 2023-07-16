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
    //public GameObject WaitingControls;

    public void Update()
    {
        if (Director.Instance.timeline.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
            this.GetComponent<Image>().enabled = true;
            if (Tools.CheckIfAnyUnitIsDeciding())
            {
                Select.SetActive(true);
                Cancel.SetActive(true);
            }
            else
            {
                Select.SetActive(true);
                Cancel.SetActive(false);
            }
        }
        else
        {
            this.GetComponent<Image>().enabled = false;
            Select.SetActive(false);
            Cancel.SetActive(false);
        }
    }
}
