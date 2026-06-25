
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private Unit currentPreviewTarget;
    private Unit subscribedUnit;

    private static readonly Color NeutralDamageColor = new Color(1f, 0.8705882f, 0.7058824f);

    private static readonly Color DefaultUnitOutlineColor = Color.clear;

    private static readonly Color ControlledUnitOutlineColor = Color.yellow;

    private static readonly Color ValidTargetOutlineColor = Color.white;

    private static readonly Color ResistedTargetOutlineColor = new Color(0.5754717f * 255f, 0.4533197f * 255f, 0.4533197f * 255f) * 0.02f;

    private static readonly Color EffectiveTargetOutlineColor = new Color(255f, 1f, 0f) * 0.02f;
    private static readonly Color NewActionOutlineColor = Color.white;

    private static readonly Color heavyColor = new Color(255f, 1f, 0f) * 0.05f;
    private static readonly Color lightColor = new Color(0, 162, 191) * 0.05f;


    private void Awake()
    {
        if (BattleSystem.Instance != null &&
            button != null &&
            !Disabled)
        {
            button.interactable = true;

            foreach (Unit unit in Tools.GetAllUnits())
            {
                if (unit == null)
                    continue;

                ResetUnitHighlight(unit);
            }
        }

        if (action != null)
        {
            numberofUses = action.numberofUses;
            limited = action.limited;
        }

        Image image = GetComponent<Image>();

        if (image != null && image.material != null)
        {
            image.material = Instantiate(image.material);
        }

        ScalableObject scaleComponent =
            GetComponent<ScalableObject>();

        if (scaleComponent != null)
        {
            scaleComponent.oldScaleSize = Vector3.one;

            scaleComponent.newScaleSize =
                scaleComponent.oldScaleSize * 1.02f;
        }

        ApplyDefaultActionOutline();
    }

    private void Start()
    {

        if (baseUnit == null || action == null)
            return;

        action.unit = baseUnit;

        SubscribeToUnit();
        RefreshActionValues();
        ApplyDefaultActionOutline();
    }

    private void OnDestroy()
    {
        UnsubscribeFromUnit();
    }

    public void Initialize(Unit unit, Action assignedAction)
    {
        UnsubscribeFromUnit();

        baseUnit = unit;
        action = assignedAction;

        if (baseUnit == null || action == null)
        {
            Debug.LogError(
                $"ActionContainer {name} could not be initialized.",
                this
            );

            return;
        }

        action.unit = baseUnit;

        numberofUses = action.numberofUses;
        limited = action.limited;

        SubscribeToUnit();
        RefreshActionValues();
        ApplyDefaultActionOutline();
    }

    private void SubscribeToUnit()
    {
        if (baseUnit == null)
            return;

        if (subscribedUnit == baseUnit)
            return;

        UnsubscribeFromUnit();

        subscribedUnit = baseUnit;

        subscribedUnit.OnActionModifiersChanged -=
            RefreshActionValues;

        subscribedUnit.OnActionModifiersChanged +=
            RefreshActionValues;
    }

    private void UnsubscribeFromUnit()
    {
        if (subscribedUnit == null)
            return;

        subscribedUnit.OnActionModifiersChanged -=
            RefreshActionValues;

        subscribedUnit = null;
    }

    private void Update()
    {
        if (!targetting ||
            BattleSystem.Instance == null ||
            BattleSystem.Instance.state == BattleStates.WON)
        {
            ResetUIState();
            return;
        }

        if (HandleActionCancel())
            return;

        RaycastHit hit = Tools.GetMousePos();

        UpdateDamageNumbers(hit);
        UpdateCostNumbers();
        UpdateTargetHighlights();
        ExecuteActionOnClick(hit);
        UpdateActionStyleOutline();
        PerformOnActionSelected();
    }



    private void ApplyDefaultActionOutline()
    {
        Image image = GetComponent<Image>();

        if (image == null || image.material == null)
            return;

        if (action != null && action.New)
        {
            image.material.SetFloat(
                "OutlineThickness",
                1f
            );

            image.material.SetColor(
                "OutlineColor",
               NewActionOutlineColor * 100
            );
        }
        else
        {
            image.material.SetFloat(
                "OutlineThickness",
                0f
            );

            image.material.SetColor(
                "OutlineColor",
                Color.white
            );
        }
    }

    private void UpdateDamageDisplay(Unit target = null)
    {
        if (damageNums == null ||
            action == null ||
            baseUnit == null)
        {
            return;
        }

        action.unit = baseUnit;

        int baseDamage =
            CombatTools.DetermineTrueActionValue(action) +
            baseUnit.attackStat;

        float typeMultiplier =
            target != null
                ? CombatTools.ReturnTypeMultiplier(
                    target,
                    action.damageType
                )
                : 1f;

        float modifiedDamage =
            (baseDamage + baseUnit.damageAddend) *
            baseUnit.DamageModifier *
            typeMultiplier;

        int finalDamage =
            Mathf.Max(
                0,
                Mathf.RoundToInt(modifiedDamage)
            );

        damageNums.text =
            $"<sprite name=\"{action.damageType}\">{finalDamage}";

        if (target == null)
        {
            damageNums.color = NeutralDamageColor;
        }
        else if (typeMultiplier < 1f)
        {
            damageNums.color = Color.red;
        }
        else if (typeMultiplier > 1f)
        {
            damageNums.color = Color.green;
        }
        else
        {
            damageNums.color = NeutralDamageColor;
        }
    }



    private Unit GetUnitFromHit(RaycastHit hit)
    {
        if (hit.collider == null)
            return null;

        return hit.collider.GetComponentInParent<Unit>();
    }

    private Unit GetEnemyTargetFromHit(RaycastHit hit)
    {
        Unit targetUnit = GetUnitFromHit(hit);

        if (targetUnit == null)
            return null;

        if (targetUnit.IsPlayerControlled)
            return null;

        return targetUnit;
    }

    private void UpdateDamageNumbers(RaycastHit hit)
    {
        if (action == null ||
            action.actionType != Action.ActionType.ATTACK)
        {
            return;
        }

        bool targetsEnemies =
            action.targetType == Action.TargetType.ENEMY ||
            action.targetType == Action.TargetType.ALL_ENEMIES;

        if (!targetsEnemies)
        {
            currentPreviewTarget = null;
            UpdateDamageDisplay();
            return;
        }

        Unit targetUnit = GetEnemyTargetFromHit(hit);

        if (targetUnit != null)
        {
            currentPreviewTarget = targetUnit;

            UpdateDamageDisplay(currentPreviewTarget);
            UpdateTempTimelineChildIfNeeded(currentPreviewTarget);
        }
        else
        {
            currentPreviewTarget = null;

            UpdateDamageDisplay();
            DestroyTempTimelineChildIfNeeded();
        }
    }

    private void UpdateCostNumbers()
    {
        if (costNums == null || action == null)
            return;

        action.unit = baseUnit;

        float finalCost =
            Mathf.Clamp(
                CombatTools.DetermineTrueCost(action),
                0f,
                100f
            );

        costNums.text =
            $"{Mathf.RoundToInt(finalCost)}%";
    }

    private bool HandleActionCancel()
    {
        if (!Input.GetMouseButtonUp(1))
            return false;

        /*
         * SetActive(false) calls DeactivateAction(), which clears all
         * targeting visuals and resets the selected action.
         */
        SetActive(false);
        LabCamera.Instance.MoveToUnit(CombatTools.FindDecidingUnit(), new Vector3(0, 16.8f, 0), CombatTools.FindDecidingUnit().GetComponent<SpriteRenderer>().sprite.bounds.center.x / 5f, 0, 0);
        AudioManager.QuickPlay("ui_woosh_002");
        SetStyleLight(true);
        return true;
    }

    private void UpdateTargetHighlights()
    {
        foreach (Unit unit in Tools.GetAllUnits())
        {
            if (unit == null)
                continue;

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

    private void UpdateUnitHighlight(Unit unit)
    {
        if (unit == null ||
            action == null ||
            baseUnit == null)
        {
            return;
        }

        SpriteRenderer renderer =
            unit.GetComponent<SpriteRenderer>();

        /*
         * The controlled unit remains yellow whenever they are
         * choosing another target.
         */
        if (unit == baseUnit &&
     action.targetType != Action.TargetType.SELF)
        {
            unit.IsHighlighted = true;
            unit.isDarkened = false;

            switch (action.actionStyle)
            {
                case Action.ActionStyle.LIGHT:
                    {
                        SetOutlineColor(
                            renderer,
                            lightColor
                        );

                        break;
                    }

                case Action.ActionStyle.HEAVY:
                    {
                        SetOutlineColor(
                            renderer,
                            heavyColor
                        );

                        break;
                    }

                case Action.ActionStyle.STANDARD:
                default:
                    {
                        SetOutlineColor(
                            renderer,
                            ControlledUnitOutlineColor
                        );

                        break;
                    }
            }

            return;
        }


        switch (action.targetType)
        {
            case Action.TargetType.ENEMY:
            case Action.TargetType.ALL_ENEMIES:
                {
                    if (!unit.IsPlayerControlled)
                    {
                        SetUnitAsEnemyTarget(unit, renderer);
                    }
                    else
                    {
                        SetUnitAsInvalidTarget(unit, renderer);
                    }

                    break;
                }

            case Action.TargetType.ALLY:
                {
                    if (unit.IsPlayerControlled)
                    {
                        SetUnitAsNeutralTarget(unit, renderer);
                    }
                    else
                    {
                        SetUnitAsInvalidTarget(unit, renderer);
                    }

                    break;
                }

            case Action.TargetType.SELF:
                {
                    if (unit == baseUnit)
                    {
                        /*
                         * For a self-targeted action, the controlled unit is
                         * the target, so it uses the normal white target color.
                         */
                        SetUnitAsNeutralTarget(unit, renderer);
                    }
                    else
                    {
                        SetUnitAsInvalidTarget(unit, renderer);
                    }

                    break;
                }

            default:
                {
                    ResetUnitHighlight(unit);
                    break;
                }
        }
    }

    private void SetUnitAsEnemyTarget(
    Unit unit,
    SpriteRenderer renderer)
    {
        unit.IsHighlighted = true;
        unit.isDarkened = false;

        float multiplier =
            CombatTools.ReturnTypeMultiplier(
                unit,
                action.damageType
            );

        if (multiplier < 1f)
        {
            SetOutlineColor(
                renderer,
                ResistedTargetOutlineColor
            );
        }
        else if (multiplier > 1f)
        {
            SetOutlineColor(
                renderer,
                EffectiveTargetOutlineColor
            );
        }
        else
        {
            SetOutlineColor(
                renderer,
                ValidTargetOutlineColor
            );
        }
    }

    private void SetUnitAsNeutralTarget(
        Unit unit,
        SpriteRenderer renderer)
    {
        unit.IsHighlighted = true;
        unit.isDarkened = false;

        SetOutlineColor(
            renderer,
            ValidTargetOutlineColor
        );
    }

    private void SetUnitAsInvalidTarget(
        Unit unit,
        SpriteRenderer renderer)
    {
        unit.IsHighlighted = false;
        unit.isDarkened = true;

        ResetOutlineColor(renderer);
    }



    private void SetOutlineColor(SpriteRenderer renderer, Color color)
    {
        if (renderer == null)
            return;

        renderer.material.SetColor("_OutlineColor", color);
    }

    private void ResetOutlineColor(
        SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.material.SetColor(
            "_OutlineColor",
            DefaultUnitOutlineColor
        );
    }

    private void ResetUnitHighlight(Unit unit)
    {
        if (unit == null)
            return;

        unit.IsHighlighted = false;
        unit.isDarkened = false;

        SpriteRenderer spriteRenderer =
            unit.GetComponent<SpriteRenderer>();

        ResetOutlineColor(spriteRenderer);
    }

    private void ClearAllTargetingVisuals()
    {
        foreach (Unit unit in Tools.GetAllUnits())
        {
            ResetUnitHighlight(unit);
        }
    }

    private void UpdateTempTimelineChildIfNeeded(
        Unit targetUnit)
    {
        bool effectiveHit =
            CombatTools.ReturnTypeMultiplier(
                targetUnit,
                action.damageType
            ) > 1f;

        if (effectiveHit ||
            action.actionStyle !=
                Action.ActionStyle.STANDARD ||
            action.AppliesStun)
        {
            UpdateTempTimelineChild(targetUnit);
        }
        else
        {
            DestroyTempTimelineChildIfNeeded();
        }
    }

    private void DestroyTempTimelineChildIfNeeded()
    {
        if (!CreatedTempTimelineChild)
            return;

        if (TempTL != null)
        {
            Director.Instance.timeline.children.Remove(TempTL);
            Destroy(TempTL.gameObject);
        }

        TempTL = null;
        CreatedTempTimelineChild = false;
    }

    private void UpdateTempTimelineChild(Unit targetUnit)
    {
        if (CreatedTempTimelineChild)
            return;

        Action targetAction =
            Director.Instance.timeline
                .ReturnTimeChildAction(targetUnit);

        if (targetAction == null)
            return;

        bool canPreview =
            CombatTools.ReturnIconStatus(
                targetUnit,
                "INDOMITABLE"
            );

        if (!canPreview)
            return;

        if (CombatTools.DetermineTrueCost(targetAction) <
            CombatTools.DetermineTrueCost(action))
        {
            return;
        }

        CreateTempTimeLineChild(targetUnit);
    }

    private void ExecuteActionOnClick(RaycastHit hit)
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        Unit clickedUnit = GetUnitFromHit(hit);

        switch (action.targetType)
        {
            case Action.TargetType.ENEMY:
                {
                    if (clickedUnit != null &&
                        clickedUnit != baseUnit &&
                        !clickedUnit.IsPlayerControlled)
                    {
                        QueueSelectedAction(clickedUnit);
                    }

                    break;
                }

            case Action.TargetType.SELF:
                {
                    if (clickedUnit == baseUnit)
                    {
                        QueueSelectedAction(baseUnit);
                    }

                    break;
                }

            case Action.TargetType.ALLY:
                {
                    if (clickedUnit != null &&
                        clickedUnit.IsPlayerControlled)
                    {
                        QueueSelectedAction(clickedUnit);
                    }

                    break;
                }

            case Action.TargetType.ALL_ENEMIES:
                {
                    if (clickedUnit != null &&
                        clickedUnit != baseUnit &&
                        !clickedUnit.IsPlayerControlled)
                    {
                        QueueSelectedAction(clickedUnit);
                    }

                    break;
                }
        }
    }

    private void QueueSelectedAction(Unit target)
    {
        if (baseUnit == null ||
            action == null ||
            target == null)
        {
            return;
        }

        /*
         * Clear visual state before the next unit starts deciding.
         */
        targetting = false;
        currentPreviewTarget = null;
        ClearAllTargetingVisuals();

        baseUnit.state = PlayerState.READY;

        action.targets = target;
        action.unit = baseUnit;

        baseUnit.Queue(action);

        if (baseUnit.timelinechild != null)
        {
            baseUnit.timelinechild.CanMove = true;
        }

        Director.Instance.StartCoroutine(
            AutoSelectNextAvailableUnit()
        );

        BattleLog.Instance.ResetBattleLog();

        ClearTimelineChildren();
        DestroyTempTimelineChildIfNeeded();

        LabCamera.Instance.ResetPosition();

        SetStyleLight(true);

        AudioManager.QuickPlay("button_Hit_005");

        SetActive(false);
    }

    public void UpdateActionStyleOutline()
    {
        Image image = GetComponent<Image>();

        if (image == null ||
            image.material == null ||
            action == null)
        {
            return;
        }

        switch (action.actionStyle)
        {
            case Action.ActionStyle.STANDARD:
                {
                    ApplyDefaultActionOutline();
                    break;
                }

            case Action.ActionStyle.HEAVY:
                {
                    Color heavyOutlineColor =
                        new Color(225f, 1f, 0f);

                    image.material.SetFloat(
                        "OutlineThickness",
                        1f
                    );

                    image.material.SetColor(
                        "OutlineColor",
                        heavyOutlineColor * 10f
                    );

                    break;
                }

            case Action.ActionStyle.LIGHT:
                {
                    Color lightOutlineColor =
                        new Color(0f, 162f, 191f);

                    image.material.SetFloat(
                        "OutlineThickness",
                        1f
                    );

                    image.material.SetColor(
                        "OutlineColor",
                        lightOutlineColor * 10f
                    );

                    break;
                }
        }
    }

    //Sets the outline color of the action container 
    public void SetOutline(Color outlineColor, float outlineThickness, float colorAlpha)
    {
        Image image = GetComponent<Image>();

        if (image == null || image.material == null)
            return;

        image.material.SetFloat(
            "OutlineThickness",
            outlineThickness
        );

        image.material.SetColor(
            "OutlineColor",
            outlineColor * colorAlpha
        );
    }

    private void PerformOnActionSelected()
    {
        if (HasDoneOnAction ||
            baseUnit == null)
        {
            return;
        }

        baseUnit.DoOnActionSelected(this);
        HasDoneOnAction = true;
    }

    public void ResetUIState()
    {
        if (damageNums != null)
        {
            damageNums.color =
                NeutralDamageColor;
        }

        ApplyDefaultActionOutline();
    }

    public IEnumerator AutoSelectNextAvailableUnit()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (Unit playerUnit
                 in BattleSystem.Instance.playerUnits)
        {
            if (playerUnit == null)
                continue;

            if (playerUnit.state != PlayerState.READY)
            {
                playerUnit.StartDecision();
                break;
            }
        }
    }

    public void CreateTempTimeLineChild(Unit targetUnit)
    {
        if (targetUnit == null)
            return;

        CreatedTempTimelineChild = true;

        TempTL = Instantiate(
            Director.Instance.timeline.borderChildprefab,
            Director.Instance.timeline.startpoint
        );

        TempTL.unit = targetUnit;

        if (TempTL.portrait != null &&
            targetUnit.charPortraits != null &&
            targetUnit.charPortraits.Count > 0)
        {
            TempTL.portrait.sprite =
                targetUnit.charPortraits[0];
        }

        Director.Instance.timeline.children.Add(TempTL);

        TempTL.CanMove = false;

        float costToReturn = 0f;

        Action targetAction =
            Director.Instance.timeline
                .ReturnTimeChildAction(targetUnit);

        if (targetAction != null)
        {
            costToReturn =
                CombatTools.DetermineTrueCost(targetAction);
        }

        bool effectiveHit =
            CombatTools.ReturnTypeMultiplier(
                targetUnit,
                action.damageType
            ) > 1f;

        if (effectiveHit)
        {
            costToReturn +=
                Director.Instance.TimelineAddition;

            costToReturn +=
                targetUnit.knockbackModifider;
        }

        if (action.actionStyle !=
            Action.ActionStyle.STANDARD)
        {
            costToReturn +=
                Director.Instance
                    .TimelineReductionNonStandardAction;
        }

        if (costToReturn > 100f ||
            action.AppliesStun)
        {
            costToReturn = 100f;
        }

        float verticalPosition =
            targetUnit.IsPlayerControlled
                ? 50f
                : -50f;

        TempTL.rectTransform.anchoredPosition =
            new Vector3(
                (100f - costToReturn) * TempTL.offset,
                verticalPosition
            );

        TempTL.staminaText.text =
            Mathf.RoundToInt(costToReturn).ToString();

        TempTL.CanClear = true;

        LabUIInteractable interactable =
            TempTL.GetComponent<LabUIInteractable>();

        if (interactable != null)
        {
            interactable.CanHover = false;
        }

        TempTL.CanBeHighlighted = false;

        Image timelineImage =
            TempTL.GetComponentInChildren<Image>();

        if (timelineImage != null)
        {
            timelineImage.color =
                new Color(1f, 1f, 1f, 0.5f);
        }

        if (TempTL.portrait != null)
        {
            TempTL.portrait.color =
                new Color(1f, 1f, 1f, 0.5f);
        }
    }

    public IEnumerator lightCoroutine;

    public void SetStyleLight(bool TurnOn)
    {
        if (TurnOn)
        {
            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = TurnOnLight(10);
            if (this != null)
                Director.Instance.StartCoroutine(lightCoroutine);

        }
        else
        {
            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = TurnOffLight(10);
            if (this != null)
                Director.Instance.StartCoroutine(lightCoroutine);
        }
    }

    private IEnumerator TurnOffLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance != null && BattleSystem.Instance.lablights != null)
        {
            foreach (var light in BattleSystem.Instance.lablights.ToList())
            {
                float currentIntensity = light.lightComponent.intensity;
                float startIntensity = light.startIntensity;
                float currentTime = 0;
                float TargetIntensity = startIntensity / 22.5f;
                while (light.lightComponent.intensity > TargetIntensity && action.actionStyle != Action.ActionStyle.STANDARD)
                {
                    currentTime += Time.deltaTime * delay;
                    light.lightComponent.intensity = Mathf.Lerp(currentIntensity, TargetIntensity, currentTime);
                    yield return null;
                }
            }
        }
    }
    private IEnumerator TurnOnLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance != null && BattleSystem.Instance.lablights != null)
        {
            foreach (var light in BattleSystem.Instance.lablights.ToList())
            {
                float currentIntensity = light.lightComponent.intensity;
                float startIntensity = light.startIntensity;
                float currentTime = 0;
                float TargetIntensity = startIntensity;
                while (light.lightComponent.intensity < TargetIntensity && action.actionStyle == Action.ActionStyle.STANDARD)
                {
                    currentTime += Time.deltaTime * delay;
                    light.lightComponent.intensity = Mathf.Lerp(currentIntensity, TargetIntensity, currentTime);
                    yield return null;
                }
            }
        }
    }

    public void SetDescription()
    {
        if (!isActiveAndEnabled ||
            action == null ||
            baseUnit == null)
        {
            return;
        }

        RectTransform rectTransform =
            GetComponent<RectTransform>();

        action.unit = baseUnit;

        if (currentEffectPopup == null)
        {
            currentEffectPopup = Instantiate(
                Director.Instance.EffectPopUp,
                transform
            );

            currentEffectPopup.transform.localScale =
                new Vector3(1.5f, 1.5f, 1.5f);
        }
        else
        {
            currentEffectPopup.SetActive(true);
        }

        TextMeshProUGUI popupText =
            currentEffectPopup
                .GetComponentInChildren<TextMeshProUGUI>();

        popupText.text =
            $"{action.ReturnActionType()}\n" +
            $"{action.GetDescription()}";

        if (limited)
        {
            popupText.text +=
                $"\nUses: {numberofUses}.";
        }

        if (!action.CanBeStyled)
        {
            popupText.text +=
                "<color=#FF0000>\n" +
                "Can't be ignited.</color>";
        }

        RectTransform popupRect =
            currentEffectPopup
                .GetComponent<RectTransform>();

        if (BattleSystem.Instance != null &&
            BattleSystem.Instance.state !=
                BattleStates.WON)
        {
            popupRect.anchoredPosition =
                new Vector3(
                    rectTransform.anchoredPosition.x,
                    rectTransform.anchorMax.y +
                    75f +
                    popupRect.rect.width /
                    (popupRect.rect.width / 2f),
                    0f
                );

            currentEffectPopup.transform
                .SetAsLastSibling();
        }
        else
        {
            currentEffectPopup.transform.localScale =
                new Vector3(0.02f, 0.02f, 1f);

            popupRect.anchoredPosition =
                new Vector3(0f, 1f, 0f);

            currentEffectPopup.transform
                .SetAsLastSibling();
        }

        Director.Instance.StartCoroutine(
            Tools.UpdateParentLayoutGroup(
                popupText.gameObject
            )
        );
    }

    public void RemoveDescription()
    {
        if (currentEffectPopup != null)
        {
            currentEffectPopup.SetActive(false);
        }
    }

    public void ResetAllStyleButtons(
        bool turnOn = false)
    {
        if (action.actionStyle !=
                Action.ActionStyle.STANDARD &&
            baseUnit.state == PlayerState.DECIDING)
        {
            CombatTools.ReturnPipCounter().AddPip();
        }

        AudioManager.Instance.Stop(
            "statUp_Loop_001"
        );

        action.actionStyle =
            Action.ActionStyle.STANDARD;

        /*
         * Clear the action-style outline before restoring lights.
         */
        if (baseUnit != null)
        {
            SpriteRenderer renderer =
                baseUnit.GetComponent<SpriteRenderer>();

            ResetOutlineColor(renderer);
        }

        Light unitLight =
            baseUnit != null
                ? baseUnit.GetComponentInChildren<Light>()
                : null;

        if (unitLight != null &&
            unitLight.intensity < 1.1f)
        {
            baseUnit.ChangeUnitsLight(
                unitLight,
                0f,
                0f,
                unitLight.color,
                0f,
                0f
            );
        }

        if (lightButton != null)
        {
            lightButton.state =
                ActionTypeButton
                    .ActionButtonState.LIGHT;
        }

        if (heavyButton != null)
        {
            heavyButton.state =
                ActionTypeButton
                    .ActionButtonState.HEAVY;
        }

        SetActionStyleButtonsActive(turnOn);
        RefreshActionValues();
        ApplyDefaultActionOutline();
    }

    public void SetActionStyle(Action.ActionStyle newStyle)
    {
        if (action == null || baseUnit == null)
            return;

        /*
         * ActionContainer already owns a runtime action instance.
         * Mutating that instance avoids losing the Unit reference
         * and its action-cost modifiers when changing styles.
         */
        action.unit = baseUnit;
        action.actionStyle = newStyle;

        UpdateOnStyleSwitch();
    }

    public void UpdateOnStyleSwitch()
    {
        if (action == null || baseUnit == null)
            return;

        /*
         * Rebind before every cost calculation. DetermineTrueCost()
         * reads actionCostMultiplier/actionCostAddend from action.unit.
         */
        action.unit = baseUnit;

        RefreshActionValues();

        if (targetting)
        {
            UpdateTargetHighlights();
        }

        UpdateActionStyleOutline();
        SetDescription();
    }

    public void SetActionStyleButtonsActive(
        bool setActive)
    {
        if (BattleSystem.Instance == null ||
            action == null)
        {
            return;
        }

        bool hasDusty =
            BattleSystem.Instance.enemyUnits.Any(
                enemy =>
                    enemy != null &&
                    enemy.unitName.Contains("Dusty")
            );

        if (Director.Instance.UnlockedPipSystem &&
            !hasDusty)
        {
            if (action.CanBeStyled)
            {
                if (lightButton != null)
                {
                    lightButton.gameObject.SetActive(
                        setActive
                    );

                    lightButton.interactable = true;
                }

                if (heavyButton != null)
                {
                    heavyButton.gameObject.SetActive(
                        setActive
                    );

                    heavyButton.interactable = true;
                }
            }

            Director.Instance.StartCoroutine(
                CombatTools.StopAndDestroyVFX(
                    0.01f
                )
            );
        }
        else
        {
            Director.Instance.StartCoroutine(
                CombatTools.StopAndDestroyVFX(
                    0.01f
                )
            );

            if (lightButton != null)
            {
                lightButton.gameObject.SetActive(false);
            }

            if (heavyButton != null)
            {
                heavyButton.gameObject.SetActive(false);
            }
        }
    }

    public void SetActive(bool turnOn)
    {
        if (!Director.Instance.timeline
                .gameObject.activeSelf)
        {
            return;
        }

        if (damageNums != null)
        {
            damageNums.color =
                NeutralDamageColor;
        }

        if (BattleSystem.Instance != null &&
            baseUnit != null)
        {
            if (targetting || !turnOn)
            {
                DeactivateAction();
            }
            else
            {
                ActivateAction();
            }
        }
        else
        {
            SetDescription();
            HasDoneOnAction = false;
        }
    }

    private void DeactivateAction()
    {
        /*
         * Set state and clear visuals first so nothing later in the
         * method can leave an old target highlighted.
         */
        targetting = false;
        currentPreviewTarget = null;

        ClearAllTargetingVisuals();

        SetActionStyleButtonsActive(false);
        SetStyleLight(true);

        LabCamera.Instance.ResetPosition();

        Unit decidingUnit =
            CombatTools.FindDecidingUnit();

        if (decidingUnit != null)
        {
            SpriteRenderer decidingRenderer =
                decidingUnit.GetComponent<SpriteRenderer>();

            float cameraOffset = 0f;

            if (decidingRenderer != null &&
                decidingRenderer.sprite != null)
            {
                cameraOffset =
                    decidingRenderer.sprite
                        .bounds.center.x / 5f;
            }

            LabCamera.Instance.MoveToUnit(
                decidingUnit,
                new Vector3(0f, 16.8f, 0f),
                cameraOffset,
                0f,
                0f
            );
        }

        RemoveDescription();
        ResetAllStyleButtons();
        RefreshActionValues();

        UpdateButtonInteractability(button);

        ClearTimelineChildren();
        DestroyTempTimelineChildIfNeeded();
    }

    private void ActivateAction()
    {
        if (baseUnit == null || action == null)
            return;

        action.unit = baseUnit;

        SubscribeToUnit();

        SetActionStyleButtonsActive(false);

        DeactivateOtherActionContainers();
        ClearTimelineChildren();

        /*
         * Ensure no visual state remains from the previously
         * selected action before applying this action's targets.
         */
        ClearAllTargetingVisuals();

        SetStyleLight(true);

        LabCamera.Instance.ResetPosition();

        RemoveDescription();

        TL = InstantiateTimelineChild();
        SetTimelineChildProperties();

        targetting = true;

        RefreshActionValues();
        SetDescription();

        SetButtonInteractable(false);

        ScaleObject();

        SetActionStyleButtonsActive(
            CombatTools.ReturnPipCounter()
                .pipCount > 0
        );

        AudioManager.QuickPlay(
            "button_Hit_001",
            true
        );

        HasDoneOnAction = false;
    }

    private void UpdateButtonInteractability(
        Button targetButton)
    {
        if (targetButton == null)
            return;

        if (!Disabled)
        {
            targetButton.interactable =
                !limited || numberofUses > 0;
        }
        else
        {
            targetButton.interactable = false;
        }
    }

    private void DeactivateOtherActionContainers()
    {
        ActionContainer[] actionContainers =
            FindObjectsOfType<ActionContainer>();

        foreach (ActionContainer container
                 in actionContainers)
        {
            if (container == this)
                continue;

            container.SetActive(false);

            container.UpdateButtonInteractability(
                container.button
            );
        }
    }

    private void ClearTimelineChildren()
    {
        foreach (TimeLineChild child
                 in Director.Instance.timeline
                     .children.ToList())
        {
            if (child == null || !child.CanClear)
                continue;

            Director.Instance.timeline
                .children.Remove(child);

            if (child == TempTL)
            {
                TempTL = null;
                CreatedTempTimelineChild = false;
            }

            if (child == TL)
            {
                TL = null;
            }

            Destroy(child.gameObject);
        }
    }

    private TimeLineChild InstantiateTimelineChild()
    {
        TL = Instantiate(
            Director.Instance.timeline.borderChildprefab,
            Director.Instance.timeline.startpoint
        );

        TL.unit = baseUnit;

        if (TL.portrait != null &&
            baseUnit.charPortraits != null &&
            baseUnit.charPortraits.Count > 0)
        {
            TL.portrait.sprite =
                baseUnit.charPortraits[0];
        }

        Director.Instance.timeline.children.Add(TL);

        TL.CanMove = false;

        ApplyTimelineChildCost();

        TL.CanClear = true;

        LabUIInteractable interactable =
            TL.GetComponent<LabUIInteractable>();

        if (interactable != null)
        {
            interactable.CanHover = false;
        }

        TL.CanBeHighlighted = false;

        Image timelineImage =
            TL.GetComponentInChildren<Image>();

        if (timelineImage != null)
        {
            timelineImage.color =
                new Color(1f, 1f, 1f, 0.5f);
        }

        if (TL.portrait != null)
        {
            TL.portrait.color =
                new Color(1f, 1f, 1f, 0.5f);
        }

        return TL;
    }

    private void SetTimelineChildProperties()
    {
        if (TL == null)
            return;

        ApplyTimelineChildCost();

        TL.CanClear = true;

        LabUIInteractable interactable =
            TL.GetComponent<LabUIInteractable>();

        if (interactable != null)
        {
            interactable.CanHover = false;
        }

        TL.CanBeHighlighted = false;

        Image timelineImage =
            TL.GetComponentInChildren<Image>();

        if (timelineImage != null)
        {
            timelineImage.color =
                new Color(1f, 1f, 1f, 0.5f);
        }

        if (TL.portrait != null)
        {
            TL.portrait.color =
                new Color(1f, 1f, 1f, 0.5f);
        }
    }

    private void ApplyTimelineChildCost()
    {
        if (TL == null ||
            action == null ||
            baseUnit == null)
        {
            return;
        }

        action.unit = baseUnit;

        float finalCost =
            Mathf.Clamp(
                CombatTools.DetermineTrueCost(action),
                0f,
                100f
            );

        TL.rectTransform.anchoredPosition =
            new Vector3(
                (100f - finalCost) * TL.offset,
                50f
            );

        TL.staminaText.text =
            Mathf.RoundToInt(finalCost).ToString();
    }

    private void ScaleObject()
    {
        ScalableObject scalableObject =
            GetComponent<ScalableObject>();

        if (scalableObject != null)
        {
            transform.localScale =
                scalableObject.oldScaleSize;
        }
    }

    private void SetButtonInteractable(
        bool interactable)
    {
        Button actionButton =
            GetComponent<Button>();

        if (actionButton != null)
        {
            actionButton.interactable =
                interactable;
        }
    }

    public void RefreshActionValues()
    {
        if (baseUnit == null || action == null)
            return;

        action.unit = baseUnit;

        SubscribeToUnit();

        if (damageNums != null &&
            action.actionType ==
                Action.ActionType.ATTACK)
        {
            Unit previewTarget =
                targetting
                    ? currentPreviewTarget
                    : null;

            UpdateDamageDisplay(previewTarget);
        }

        UpdateCostNumbers();

        if (TL != null)
        {
            ApplyTimelineChildCost();
        }
    }
}