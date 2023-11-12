using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class Stamina : MonoBehaviour
{
    public float value;
    //public Slider backSlider;
    public Unit unit;
    //public bool Paused = false;
    void Start()
    {
        value = 100;
        /*backSlider.maxValue = 100;
        if (!unit.IsPlayerControlled)
        {
            slider.value = unit.StartingStamina;
            backSlider.value = slider.value;
        }
        */
    }

    /*void Update()
    {
        if (slider.value < slider.maxValue && !Paused)
        {
            slider.value += (float)(unit.speedStat * Time.deltaTime) / OptionsManager.Instance.UserTimelineSpeedDelay;
        }


    }
    */


    public void DoCost(float cost)
    {
        //backSlider.value = backSlider.maxValue;
        value -= cost * unit.actionCostMultiplier < 100 ? cost * unit.actionCostMultiplier : 100;
    }
 

}