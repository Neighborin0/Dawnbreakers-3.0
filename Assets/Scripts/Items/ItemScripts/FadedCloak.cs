using System.Collections;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FadedCloak",
    menuName = "Assets/Items/FadedCloak")]
public class FadedCloak : Item
{
    private const int DamageBonus = 1;
    private const int CostReduction = 5;

    private bool costReductionActive;
    private bool damageBonusActive;

    private void OnEnable()
    {
        itemName = "Faded Cloak";

        itemDescription =
            "The first damaging action each battle costs " +
            "<color=#FFFF00>5 less</color>. " +
            "After using it, the next damaging action deals " +
            "<color=#FF0500>+1</color> DMG.";

        CanBeTransfered = false;
        ExcludedFromLootPools = true;

        costReductionActive = false;
        damageBonusActive = false;
    }

    public override void OnPickup(Unit unit)
    {
        if (unit == null)
            return;

        // Prevent duplicate event subscriptions.
        unit.BattleStarted -= ApplyEffect;
        unit.BattleStarted += ApplyEffect;

        unit.OnPostAction -= ApplyDamageBonus;
        unit.OnPostAction -= RemoveDamageBonus;

        costReductionActive = false;
        damageBonusActive = false;

        Debug.Log(
            $"[Faded Cloak] Registered on {unit.unitName}"
        );
    }

    private void ApplyEffect(Unit unit)
    {
        if (unit == null)
            return;

        Debug.Log(
            $"[Faded Cloak] Battle started. " +
            $"Cost before: {unit.actionCostAddend}"
        );

        // Remove old action subscriptions without modifying stats.
        unit.OnPostAction -= ApplyDamageBonus;
        unit.OnPostAction -= RemoveDamageBonus;

        costReductionActive = false;
        damageBonusActive = false;

        // Activate the opening cost reduction.
        unit.actionCostAddend -= CostReduction;
        costReductionActive = true;

        unit.OnPostAction += ApplyDamageBonus;

        unit.DoActionModifiersChanged();

        Debug.Log(
            $"[Faded Cloak] Cost after: " +
            $"{unit.actionCostAddend}"
        );

        ShowPopup(
            unit,
            "-5 ACTION COST",
            Color.yellow
        );
    }

    private void ApplyDamageBonus(Unit unit)
    {
        if (unit == null)
            return;

        // The opening effect only triggers once.
        unit.OnPostAction -= ApplyDamageBonus;

        if (costReductionActive)
        {
            unit.actionCostAddend += CostReduction;
            costReductionActive = false;
        }

        unit.actionDMGAddend += DamageBonus;
        damageBonusActive = true;

        unit.OnPostAction -= RemoveDamageBonus;
        unit.OnPostAction += RemoveDamageBonus;

        unit.DoActionModifiersChanged();

        Debug.Log(
            $"[Faded Cloak] Damage bonus activated. " +
            $"Damage addend: {unit.actionDMGAddend}"
        );

       
        if (BattleSystem.Instance != null)
        {
            BattleSystem.Instance.StartCoroutine(
                ShowDamagePopupAtDecisionPhase(unit)
            );
        }
    }

    private IEnumerator ShowDamagePopupAtDecisionPhase(Unit unit)
    {
        if (unit == null ||
            BattleSystem.Instance == null)
        {
            yield break;
        }

        yield return new WaitUntil(() =>
            BattleSystem.Instance == null ||
            BattleSystem.Instance.state ==
                BattleStates.DECISION_PHASE ||
            BattleSystem.Instance.state ==
                BattleStates.WON ||
            BattleSystem.Instance.state ==
                BattleStates.DEAD
        );

        if (BattleSystem.Instance == null ||
            unit == null)
        {
            yield break;
        }

        if (BattleSystem.Instance.state !=
            BattleStates.DECISION_PHASE)
        {
            yield break;
        }

        ShowPopup(unit, "+1 DMG UP", Color.white);
    }

    private void RemoveDamageBonus(Unit unit)
    {
        if (unit == null)
            return;

        unit.OnPostAction -= RemoveDamageBonus;

        if (damageBonusActive)
        {
            unit.actionDMGAddend -= DamageBonus;
            damageBonusActive = false;
        }

        unit.DoActionModifiersChanged();

        Debug.Log(
            $"[Faded Cloak] Damage bonus consumed. " +
            $"Damage addend: {unit.actionDMGAddend}"
        );
    }

    private void ShowPopup(
        Unit unit,
        string message,
        Color color)
    {
        if (unit == null)
            return;

        if (BattleSystem.Instance == null)
        {
            Debug.LogWarning(
                $"[Faded Cloak] Could not show popup: " +
                $"BattleSystem.Instance is null."
            );

            return;
        }

        if (BattleSystem.Instance.statPopUp == null)
        {
            Debug.LogWarning(
                $"[Faded Cloak] Could not show popup: " +
                $"BattleSystem.statPopUp is not assigned."
            );

            return;
        }

        SpriteRenderer renderer =
            unit.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            Debug.LogWarning(
                $"[Faded Cloak] Could not show popup: " +
                $"{unit.unitName} has no SpriteRenderer."
            );

            return;
        }

        BattleSystem.Instance.DoTextPopup(
            unit,
            message,
            color
        );

        Debug.Log(
            $"[Faded Cloak] Popup displayed: {message}"
        );
    }

    private void ClearPendingEffects(Unit unit)
    {
        if (unit == null)
            return;

        unit.OnPostAction -= ApplyDamageBonus;
        unit.OnPostAction -= RemoveDamageBonus;

        if (costReductionActive)
        {
            unit.actionCostAddend += CostReduction;
            costReductionActive = false;
        }

        if (damageBonusActive)
        {
            unit.actionDMGAddend -= DamageBonus;
            damageBonusActive = false;
        }
    }

    public override void OnRemoved(Unit unit)
    {
        if (unit == null)
            return;

        ClearPendingEffects(unit);

        unit.BattleStarted -= ApplyEffect;
        unit.OnPostAction -= ApplyDamageBonus;
        unit.OnPostAction -= RemoveDamageBonus;

        unit.DoActionModifiersChanged();
    }
}