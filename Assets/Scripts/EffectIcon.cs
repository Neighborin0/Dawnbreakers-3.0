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
        BattleLog.SetBattleText("");
        BattleLog.SetBattleText($"{iconName}\n{description}");
    }

    public void RemoveDescription()
    {
        BattleLog.Instance.ResetBattleLog();
    }

    public void DestoryEffectIcon()
    {
            if (action != null)
                action.OnEnded(owner, storedValue, DoFancyStatChanges);
        Destroy(this.gameObject);

    }

   /* public IEnumerator FadeOut()
    {
        StartCoroutine(Tools.FadeObject(this.GetComponent<Image>(), 100f, false, false));
        yield return new WaitUntil(() => this.GetComponent<Image>().color == new Color(0, 0, 0, 1));
        Destroy(this.gameObject);

    }
   */
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
