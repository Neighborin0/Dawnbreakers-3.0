using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static ActionContainer;
using UnityEngine.XR;
using JetBrains.Annotations;

public class ActionTypeButton : Button
{
    public enum ActionButtonState { LIGHT, HEAVY, DEFAULT }
    public ActionButtonState state;

    public float lightEfficiencyModifier = 0.3f;
    public float heavyEfficiencyModifier = 1.3f;

    public float heavyCostModifier = 20;
    public float lightCostModifier = -20;

    public ActionContainer actionContainerParent;
    public Animator BaseAnimator;

    public new void Start()
    {
        if (GetComponent<Animator>() != null)
            BaseAnimator = GetComponent<Animator>();
    }
    public void ModifyAction()
    {
        var action = actionContainerParent.action;
        AudioManager.Instance.Stop("statUp_Loop_001");
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
                    actionContainerParent.SetStyleLight(false);

                    var target = actionContainerParent.baseUnit;
                    var Light = actionContainerParent.baseUnit.GetComponentInChildren<Light>();
                    Color lightColor = new Color(0, 216, 255);
                    Light.color = lightColor * 0.01f;
                    Light.intensity = 1f;
                    AudioManager.QuickPlay("button_Hit_005", false);
                    AudioManager.Instance.Play("statUp_Loop_001", 0, false, 1f);

                    Director.Instance.StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatUpVFX", lightColor * 0.3f, lightColor * 0.3f, new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y, 0), Quaternion.identity, float.PositiveInfinity, 0, true, 0, 0.1f, 0.01f));
                    Director.Instance.StartCoroutine(CombatTools.PlayVFX(target.gameObject, "IgniteSmoke", lightColor * 0.3f, lightColor * 0.3f, new Vector3(0, 0, 0), Quaternion.identity, float.PositiveInfinity, 0, true, 0, 0.1f, 0.01f));
                    Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(0.01f));
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
                    actionContainerParent.SetStyleLight(false);

                    var target = actionContainerParent.baseUnit;
                    var Light = actionContainerParent.baseUnit.GetComponentInChildren<Light>();
                    Color heavyColor = new(225, 27, 0);
                    Light.color = heavyColor * 0.01f;
                    Light.intensity = 1f;
                    AudioManager.QuickPlay("button_Hit_005", false);
                    AudioManager.QuickPlay("statUp_Loop_001");


                    Director.Instance.StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatUpVFX", heavyColor * 0.3f, heavyColor * 0.3f, new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y, 0), Quaternion.identity, float.PositiveInfinity, 0, true, 0, 0.1f, 0.01f));
                    Director.Instance.StartCoroutine(CombatTools.PlayVFX(target.gameObject, "IgniteSmoke", heavyColor * 0.3f, heavyColor * 0.3f, new Vector3(0, 0, 0), Quaternion.identity, float.PositiveInfinity, 0, true, 0, 0.1f, 0.01f));
                    Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(0.01f));
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
                    actionContainerParent.SetStyleLight(true);
                    var Light = actionContainerParent.baseUnit.GetComponentInChildren<Light>();
                    actionContainerParent.baseUnit.ChangeUnitsLight(Light, 0, 15, Light.color, 0.1f, 0);
                    AudioManager.QuickPlay("ui_woosh_002");
                    Director.Instance.StartCoroutine(CombatTools.StopAndDestroyVFX(0.01f));
                }
                break;
        }
    }

   

}
