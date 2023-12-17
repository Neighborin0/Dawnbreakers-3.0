using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.UI.CanvasScaler;



public class ActionContainer : MonoBehaviour
{

    public ActionTypeButton lightButton;
    public ActionTypeButton heavyButton;


    public Action action;
    public TextMeshProUGUI textMesh;
    public bool targetting = false;
    public Unit baseUnit;
    public Button button;
    public TextMeshProUGUI damageNums;
    public TextMeshProUGUI costNums;
    public TextMeshProUGUI durationNums;
    public GameObject damageParent;
    public GameObject costParent;
    public GameObject durationParent;
    private float newTimeLineSpeedDelay = 0.1f;
    public bool Disabled = false;
    public int numberofUses;
    public bool limited = false;
    private TimeLineChild TL;
    private GameObject currentEffectPopup;

    void Awake()
    {
        if (BattleSystem.Instance != null)
        {
            if (!this.Disabled)
                button.interactable = true;
            Unit[] units = Tools.GetAllUnits();
            foreach (var unit in units)
            {
                unit.IsHighlighted = false;
                unit.isDarkened = false;
            }
        }
        numberofUses = action.numberofUses;
        limited = action.limited;
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
    }

    void Update()
    {
        var hit = Tools.GetMousePos();

        if (targetting && BattleSystem.Instance.state != BattleStates.WON)
        {
            //Checks if mouse is actually hovering over something
            if (hit.collider != null && hit.collider.gameObject.GetComponent<BoxCollider>() != null && hit.collider.gameObject.GetComponent<Unit>() != null && action.targetType == Action.TargetType.ENEMY && action.actionType == Action.ActionType.ATTACK && hit.collider.gameObject.GetComponent<Unit>().IsHighlighted && !hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
            {
                var unit = hit.collider.gameObject.GetComponent<Unit>();
                unit.timelinechild.Shift(unit);

                //If the user's attack is greater than zero, then string return is the true damage, otherwise the string is zero.

                damageNums.text = (int)((CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat) * CombatTools.ReturnTypeMultiplier(unit, action.damageType)) > 0 ?
                    $"<sprite name=\"{action.damageType}\">" +
                    ((int)((CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat) * CombatTools.ReturnTypeMultiplier(unit, action.damageType))).ToString()
                    : $"<sprite name=\"{action.damageType}\">" + "0";

                //If the true damage is being reduced, then the text will turn red. The text also turns red when a resisted move appears

                if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) < 1)
                {
                    damageNums.color = Color.red;
                }

                //If the true damage is being increased, then the text will turn green. The text also turns green when a effective move appears

                else if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) > 1)
                {
                    damageNums.color = Color.green;
                }
            }
            else if (action.targetType == Action.TargetType.ENEMY && action.actionType == Action.ActionType.ATTACK)
            {
                damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat).ToString();
                damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
            }

            costNums.text = CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier < 100 ? $"{CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier}%" : $"100%";

            //Action Cancel
            if (Input.GetMouseButtonUp(1))
            {
                LabCamera.Instance.MoveToUnit(CombatTools.FindDecidingUnit(), Vector3.zero);
                SetActive(false);
            }

            switch (action.actionStyle)
            {
                case Action.ActionStyle.STANDARD:
                    {
                        this.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
                        this.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                    }
                    break;
                case Action.ActionStyle.HEAVY:
                    {
                        Color heavyColor = new Color(225, 1, 0);
                        this.GetComponent<Image>().material.SetFloat("OutlineThickness", 1);
                        this.GetComponent<Image>().material.SetColor("OutlineColor", heavyColor);
                        baseUnit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", heavyColor * 0.02f);
                    }
                    break;
                case Action.ActionStyle.LIGHT:
                    {
                        Color lightColor = new Color(0, 162, 191);
                        this.GetComponent<Image>().material.SetFloat("OutlineThickness", 1);
                        this.GetComponent<Image>().material.SetColor("OutlineColor", lightColor);
                        baseUnit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", lightColor * 0.02f);
                    }
                    break;
            }

            switch (action.targetType)
            {
                case Action.TargetType.ENEMY:

                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (!unit.IsPlayerControlled)
                        {
                            unit.IsHighlighted = true;
                        }
                        else
                        {
                            unit.isDarkened = true;
                        }

                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (hit.collider != null && hit.collider.gameObject != null && hit.collider != baseUnit.gameObject.GetComponent<BoxCollider>() && hit.collider.gameObject.GetComponent<Unit>() != null && !hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
                        {
                            baseUnit.state = PlayerState.READY;
                            var unit = hit.collider.GetComponent<Unit>();

                            action.targets = unit;
                            action.unit = baseUnit;

                            baseUnit.Queue(action);
                            baseUnit.timelinechild.CanMove = true;
                            Director.Instance.timelinespeedDelay = newTimeLineSpeedDelay;
                            Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                            BattleLog.Instance.ResetBattleLog();
                            LabCamera.Instance.ResetPosition();
                            SetActive(false);
                        }
                    }
                    break;
                case Action.TargetType.SELF:
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (targetting)
                        {
                            if (unit != baseUnit)
                            {
                                unit.isDarkened = true;
                            }
                            baseUnit.IsHighlighted = true;
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        var bU = Tools.GetMousePos();
                        if (hit.collider != null && bU.collider.gameObject != null && bU.collider == baseUnit.gameObject.GetComponent<BoxCollider>())
                        {
                            baseUnit.state = PlayerState.READY;
                            action.targets = baseUnit;
                            action.unit = baseUnit;
                            baseUnit.Queue(action);
                            SetActive(false);
                            baseUnit.timelinechild.CanMove = true;
                            Director.Instance.timelinespeedDelay = newTimeLineSpeedDelay;
                            LabCamera.Instance.ResetPosition();
                            BattleLog.Instance.ResetBattleLog();
                            Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                        }
                    }
                    break;
                case Action.TargetType.ALLY:
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (targetting)
                        {
                            if (!unit.IsPlayerControlled)
                            {
                                unit.isDarkened = true;
                            }
                            else
                            {
                                unit.IsHighlighted = true;
                            }
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<BoxCollider>() && hit.collider.gameObject.GetComponent<Unit>() != null && hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
                        {
                            baseUnit.state = PlayerState.READY;
                            BattleLog.Instance.ResetBattleLog();
                            this.targetting = false;
                            var unit = hit.collider.GetComponent<Unit>();
                            action.targets = unit;
                            action.unit = baseUnit;
                            baseUnit.Queue(action);

                            baseUnit.timelinechild.CanMove = true;
                            Director.Instance.timelinespeedDelay = newTimeLineSpeedDelay;
                            Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                            SetActive(false);
                            LabCamera.Instance.ResetPosition();
                        }
                    }
                    break;
            }
        }
        else
        {
            if (damageNums != null)
                damageNums.color = new Color(1, 0.8705882f, 0.7058824f);

            this.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
            this.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
        }

    }

    public IEnumerator AutoSelectNextAvailableUnit()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (var PU in BattleSystem.Instance.playerUnits)
        {
            if (PU.state != PlayerState.READY)
            {
                PU.StartDecision();
                break;
            }
        }
    }

    public IEnumerator lightCoroutine;
    public void SetStyleLight(bool TurnOn)
    {
        if (TurnOn)
        {
            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = CombatTools.TurnOnDirectionalLight(0.01f);
            if(this != null)
           Director.Instance.StartCoroutine(lightCoroutine);

        }
        else
        {
            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = CombatTools.TurnOffDirectionalLight(0.01f);
            if (this != null)
                Director.Instance.StartCoroutine(lightCoroutine);
        }
    }
    public void SetDescription()
    {
        if (isActiveAndEnabled)
        {
            AudioManager.Instance.Play("ButtonHover");
            action.unit = baseUnit;
            if (currentEffectPopup == null)
            {
                var EP = Instantiate(Director.Instance.EffectPopUp, Director.Instance.canvas.transform);
                EP.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                currentEffectPopup = EP;
            }
            else
            {
                currentEffectPopup.SetActive(true);
            }

            var rectTrans = transform.GetComponent<RectTransform>();

            var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
            //Description for Battle
            EPtext.text = $"{action.GetDescription()}";

            if (limited)
            {
                EPtext.text += $"\nUses: {numberofUses}.";
            }
            if (!action.CanBeStyled)
            {
                EPtext.text += "<color=#FF0000>\nCan't be styled.</color>";
            }

            if (BattleSystem.Instance != null)
            {
                currentEffectPopup.transform.GetComponent<RectTransform>().localPosition = new Vector3(rectTrans.anchoredPosition.x - 400, rectTrans.anchoredPosition.y - 210, 0);
            }
            else
            {
                var rect = transform.position;
                currentEffectPopup.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(rect.x - 706, rect.y - 465);
            }

            Director.Instance.StartCoroutine(Tools.UpdateParentLayoutGroup(EPtext.gameObject));
        }
    }




    public void RemoveDescription()
    {
        if (currentEffectPopup != null)
            currentEffectPopup.SetActive(false);
    }


    public void ResetAllStyleButtons(bool turnOn = false)
    {
        if (action.actionStyle != Action.ActionStyle.STANDARD && baseUnit.state == PlayerState.DECIDING)
        {
            CombatTools.ReturnPipCounter().AddPip();
        }
        action.actionStyle = Action.ActionStyle.STANDARD;
        //action.ResetAction();
        var Light = baseUnit.GetComponentInChildren<Light>();
        baseUnit.ChangeUnitsLight(Light, 0, 15, Color.white, 0.04f, 0.001f);
        lightButton.state = ActionTypeButton.ActionButtonState.LIGHT;
        heavyButton.state = ActionTypeButton.ActionButtonState.HEAVY;
        SetActionStyleButtonsActive(turnOn);

    }

    public void UpdateOnStyleSwitch()
    {
        if (TL != null)
        {
            TL.staminaText.text = (100 - CombatTools.DetermineTrueCost(action)).ToString();
            TL.rectTransform.anchoredPosition = new Vector3((100 - CombatTools.DetermineTrueCost(action)) * TL.offset, 50);
        }
        SetDescription();
    }

    public void SetActionStyleButtonsActive(bool SetActive)
    {
        if (Director.Instance.UnlockedPipSystem)
        {
            if (action.CanBeStyled)
            {
                lightButton.gameObject.SetActive(SetActive);
                heavyButton.gameObject.SetActive(SetActive);
            }
            Director.Instance.StartCoroutine(CombatTools.StopAndDestroyVFX(0.01f));
            lightButton.interactable = true;
            heavyButton.interactable = true;
        }
        else
        {
            lightButton.gameObject.SetActive(false);
            heavyButton.gameObject.SetActive(false);
        }
    }
    public void SetActive(bool TurnOn)
    {
        if (Director.Instance.timeline.gameObject.activeSelf)
        {
            damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
            if (BattleSystem.Instance != null)
            {
                //Turns Off
                if (targetting == true || !TurnOn)
                {
                    targetting = false;
                    SetActionStyleButtonsActive(false);
                    var Light = baseUnit.GetComponentInChildren<Light>();
                    baseUnit.ChangeUnitsLight(Light, 0, 15, Color.white, 0.1f, 0);
                    LabCamera.Instance.ResetPosition();
                    RemoveDescription();
                    ResetAllStyleButtons();
                    SetStyleLight(true);
                    if (action.targetType == Action.TargetType.ENEMY && action.actionType == Action.ActionType.ATTACK)
                    {
                        damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat).ToString();
                        damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
                    }
                    costNums.text = CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier < 100 ? $"{CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier}%" : $"100%";

                    var button = GetComponent<Button>();

                    if (!Disabled)
                    {
                        if (limited)
                        {
                            if (numberofUses > 0)
                            {
                                button.interactable = true;
                            }
                            else
                            {
                                button.interactable = false;
                            }
                        }
                        else
                            button.interactable = true;
                    }

                    foreach (var z in Tools.GetAllUnits())
                    {
                        z.IsHighlighted = false;
                        z.isDarkened = false;
                    }

                    if (TL != null)
                    {
                        Director.Instance.timeline.children.Remove(TL);
                        Destroy(TL.gameObject);
                    }

                }
                //Turn On
                else
                {
                    SetActionStyleButtonsActive(false);
                    ActionContainer[] actionContainers = UnityEngine.Object.FindObjectsOfType<ActionContainer>();
                    foreach (var x in actionContainers)
                    {
                        if (x != this)
                        {
                            x.SetActive(false);
                            var button = x.GetComponent<Button>();
                            if (!x.Disabled)
                            {
                                if (x.limited)
                                {
                                    if (numberofUses > 0)
                                    {
                                        button.interactable = true;
                                    }
                                    else
                                    {
                                        button.interactable = false;
                                    }
                                }
                                else
                                    button.interactable = true;
                            }
                        }

                    }
                    foreach (TimeLineChild child in Director.Instance.timeline.children)
                    {
                        if (child.CanClear)
                        {
                            Director.Instance.timeline.children.Remove(child);
                            Destroy(child.gameObject);
                            break;
                        }
                    }
                    LabCamera.Instance.ResetPosition();
                    RemoveDescription();
                    TL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
                    TL.unit = baseUnit;
                    TL.portrait.sprite = baseUnit.charPortraits[0];
                    Director.Instance.timeline.children.Add(TL);
                    TL.CanMove = false;
                    TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);
                    TL.rectTransform.anchoredPosition = new Vector3((100 - CombatTools.DetermineTrueCost(action)) * TL.offset, 50);
                    TL.staminaText.text = (100 - CombatTools.DetermineTrueCost(action)).ToString();
                    TL.CanClear = true;
                    TL.CanBeHighlighted = false;
                    TL.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
                    TL.portrait.color = new Color(1, 1, 1, 0.5f);
                    targetting = true;
                    print("targetting");
                    SetDescription();
                    button.interactable = false;
                    if (CombatTools.ReturnPipCounter().pipCount > 0)
                    {
                        SetActionStyleButtonsActive(true);
                    }
                    else
                    {
                        SetActionStyleButtonsActive(false);
                    }
                    if (action.targetType == Action.TargetType.ENEMY)
                        LabCamera.Instance.MoveToPosition(new Vector3(1, LabCamera.Instance.transform.position.y, LabCamera.Instance.transform.position.z), 1f);

                }
            }
            else
            {
                SetDescription();
            }

        }

    }

}
