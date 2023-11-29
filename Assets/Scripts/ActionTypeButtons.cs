using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static ActionContainer;

public class ActionTypeButton : Button
{
    public enum ActionButtonState { LIGHT, HEAVY, DEFAULT }
    public ActionButtonState state;

    public float lightEfficiencyModifier = 0.3f;
    public float heavyEfficiencyModifier = 1.3f;

    public float heavyCostModifier = 20;
    public float lightCostModifier = -20;



    public ActionContainer actionContainerParent;

    public void ModifyAction()
    {
        var action = actionContainerParent.action;
        switch (state)
        {
            case ActionButtonState.LIGHT:
                {
                    if(action.actionType == Action.ActionType.STATUS && action.statAmount != 0)
                    {
                        action.statAmount = (int)(action.statAmount * lightEfficiencyModifier);
                    }
                    else
                    {
                        action.damage = (int)(action.damage * lightEfficiencyModifier);
                    }
                    action.cost += lightCostModifier;
                    Director.Instance.timeline.pipCounter.TakePip();
                    actionContainerParent.lightButton.gameObject.SetActive(false);
                    actionContainerParent.heavyButton.state = ActionButtonState.DEFAULT;
                    actionContainerParent.actionStyle = ActionContainer.ActionStyle.LIGHT;
                }
                break;
            case ActionButtonState.HEAVY:
                {
                    if (action.actionType == Action.ActionType.STATUS && action.statAmount != 0)
                    {
                        action.statAmount = (int)(action.statAmount * heavyEfficiencyModifier);
                    }
                    else
                    {
                        action.damage = (int)(action.damage * heavyEfficiencyModifier);
                    }
                    action.cost += heavyCostModifier;
                    Director.Instance.timeline.pipCounter.TakePip();
                    actionContainerParent.heavyButton.gameObject.SetActive(false);
                    actionContainerParent.lightButton.state = ActionButtonState.DEFAULT;
                    actionContainerParent.actionStyle = ActionContainer.ActionStyle.HEAVY;
                }
                break;
            case ActionButtonState.DEFAULT:
                {
                    var newAction = Instantiate(action);
                    if (action.actionType == Action.ActionType.STATUS && action.statAmount != 0)
                    {
                        action.statAmount = newAction.statAmount;
                    }
                    else
                    {
                        action.damage = newAction.damage;
                    }
                    action.cost = newAction.cost;
                    actionContainerParent.heavyButton.gameObject.SetActive(true);
                    actionContainerParent.lightButton.gameObject.SetActive(true);
                    actionContainerParent.lightButton.state = ActionButtonState.LIGHT;
                    actionContainerParent.heavyButton.state = ActionButtonState.HEAVY;
                    actionContainerParent.actionStyle = ActionContainer.ActionStyle.STANDARD;
                    Director.Instance.timeline.pipCounter.AddPip();
                }
                break;
        }
    }

    
}
