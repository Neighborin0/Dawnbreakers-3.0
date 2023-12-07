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
                    var newAction = Instantiate(action);
                    CombatTools.ReturnPipCounter().TakePip();
                    actionContainerParent.lightButton.interactable = false;
                    actionContainerParent.heavyButton.state = ActionButtonState.DEFAULT;    
                    newAction.actionStyle = Action.ActionStyle.LIGHT;
                    actionContainerParent.action = newAction;
                    actionContainerParent.UpdateOnStyleSwitch();
                }
                break;
            case ActionButtonState.HEAVY:
                {
                    
                    var newAction = Instantiate(action);
                    CombatTools.ReturnPipCounter().TakePip();
                    actionContainerParent.heavyButton.interactable = false;
                    actionContainerParent.lightButton.state = ActionButtonState.DEFAULT;
                    newAction.actionStyle = Action.ActionStyle.HEAVY;
                    actionContainerParent.action = newAction;
                    actionContainerParent.UpdateOnStyleSwitch();
                }
                break;
            case ActionButtonState.DEFAULT:
                {
                    var newAction = Instantiate(action);
                    action.cost = newAction.cost;
                    actionContainerParent.SetActionStyleButtonsActive(true);
                    actionContainerParent.lightButton.state = ActionButtonState.LIGHT;
                    actionContainerParent.heavyButton.state = ActionButtonState.HEAVY;
                    action.actionStyle = Action.ActionStyle.STANDARD;
                    CombatTools.ReturnPipCounter().AddPip();
                    actionContainerParent.UpdateOnStyleSwitch();
                }
                break;
        }
    }

    
}
