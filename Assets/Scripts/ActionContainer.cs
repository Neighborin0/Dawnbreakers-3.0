using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.UI.CanvasScaler;
using System.Linq;
using static Action;
using JetBrains.Annotations;

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
    public bool Disabled = false;
    public int numberofUses;
    public bool limited = false;
    private TimeLineChild TL;
    private GameObject currentEffectPopup;
    private bool HasDoneOnAction = false;
    private bool CreatedTempTimelineChild = false;
    private TimeLineChild TempTL;

    void Awake()
    {
        if (BattleSystem.Instance != null && !Disabled)
        {
            button.interactable = true;
            Unit[] units = Tools.GetAllUnits();
            foreach (var unit in units)
            {
                unit.IsHighlighted = false;
                unit.isDarkened = false;
            }
        }
        Disabled = false;
        numberofUses = action.numberofUses;
        limited = action.limited;
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        var scaleComponent = GetComponent<ScalableObject>();
        scaleComponent.oldScaleSize = Vector3.one;
        scaleComponent.newScaleSize = scaleComponent.oldScaleSize * 1.02f;

    }

    private void OnEnable()
    {
        try
        {
            if (BattleSystem.Instance != null && BattleSystem.Instance.state != BattleStates.WON)
                UpdateDamageNumsText();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in OnEnable: {ex.Message}");
        }
    }

    private void UpdateDamageNumsText()
    {
        damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat).ToString();
    }
    void Update()
    {
        var hit = Tools.GetMousePos();
        if (Input.GetMouseButtonUp(1))
        {
            SetStyleLight(true);
        }

        if (targetting && BattleSystem.Instance.state != BattleStates.WON)
        {
            UpdateDamageNumbers(hit);
            UpdateCostNumbers();
            HandleActionCancel();
            UpdateTargetHighlights();
            ExecuteActionOnClick(hit);
            UpdateActionStyleOutline();
            PerformOnActionSelected();
        }
        else
        {
            ResetUIState();
        }
    }

    void UpdateDamageNumbers(RaycastHit hit)
    {
        if (hit.collider != null && hit.collider.gameObject.GetComponent<BoxCollider>() != null &&
            hit.collider.gameObject.GetComponent<Unit>() != null && action.targetType == Action.TargetType.ENEMY &&
            action.actionType == Action.ActionType.ATTACK &&
            !hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
        {
            var targetUnit = hit.collider.gameObject.GetComponent<Unit>();
            UpdateDamageNumbersForTargetUnit(targetUnit);
            UpdateTempTimelineChildIfNeeded(targetUnit);
        }
        else if (action.targetType == Action.TargetType.ENEMY && action.actionType == Action.ActionType.ATTACK)
        {
            UpdateDamageNumbersForSelf();
            DestroyTempTimelineChildIfNeeded();
        }
    }
    void UpdateDamageNumbersForTargetUnit(Unit targetUnit)
    {
        int trueDamage = (int)((CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat) * CombatTools.ReturnTypeMultiplier(targetUnit, action.damageType));
        damageNums.text = $"<sprite name=\"{action.damageType}\">" + (trueDamage > 0 ? trueDamage.ToString() : "0");

        UpdateDamageNumberColor(targetUnit);
    }

    void UpdateDamageNumberColor(Unit targetUnit)
    {
        if (CombatTools.ReturnTypeMultiplier(targetUnit, action.damageType) < 1)
        {
            damageNums.color = Color.red;
        }
        else if (CombatTools.ReturnTypeMultiplier(targetUnit, action.damageType) > 1)
        {
            damageNums.color = Color.green;
        }
    }

    void UpdateTempTimelineChildIfNeeded(Unit targetUnit)
    {
        if (CombatTools.ReturnTypeMultiplier(targetUnit, action.damageType) > 1 || action.actionStyle != Action.ActionStyle.STANDARD || action.AppliesStun)
        {
            UpdateTempTimelineChild(targetUnit);
        }
    }

    void DestroyTempTimelineChildIfNeeded()
    {
        if (CreatedTempTimelineChild)
        {
            if (TempTL != null)
            {
                Director.Instance.timeline.children.Remove(TempTL);
                Destroy(TempTL.gameObject);
            }
            CreatedTempTimelineChild = false;
        }
    }

    void UpdateTempTimelineChild(Unit targetUnit)
    {
        if (!CreatedTempTimelineChild && CombatTools.ReturnIconStatus(targetUnit, "INDOMITABLE") && CombatTools.DetermineTrueCost(Director.Instance.timeline.ReturnTimeChildAction(targetUnit)) >= CombatTools.DetermineTrueCost(action))
        {
            CreateTempTimeLineChild(targetUnit);
        }
    }

    void UpdateDamageNumbersForSelf()
    {
        damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat).ToString();
        damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
    }

    void UpdateCostNumbers()
    {
        costNums.text = (CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier < 100) ?
                        $"{CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier}%" :
                        $"100%";
    }

    void HandleActionCancel()
    {
        if (Input.GetMouseButtonUp(1))
        {
            LabCamera.Instance.MoveToUnit(CombatTools.FindDecidingUnit(), Vector3.zero);
            AudioManager.QuickPlay("ui_woosh_002");
            SetStyleLight(true);
            SetActive(false);
        }
    }

    void UpdateTargetHighlights()
    {
        foreach (var unit in Tools.GetAllUnits())
        {
            if (targetting)
            {
                UpdateUnitHighlight(unit);
            }
            else
            {
                ResetUnitHighlight(unit);
            }
        }
    }

    void UpdateUnitHighlight(Unit unit)
    {
        if (!unit.IsPlayerControlled)
        {
            if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) < 1)
            {
                unit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", new Color(0.5754717f * 255, 0.4533197f * 255, 0.4533197f * 255) * 0.02f);
            }
            else if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) > 1)
            {
                unit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", new Color(255, 1, 0) * 0.02f);
            }
        }
    }

    void ResetUnitHighlight(Unit unit)
    {
        unit.IsHighlighted = false;
        unit.isDarkened = false;
    }

    void ExecuteActionOnClick(RaycastHit hit)
    {
        switch (action.targetType)
        {
            case Action.TargetType.ENEMY:

                foreach (var unit in Tools.GetAllUnits())
                {
                    if (targetting)
                    {
                        if (!unit.IsPlayerControlled)
                        {
                            unit.IsHighlighted = true;
                            if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) < 1) // Not Effective
                            {
                                unit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", new Color(0.5754717f * 255, 0.4533197f * 255, 0.4533197f * 255) * 0.02f);
                            }
                            else if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) > 1) //Effective
                            {
                                unit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", new Color(255, 1, 0) * 0.02f);
                            }
                        }
                        else
                        {
                            unit.isDarkened = true;
                        }

                    }
                    else
                    {
                        unit.IsHighlighted = false;
                        unit.isDarkened = false;
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
                        Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                        BattleLog.Instance.ResetBattleLog();
                        LabCamera.Instance.ResetPosition();
                        AudioManager.QuickPlay("button_Hit_005");
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
                        LabCamera.Instance.ResetPosition();
                        BattleLog.Instance.ResetBattleLog();
                        Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                        AudioManager.QuickPlay("button_Hit_005");
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
                        Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                        SetActive(false);
                        AudioManager.QuickPlay("button_Hit_005");
                        LabCamera.Instance.ResetPosition();
                    }
                }
                break;

            case Action.TargetType.ALL_ENEMIES:
                {
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (targetting)
                        {
                            if (!unit.IsPlayerControlled)
                            {
                                unit.IsHighlighted = true;
                                if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) < 1) // Not Effective
                                {
                                    unit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", new Color(0.5754717f * 255, 0.4533197f * 255, 0.4533197f * 255) * 0.02f);
                                }
                                else if (CombatTools.ReturnTypeMultiplier(unit, action.damageType) > 1) //Effective
                                {
                                    unit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", new Color(255, 1, 0) * 0.02f);
                                }
                            }
                            else
                            {
                                unit.isDarkened = true;
                            }
                        }
                        else
                        {
                            unit.IsHighlighted = false;
                            unit.isDarkened = false;
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
                            Director.Instance.StartCoroutine(AutoSelectNextAvailableUnit());
                            BattleLog.Instance.ResetBattleLog();
                            LabCamera.Instance.ResetPosition();
                            AudioManager.QuickPlay("button_Hit_005");
                            SetActive(false);
                        }
                    }

                }
                break;

        }
    }

    void UpdateActionStyleOutline()
    {
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
                    this.GetComponent<Image>().material.SetColor("OutlineColor", heavyColor * 10);
                    baseUnit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", heavyColor * 0.02f);
                }
                break;
            case Action.ActionStyle.LIGHT:
                {
                    Color lightColor = new Color(0, 162, 191);
                    this.GetComponent<Image>().material.SetFloat("OutlineThickness", 1);
                    this.GetComponent<Image>().material.SetColor("OutlineColor", lightColor * 10);
                    baseUnit.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", lightColor * 0.02f);
                }
                break;
        }
    }

    void PerformOnActionSelected()
    {
        if (!HasDoneOnAction)
        {
            baseUnit.DoOnActionSelected(this);
            HasDoneOnAction = true;
        }
    }

    void ResetUIState()
    {
        if (damageNums != null)
            damageNums.color = new Color(1, 0.8705882f, 0.7058824f);

        GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
        GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
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

    public void CreateTempTimeLineChild(Unit TargetUnit)
    {
        CreatedTempTimelineChild = true;
        TempTL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
        TempTL.unit = TargetUnit;
        TempTL.portrait.sprite = TargetUnit.charPortraits[0];
        Director.Instance.timeline.children.Add(TempTL);
        TempTL.CanMove = false;
        float costToReturn = 0;
        if (TargetUnit != null)
            costToReturn = Director.Instance.timeline.ReturnTimeChildAction(TargetUnit).cost;

        if (CombatTools.ReturnTypeMultiplier(TargetUnit, action.damageType) > 1)
            costToReturn += Director.Instance.TimelineReduction;

        if (action.actionStyle != Action.ActionStyle.STANDARD)
            costToReturn += 10;

        if (costToReturn > 100 || action.AppliesStun)
            costToReturn = 100;

        if (TargetUnit.IsPlayerControlled)
            TempTL.rectTransform.anchoredPosition = new Vector3((100 - costToReturn) * TempTL.offset, 50);
        else
            TempTL.rectTransform.anchoredPosition = new Vector3((100 - costToReturn) * TempTL.offset, -50);

        TempTL.staminaText.text = (100 - costToReturn).ToString();
        TempTL.CanClear = true;
        TempTL.GetComponent<LabUIInteractable>().CanHover = false;
        TempTL.CanBeHighlighted = false;
        TempTL.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
        TempTL.portrait.color = new Color(1, 1, 1, 0.5f);
    }

    public IEnumerator lightCoroutine;
    public void SetStyleLight(bool TurnOn)
    {
        if (TurnOn)
        {
            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = TurnOnLight(0.01f);
            if (this != null)
                Director.Instance.StartCoroutine(lightCoroutine);

        }
        else
        {
            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = TurnOffLight(0.01f);
            if (this != null)
                Director.Instance.StartCoroutine(lightCoroutine);
        }
    }

    private IEnumerator TurnOffLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance.mainLight != null)
        {
            while (BattleSystem.Instance.mainLight.intensity != 0 && action.actionStyle != Action.ActionStyle.STANDARD)
            {
                BattleSystem.Instance.mainLight.intensity -= 0.01f;
                yield return new WaitForSeconds(delay);
            }
        }
    }
    private IEnumerator TurnOnLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance.mainLight != null)
        {
            while (BattleSystem.Instance.mainLight.intensity < BattleSystem.Instance.mainLightValue && action.actionStyle == Action.ActionStyle.STANDARD)
            {
                BattleSystem.Instance.mainLight.intensity += 0.01f;
                yield return new WaitForSeconds(delay);
            }
        }
    }
    public void SetDescription()
    {
        if (isActiveAndEnabled)
        {
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
            // currentEffectPopup.GetComponent<EffectPopUp>().CheckForSpecialText();

            if (limited)
            {
                EPtext.text += $"\nUses: {numberofUses}.";
            }
            if (!action.CanBeStyled)
            {
                EPtext.text += "<color=#FF0000>\nCan't be styled.</color>";
            }

            if (BattleSystem.Instance != null && BattleSystem.Instance.state != BattleStates.WON)
            {
                currentEffectPopup.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(rectTrans.anchoredPosition.x - 400, rectTrans.anchoredPosition.y - 210, 0);
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
        AudioManager.Instance.Stop("statUp_Loop_001");
        action.actionStyle = Action.ActionStyle.STANDARD;
        //action.ResetAction();
        if (baseUnit != null && baseUnit.GetComponentInChildren<Light>().intensity < 1.1)
        {
            var Light = baseUnit.GetComponentInChildren<Light>();
            baseUnit.ChangeUnitsLight(Light, 0, 0, Light.color, 0f, 0);
        }

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
        if (Director.Instance.UnlockedPipSystem && !BattleSystem.Instance.enemyUnits.Where(obj => obj.unitName.Contains("Dusty")).SingleOrDefault())
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
            Director.Instance.StartCoroutine(CombatTools.StopAndDestroyVFX(0.01f));
            lightButton.gameObject.SetActive(false);
            heavyButton.gameObject.SetActive(false);
        }
    }
    public void SetActive(bool turnOn)
    {
        if (!Director.Instance.timeline.gameObject.activeSelf)
            return;

        damageNums.color = new Color(1, 0.8705882f, 0.7058824f);

        if (BattleSystem.Instance != null && baseUnit != null)
        {
            if (targetting || !turnOn)
                DeactivateAction();
            else
                ActivateAction();
        }
        else
        {
            SetDescription();
            HasDoneOnAction = false;
        }
    }

    void DeactivateAction()
    {
        targetting = false;
        SetActionStyleButtonsActive(false);
        SetStyleLight(true);
        LabCamera.Instance.ResetPosition();
        RemoveDescription();
        ResetAllStyleButtons();

        if (action.targetType == Action.TargetType.ENEMY && action.actionType == Action.ActionType.ATTACK)
        {
            damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + baseUnit.attackStat).ToString();
            damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
        }

        costNums.text = CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier < 100 ? $"{CombatTools.DetermineTrueCost(action) * baseUnit.actionCostMultiplier}%" : $"100%";

        UpdateButtonInteractability(GetComponent<Button>());

        foreach (var unit in Tools.GetAllUnits())
        {
            unit.IsHighlighted = false;
            unit.isDarkened = false;
        }

        ClearTimelineChildren();
    }

    void ActivateAction()
    {
        SetActionStyleButtonsActive(false);

        DeactivateOtherActionContainers();

        ClearTimelineChildren();

        LabCamera.Instance.ResetPosition();
        RemoveDescription();

        TL = InstantiateTimelineChild();
        SetTimelineChildProperties();

        targetting = true;
        SetDescription();
        SetButtonInteractable(false);

        ScaleObject();

        SetActionStyleButtonsActive(CombatTools.ReturnPipCounter().pipCount > 0);

        AudioManager.QuickPlay("button_Hit_001", true);
        HasDoneOnAction = false;
    }

    void UpdateButtonInteractability(Button button)
    {
        if (!Disabled)
        {
            button.interactable = limited ? (numberofUses > 0) : true;
        }
    }

    void DeactivateOtherActionContainers()
    {
        ActionContainer[] actionContainers = UnityEngine.Object.FindObjectsOfType<ActionContainer>();
        foreach (var x in actionContainers)
        {
            if (x != this)
            {
                x.SetActive(false);
                UpdateButtonInteractability(x.GetComponent<Button>());
            }
        }
    }

    void ClearTimelineChildren()
    {
        foreach (TimeLineChild child in Director.Instance.timeline.children.ToList())
        {
            if (child != null && child.CanClear)
            {
                Director.Instance.timeline.children.Remove(child);
                Destroy(child.gameObject);
                break;
            }
        }
    }

    TimeLineChild InstantiateTimelineChild()
    {
        TL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
        TL.unit = baseUnit;
        TL.portrait.sprite = baseUnit.charPortraits[0];
        Director.Instance.timeline.children.Add(TL);
        TL.CanMove = false;
        TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);
        TL.rectTransform.anchoredPosition = new Vector3((100 - CombatTools.DetermineTrueCost(action)) * TL.offset, 50);
        TL.staminaText.text = (100 - CombatTools.DetermineTrueCost(action)).ToString();
        TL.CanClear = true;
        TL.GetComponent<LabUIInteractable>().CanHover = false;
        TL.CanBeHighlighted = false;
        TL.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
        TL.portrait.color = new Color(1, 1, 1, 0.5f);
        return TL;
    }

    void SetTimelineChildProperties()
    {
        TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);
        TL.rectTransform.anchoredPosition = new Vector3((100 - CombatTools.DetermineTrueCost(action)) * TL.offset, 50);
        TL.staminaText.text = (100 - CombatTools.DetermineTrueCost(action)).ToString();
        TL.CanClear = true;
        TL.GetComponent<LabUIInteractable>().CanHover = false;
        TL.CanBeHighlighted = false;
        TL.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
        TL.portrait.color = new Color(1, 1, 1, 0.5f);
    }

    void ScaleObject()
    {
        transform.localScale = GetComponent<ScalableObject>().oldScaleSize;
    }

    void SetButtonInteractable(bool interactable)
    {
        var button = GetComponent<Button>();
        button.interactable = interactable;
    }

}


