using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Slider backSlider;
    public Unit unit;
    public bool Paused = false;
     void Start()
    {
        slider.maxValue = 100;
        backSlider.maxValue = 100;
        if (!unit.IsPlayerControlled)
        {
            slider.value = unit.StartingStamina;
            backSlider.value = slider.value;
        }
    }

    void Update()
    {
       if(slider.value < slider.maxValue && !Paused)
        {
            slider.value += (float)(unit.speedStat * Time.deltaTime) / Director.Instance.staminaSPDDivider;
        }
    
 
    }

  
    public void DoCost(float cost)
    {
        backSlider.value = backSlider.maxValue;
        StartCoroutine(HandleCost(cost));
    }
    private IEnumerator HandleCost(float cost)
    {
        slider.value -= cost * unit.actionCostMultiplier < 100 ? cost * unit.actionCostMultiplier : 100;
        yield return new WaitForSeconds(0.5f);
        while(backSlider.value > slider.value)
        {
            backSlider.value -= 2f;
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }
   

  
}
