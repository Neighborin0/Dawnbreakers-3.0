using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem.HID;
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

  
    public void CheckTimelineForSameValues(TimeLineChild TLToCheck)
    {
        var timelineChildren = Director.Instance.timeline.children;
        if (!TLToCheck.CanClear)
        {
            foreach (var OtherTL in timelineChildren)
            {
                if (OtherTL != TLToCheck && !OtherTL.CanClear && OtherTL.value == TLToCheck.value && CombatTools.CompareAffiliations(OtherTL.unit, TLToCheck.unit) && OtherTL.miniChild == null)
                {
                    SetupMiniChild(TLToCheck.unit, OtherTL);
                    TLToCheck.gameObject.SetActive(false);
                    break; 
                }           
            }
        }

    }


    public void SetupMiniChild(Unit TargetUnit, TimeLineChild parent)
    {
        var miniChild = ReturnMiniChild(TargetUnit, parent);
        miniChild.unit = TargetUnit;
        miniChild.portrait.sprite = TargetUnit.charPortraits[0];
        miniChild.parent = parent;
        miniChild.gameObject.SetActive(true);
        TargetUnit.HasMiniTimelineChild = true;
    }

    public MiniTimelineChildren ReturnMiniChild(Unit TargetUnit, TimeLineChild parent)
    {
        if (TargetUnit.IsPlayerControlled)
            parent.miniChild = parent.PlayerMiniChild;
        else
            parent.miniChild = parent.EnemyMiniChild;


        return parent.miniChild;
    }
    public void RemoveMiniChild(Unit TargetUnit, TimeLineChild parent)
    {
        foreach (TimeLineChild TL in Director.Instance.timeline.children.ToList())
        {
            if(TL.miniChild != null && TL.miniChild.unit != null && TL.miniChild.unit == TargetUnit)
            {
                TL.miniChild.gameObject.SetActive(false);
                TL.unit.HasMiniTimelineChild = false;
                TL.miniChild.unit = null;
                TL.miniChild = null;
                break;
            }
        }
    }

    //In Battle
    public IEnumerator ResetTimeline()
    {
        Resetting = true;

        foreach (var x in children.ToList())
        {
            Director.Instance.timeline.children.Remove(x);
            Destroy(x.gameObject);
        }
          

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

    public void ReplaceMainPortraitWithMiniPortrait(TimeLineChild TL)
    {
        
        if(TL.miniChild != null)
        {
            (TL.miniChild.portrait.sprite, TL.portrait.sprite) = (TL.portrait.sprite, TL.miniChild.portrait.sprite);
            TL.PortraitHasBeenReplaced = true;
        }
    }

    public void RemoveTimelineChild(Unit unitToFind)
    {
        foreach (TimeLineChild TL in Director.Instance.timeline.children.ToList())
        {
            if (TL != null && TL.unit != null && (TL.unit == unitToFind || (TL.miniChild != null && TL.miniChild.unit == unitToFind)))
            {
               
                //Timeline Child is Mini Child
                if (TL.miniChild != null && TL.miniChild.unit == unitToFind)
                {
                    RemoveMiniChild(unitToFind, TL);
                }
                //Timeline has a different Mini Child
                else if (TL.miniChild != null && TL.miniChild.unit != unitToFind)
                {
                    foreach (TimeLineChild SecondPass in Director.Instance.timeline.children.ToList())
                    {
                        if(SecondPass.unit != null && SecondPass.unit == unitToFind)
                        {
                            SecondPass.gameObject.SetActive(true);
                            SecondPass.unit = SecondPass.miniChild.unit;
                            SecondPass.portrait.sprite = SecondPass.miniChild.portrait.sprite;
                            SecondPass.unit.timelinechild = SecondPass;
                            RemoveMiniChild(SecondPass.unit, SecondPass);
                            break;
                        }
                    }
                }
                //No Mini Child is found
                else if (TL.miniChild == null)
                {
                    Director.Instance.timeline.children.Remove(TL);
                    Director.Instance.StartCoroutine(FadeOut(TL));
                }
                break;
            }
        }


            foreach (var action in BattleSystem.Instance.ActionsToPerform.ToList())
            {
                if (action.unit.unitName == unitToFind.unitName)
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
        CheckTimelineForSameValues(TL);
    }

    public void DelayCheck(TimeLineChild TL)
    {
        if (!TL.CanClear)
        {
            if (TL.unit.HasMiniTimelineChild)
            {
                //works
                foreach (var OtherTL in children)
                {
                    if (OtherTL.miniChild != null && OtherTL.miniChild.unit != null && OtherTL.miniChild.unit == TL.unit)
                    {
                        TL.value = OtherTL.value;
                        TL.GetComponent<RectTransform>().anchoredPosition = OtherTL.GetComponent<RectTransform>().anchoredPosition;
                        RemoveMiniChild(TL.unit, OtherTL);
                        TL.gameObject.SetActive(true);
                        break;
                    }

                }
                TL.gameObject.SetActive(true);
            }
            else if (TL.miniChild != null)
            {
                foreach (var OtherTL in children)
                {
                    if (TL.miniChild.unit == OtherTL.unit)
                    {
                        OtherTL.value = TL.value;
                        OtherTL.GetComponent<RectTransform>().anchoredPosition = TL.GetComponent<RectTransform>().anchoredPosition;
                        RemoveMiniChild(OtherTL.unit, TL);
                        OtherTL.gameObject.SetActive(true);
                        break;
                    }
                }

            }
        }
    }
}