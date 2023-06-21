using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class EffectIcon : MonoBehaviour
{
    public Image icon;
    public Unit owner;
    public TextMeshProUGUI timerText;
    public Action action;
    public bool isPaused;
    public bool ForceEnd = false;
    public bool DoFancyStatChanges;
    public string description;
    public string iconName;
    public float storedValue;
    public bool TimedEffect = true;
    private IEnumerator scaler;
    public GameObject currentEffectPopup;

    public void Start()
    {
        StartCoroutine(Pop());
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
        }
    }

    public void RemoveDescription()
    {
       currentEffectPopup.SetActive(false);
    }

    public void DestoryEffectIcon()
    {
            if (action != null)
                action.OnEnded(owner, storedValue, DoFancyStatChanges);
        Destroy(this.gameObject);

    }
    public virtual string GetDescription() { return description; }
    public void Update()
    {
        if(TimedEffect)
        {
            if (owner.stamina.Paused)
            {
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }
        }
       
       
    }
}
