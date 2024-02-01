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
    public ActionDisplayer actionDisplayer;

    public void Start()
    {
        slider.value = 0;
        actionDisplayer = GetComponentInChildren<ActionDisplayer>();
    }

    void FixedUpdate()
    {
        if (!Paused)
        {
            if (slider.value < slider.maxValue && !Resetting)
                slider.value += Time.deltaTime * OptionsManager.Instance.UserTimelineSpeedDelay;
        }
        if (slider.value > 0 && Resetting)
            slider.value -= Time.deltaTime * OptionsManager.Instance.UserTimelineSpeedDelay * 2f;
    }
    //In Battle
    public IEnumerator ResetTimeline()
    {
        Resetting = true;

        foreach (var x in Tools.GetAllUnits())
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

        if (unit.IsPlayerControlled)
            TL.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        else
            TL.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);

        TL.value = 100;
        return TL;
    }

    public TimeLineChild ReturnTimelineChild(Unit unit)
    {
        TimeLineChild TimelineKid = null;
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
        List<TimeLineChild> childrenToRemove = new List<TimeLineChild>();

        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child != null && child.unit.unitName == unit.unitName)
            {
                childrenToRemove.Add(child);
                Director.Instance.StartCoroutine(FadeOut(child));
            }
        }

        foreach (TimeLineChild childToRemove in childrenToRemove)
        {
            Director.Instance.timeline.children.Remove(childToRemove);
        }

        foreach (var action in BattleSystem.Instance.ActionsToPerform)
        {
            if (action.unit.unitName == unit.unitName)
            {
                if (BattleSystem.Instance.state == BattleStates.DECISION_PHASE && action.actionStyle != Action.ActionStyle.STANDARD)
                {
                    CombatTools.ReturnPipCounter().AddPip();
                }
                foreach (var skill in action.unit.skillUIs)
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

    public IEnumerator FadeOut(TimeLineChild timeLineChild)
    {
        if (timeLineChild != null && timeLineChild.gameObject != null)
        {
            while (timeLineChild.childImage != null && timeLineChild.childImage.color.a > 0 && timeLineChild.gameObject != null)
            {
                timeLineChild.childImage.color = new Color(timeLineChild.childImage.color.r, timeLineChild.childImage.color.g, timeLineChild.childImage.color.b, timeLineChild.childImage.color.a - 0.1f);
                timeLineChild.portrait.color = new Color(timeLineChild.portrait.color.r, timeLineChild.portrait.color.g, timeLineChild.portrait.color.b, timeLineChild.portrait.color.a - 0.1f);
                timeLineChild.staminaText.color = new Color(timeLineChild.staminaText.color.r, timeLineChild.staminaText.color.g, timeLineChild.staminaText.color.b, timeLineChild.staminaText.color.a - 0.1f);
                yield return null; 
            }

            yield return new WaitUntil(() => timeLineChild.childImage.color.a <= 0);

            if (timeLineChild.gameObject != null)
            {
                Destroy(timeLineChild.gameObject);
            }
        }
    }

    public IEnumerator actionDisplayerCoroutine;

    public void StartFadeAction(bool FadeIn)
    {
        if (actionDisplayerCoroutine != null)
        {
            StopCoroutine(actionDisplayerCoroutine);
        }
        actionDisplayerCoroutine = FadeActionDisplayer(FadeIn);
        Director.Instance.StartCoroutine(actionDisplayerCoroutine);

    }
    private IEnumerator FadeActionDisplayer(bool FadeIn)
    {
        actionDisplayer.gameObject.SetActive(true);
        var img = actionDisplayer.GetComponent<Image>();
        if (!FadeIn)
        {
            if (actionDisplayer.gameObject != null)
            {
                while (img.color.a > 0 && this != null)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - 0.1f);
                    actionDisplayer.baseText.color = new Color(actionDisplayer.baseText.color.r, actionDisplayer.baseText.color.g, actionDisplayer.baseText.color.b, actionDisplayer.baseText.color.a - 0.1f);
                    yield return new WaitForSeconds(0.001f);
                }
                yield return new WaitUntil(() => img.color.a <= 0);
                actionDisplayer.gameObject.SetActive(false);
            }
        }
        else
        {
            if (actionDisplayer.gameObject != null)
            {
                gameObject.SetActive(true);
                while (img.color.a < 1 && this != null)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.1f);
                    actionDisplayer.baseText.color = new Color(actionDisplayer.baseText.color.r, actionDisplayer.baseText.color.g, actionDisplayer.baseText.color.b, actionDisplayer.baseText.color.a + 0.1f);
                    yield return new WaitForSeconds(0.001f);
                }
                yield return new WaitUntil(() => img.color.a >= 1);
            }
        }
    }
    public void DoCost(float cost, Unit unit)
    {
        var TL = SpawnTimelineChild(unit);
        TL.value -= cost * unit.actionCostMultiplier < 100 ? cost * unit.actionCostMultiplier : 100;
    }
}