using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.ProBuilder.Shapes;


public class IntentContainer : MonoBehaviour
{
    public Action action;
    public TextMeshProUGUI textMesh;
    public GameObject damageParent;
    public TextMeshProUGUI damageNums;
    public TextMeshProUGUI costNums;
    public Unit unit;
    private GameObject currentEffectPopup;
    private bool CreatedTempTimelineChild = false;
    private TimeLineChild TempTL;

    public Image portraitParent;
    public Image portrait;

    void Start()
    {
        var ScaleComponent = GetComponent<ScalableObject>();
        var OldScaleVector = ScaleComponent.oldScaleSize;
        OldScaleVector = this.transform.localScale;
        ScaleComponent.newScaleSize = new Vector3(OldScaleVector.x * 1.02f, OldScaleVector.y * 1.02f, OldScaleVector.z * 1.02f);
    }
    public void DisplayIntentInfo()
    {
        if (BattleSystem.Instance.state != BattleStates.BATTLE)
        {
            action.targets.IsHighlighted = true;
            action.unit.timelinechild.HighlightedIsBeingOverwritten = true;
            this.unit.timelinechild.HighlightedIsBeingOverwritten = true;
            this.unit.timelinechild.Shift(this.unit);
            var newAction = Instantiate(action);
            newAction.unit = unit;
            SetDescription();

            if (!CreatedTempTimelineChild && CombatTools.ReturnIconStatus(newAction.targets, "INDOMITABLE") && CombatTools.DetermineTrueCost(Director.Instance.timeline.ReturnTimeChildAction(newAction.targets)) > CombatTools.DetermineTrueCost(action))
            {
                CreateTempTimeLineChild(newAction.targets);
            }
        }
    }

    public void SetDescription()
    {
        //AudioManager.Instance.Play("ButtonHover");
        if (currentEffectPopup == null)
        {
            var EP = Instantiate(Director.Instance.EffectPopUp, BattleSystem.Instance.canvasParent.transform);
            EP.transform.localScale = new Vector3(0.025f, 0.03f, -25f);
            currentEffectPopup = EP;
        }
        else
        {
            currentEffectPopup.SetActive(true);
        }
        currentEffectPopup.transform.GetComponent<RectTransform>().position = new Vector3(transform.position.x + 1f, transform.position.y - 1.5f, transform.position.z);
        currentEffectPopup.transform.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
        EPtext.text = $"{action.ReturnActionType()}\n{action.GetDescription()}";
        StartCoroutine(Tools.UpdateParentLayoutGroup(EPtext.gameObject));
    }

    void Update()
    {
        if (textMesh != null && damageNums.IsActive() && unit != null && action != null && action.targets != null)
        {
            if ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType))> 0)
            {
                damageNums.text = $"<sprite name=\"{action.damageType}\">" + ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType))).ToString();
                action.damageText = ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType))).ToString();
            }
            else
                damageNums.text = $"<sprite name=\"{action.damageType}\">0";
        }
      
    }
    public void RemoveDescription()
    {
        action.targets.IsHighlighted = false;
        this.unit.timelinechild.Return();
        this.unit.timelinechild.HighlightedIsBeingOverwritten = false;
        if (currentEffectPopup != null)
            currentEffectPopup.SetActive(false);

        if (CreatedTempTimelineChild)
        {
            Destroy(TempTL.gameObject);
            CreatedTempTimelineChild = false;
        }
    }

    public void CreateTempTimeLineChild(Unit TargetUnit)
    {
        CreatedTempTimelineChild = true;
        TempTL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
        TempTL.unit = TargetUnit;
        TempTL.portrait.sprite = TargetUnit.charPortraits[0];
        Director.Instance.timeline.children.Add(TempTL);
        TempTL.CanMove = false;


        float costToReturn = CombatTools.DetermineTrueCost(Director.Instance.timeline.ReturnTimeChildAction(TargetUnit));
        costToReturn += Director.Instance.TimelineReduction;

        if (action.actionStyle != Action.ActionStyle.STANDARD)
            costToReturn += 10;

        if (costToReturn > 100 || action.AppliesStun)
            costToReturn = 100;

        if (TargetUnit.IsPlayerControlled)
            TempTL.rectTransform.anchoredPosition = new Vector3((100 - costToReturn) * TempTL.offset, 50);
        else
            TempTL.rectTransform.anchoredPosition = new Vector3((100 - costToReturn) * TempTL.offset, -50);

        TempTL.staminaText.text = (costToReturn).ToString();
        TempTL.CanClear = true;
        TempTL.GetComponent<LabUIInteractable>().CanHover = false;
        TempTL.CanBeHighlighted = false;
        TempTL.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
        TempTL.portrait.color = new Color(1, 1, 1, 0.5f);
    }

    public void CheckTarget(Action action, Unit unit)
    {
        if(action.targets == null)
        {
            switch (action.targetType)
            {
                case Action.TargetType.ENEMY:
                    action.targets = BattleSystem.Instance.numOfUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.numOfUnits.Count)];
                    break;
                case Action.TargetType.SELF:
                    action.targets = unit;
                    break;
                case Action.TargetType.ALL_ENEMIES:
                    action.targets = unit;
                    break;
                case Action.TargetType.ALLY:
                    action.targets = unit;
                    break;
            }
        }
    }


}
