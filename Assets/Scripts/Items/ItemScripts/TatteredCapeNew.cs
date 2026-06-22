using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(
    fileName = "TatteredCapeNew",
    menuName = "Assets/Items/TatteredCapeNew")]
public class TatteredCapeNew : Item
{
    private const int DamageBonus = 1;
    private const float CostReduction = 5f;

    private void OnEnable()
    {
        itemName = "TatteredCapeNew";

        itemDescription = $"Upon taking damage, the next action deals +{DamageBonus} damage and costs -{CostReduction} less.";

        CanBeTransfered = false;
        ExcludedFromLootPools = true;
    }

    public override void OnPickup(Unit unit)
    {
        if (unit == null)
            return;

        unit.OnDamaged -= Empower;
        unit.OnDamaged += Empower;
    }

    private void Empower(Unit unit)
    {
        if (unit == null || unit.currentHP <= 0)
            return;

        BattleSystem.Instance.SetTempEffect(unit, "EMPOWER", false);

    }

    public override void OnRemoved(Unit unit)
    {
        if (unit == null)
            return;

        unit.OnDamaged -= Empower;

    }
}