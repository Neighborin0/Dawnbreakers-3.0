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
        var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
        EPtext.text = $"{action.GetDescription()}";
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
