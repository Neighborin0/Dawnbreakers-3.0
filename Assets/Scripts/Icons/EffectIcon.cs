using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class EffectIcon : MonoBehaviour
{
    public Image icon;
    public Unit owner;
    public TextMeshProUGUI timerText;
    public float duration;
    public bool ForceEnd = false;
    public bool DoFancyStatChanges;
    public string description;
    public string iconName;
    public float storedValue;
    public bool TimedEffect = true;
    private IEnumerator scaler;
    public int NumberofStacks;
    public GameObject currentEffectPopup;
    public float descriptionSize = 0.4f;

    public void Awake()
    {
        StartCoroutine(Pop());
    }

    public void Start()
    {
       timerText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void Initalize(Unit unit, bool dofancystatchanges, float duration = 0, float storedvalue = 0, float numberofStacks = 0)
    {
    
        if(duration > 0)
        {
            timerText.text = duration.ToString();
        }
        else if(numberofStacks > 0) 
        {
            timerText.text = numberofStacks.ToString();
        }
        else
        {
            timerText.text = string.Empty;
        }
        var manIHateUnityScalingSometimesAndIDontWantToBeFuckedWithThisSoHaveThisLongAssVariable = timerText.GetComponent<RectTransform>();
        manIHateUnityScalingSometimesAndIDontWantToBeFuckedWithThisSoHaveThisLongAssVariable.sizeDelta = new Vector2(70.24f, 21.96f);    
        storedValue = storedvalue;
        DoFancyStatChanges = dofancystatchanges;
    }


    public void Tick()
    {
        if (TimedEffect)
        {
            if (duration > 1)
            {
                duration -= 1;
            }
            else
            {
                duration -= 1;
                DestoryEffectIcon();
            }
            timerText.text = duration.ToString();
        }
    }

    public IEnumerator Pop()
    {
        scaler = Tools.SmoothScaleObj(transform, new Vector3(1.5f, 1.5f, 0), 0.001f);
        StartCoroutine(scaler);
        yield return new WaitForSeconds(0.2f);
        StopCoroutine(scaler);
        scaler = Tools.SmoothScaleObj(transform, new Vector3(1, 1, 0), 0.001f);
        StartCoroutine(scaler);
    }

    public void DisplayDescription()
    {
        if (BattleSystem.Instance.state != BattleStates.BATTLE)
        {
            if (currentEffectPopup == null)
            {
                var EP = Instantiate(Director.Instance.EffectPopUp, BattleSystem.Instance.canvasParent.transform);
                currentEffectPopup = EP;
            }
            else
            {
                currentEffectPopup.SetActive(true);
            }
            currentEffectPopup.transform.GetComponent<RectTransform>().position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
            EPtext.text = this.GetDescription();
            StartCoroutine(Tools.UpdateParentLayoutGroup(EPtext.gameObject));
        }
    }

  

    public void RemoveDescription()
    {
        if(currentEffectPopup != null)
       currentEffectPopup.SetActive(false);
    }

    public void DestoryEffectIcon()
    {
        OnEnded();
        RemoveDescription();
    }
    public virtual string GetDescription() { return description; }

    public void OnEnded() { Director.Instance.StartCoroutine(End()); }
    public virtual IEnumerator End() { yield break; }
 

}
