using System.Collections;
using System.Linq;
using UnityEngine;

public class EmpowerIcon : EffectIcon
{
    private const int DamageBonus = 1;
    private const int CostReduction = 5;

    private bool modifiersApplied;
    private bool ending;

    new void Start()
    {
        iconName = "Empower";

        EmpowerIcon existingEmpower = owner.statusEffects
            .OfType<EmpowerIcon>()
            .FirstOrDefault(effect => effect != this);

        if (existingEmpower != null)
        {
            // Empower does not stack.
            owner.statusEffects.Remove(this);
            Destroy(gameObject);
            return;
        }

        BattleSystem.Instance.DoTextPopup(
            owner,
            "EMPOWER",
            Color.white
        );

        owner.OnPostAction -= OnOwnerActionEnded;
        owner.OnPostAction += OnOwnerActionEnded;

        owner.actionCostAddend -= CostReduction;
        owner.actionDMGAddend += DamageBonus;

        modifiersApplied = true;

        owner.DoActionModifiersChanged();

        canBeStacked = true;
    }

    private void OnOwnerActionEnded(Unit actingUnit)
    {
        if (actingUnit != owner || ending)
            return;

        ending = true;

        owner.OnPostAction -= OnOwnerActionEnded;

        Director.Instance.StartCoroutine(End());
    }

    public override string GetDescription()
    {
        return
            $"The next action deals +{DamageBonus} damage " +
            $"and costs {CostReduction} less.";
    }

    public override IEnumerator End()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            yield break;
        }

        owner.OnPostAction -= OnOwnerActionEnded;

        if (modifiersApplied)
        {
            owner.actionCostAddend += CostReduction;
            owner.actionDMGAddend -= DamageBonus;

            modifiersApplied = false;

            // Refresh only after the modifiers have changed.
            owner.DoActionModifiersChanged();
        }

        owner.statusEffects.Remove(this);

        Destroy(gameObject);

        yield break;
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnPostAction -= OnOwnerActionEnded;
        }
    }
}