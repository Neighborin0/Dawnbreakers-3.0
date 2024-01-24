using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.CanvasScaler;

public class TimeLine : MonoBehaviour
{
    public TimeLineChild borderChildprefab;
    public Transform endpoint;
    public Transform startpoint;
    public float minPos;
    public float maxPos;
    public List<TimeLineChild> children;
    public Slider slider;
    public bool Paused = true;
    public bool Resetting = false;
    public PipCounter pipCounter;

    public void Start()
    {
        slider.value = 0;
      
       
    }


    void FixedUpdate()
    {
        if (!Paused)
        {
            if (slider.value < slider.maxValue && !Resetting)
                slider.value += Time.deltaTime * OptionsManager.Instance.UserTimelineSpeedDelay;
        }
        if (slider.value > 0 && Resetting)
            slider.value -= Time.deltaTime * OptionsManager.Instance.UserTimelineSpeedDelay * 3f;
    }
    //In Battle
    public IEnumerator ResetTimeline()
    {
        Resetting = true;

        foreach(var x in Tools.GetAllUnits())
            Director.Instance.timeline.RemoveTimelineChild(x);

        yield return new WaitUntil(() => slider.value <= 0);
        Resetting = false;
        Paused = true;
    }
    //Post Battle
    public void RefreshTimeline()
    {
        Paused = true;
        slider.value = 0;
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.state = PlayerState.IDLE;
        }

    }

    public TimeLineChild SpawnTimelineChild(Unit unit)
    {
        var TL = Instantiate(borderChildprefab, startpoint);
        children.Add(TL);
        TL.portrait.sprite = unit.charPortraits[0];
        TL.unit = unit;
        unit.timelinechild = TL;

        if(unit.IsPlayerControlled)
            TL.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        else
            TL.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);

        TL.value = 100;
        return TL;
    }

    public TimeLineChild ReturnTimelineChild(Unit unit)
    {
        var TimelineKid = new TimeLineChild();
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child.unit.unitName == unit.unitName)
            {
                TimelineKid = child;
                break;
            }
        }
        return TimelineKid;
    }

    public Action ReturnTimeChildAction(Unit unit)
    {
        Action Act = null;
        foreach (var action in BattleSystem.Instance.ActionsToPerform)
        {
            if (action.unit.unitName == unit.unitName)
            {
                Act = action;
                break;
            }
        }
        return Act;
    }

    public void RemoveTimelineChild(Unit unit)
    {
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child.unit.unitName == unit.unitName)
            {
                Director.Instance.timeline.children.Remove(child);
                Director.Instance.StartCoroutine(child.FadeOut());
                break;
            }
        }
        foreach (var action in BattleSystem.Instance.ActionsToPerform)
        {
            if (action.unit.unitName == unit.unitName)
            {
                if(BattleSystem.Instance.state == BattleStates.DECISION_PHASE && action.actionStyle != Action.ActionStyle.STANDARD)
                {
                    CombatTools.ReturnPipCounter().AddPip();
                }
                foreach(var skill in action.unit.skillUIs)
                {
                    var actionContainer = skill.GetComponent<ActionContainer>();
                    actionContainer.lightButton.state = ActionTypeButton.ActionButtonState.LIGHT;
                    actionContainer.heavyButton.state = ActionTypeButton.ActionButtonState.HEAVY;
                }
                BattleSystem.Instance.ActionsToPerform.Remove(action);

                break;
            }
        }
    }
    public void DoCost(float cost, Unit unit)
    {
        var TL = SpawnTimelineChild(unit);
        TL.value -= cost * unit.actionCostMultiplier < 100 ? cost * unit.actionCostMultiplier : 100;
    }
}
