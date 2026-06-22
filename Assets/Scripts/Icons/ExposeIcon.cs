using System.Collections;
using System.Linq;
using UnityEngine;

public class ExposeIcon : EffectIcon
{
    private const int DelayBonus = 15;
    private bool modifierApplied;

    new void Start()
    {
        iconName = "Expose";
        TimedEffect = true;

        bool alreadyExposed = owner.statusEffects
            .Any(effect => effect != this && effect.iconName == iconName);

        if (alreadyExposed)
        {
            owner.statusEffects.Remove(this);
            Destroy(gameObject);
            return;
        }

        owner.knockbackModifider += DelayBonus;
        modifierApplied = true;

        Director.Instance.StartCoroutine(
            CombatTools.PlayVFX(
                owner.gameObject,
                "StatDownVFX",
                Color.white,
                Color.white,
                new Vector3(0, 15, 0),
                Quaternion.identity,
                1f,
                0,
                false,
                1f,
                1
            )
        );
    }

    public override string GetDescription()
    {
        return $"Effective hits apply <color=#FFFFFF>+{DelayBonus} Timeline Delay</color>.";
    }

    public override IEnumerator End()
    {
        if (modifierApplied)
        {
            owner.knockbackModifider =
                Mathf.Max(0, owner.knockbackModifider - DelayBonus);

            modifierApplied = false;
        }

        Director.Instance.StartCoroutine(
            CombatTools.PlayVFX(
                owner.gameObject,
                "StatDownVFX",
                Color.white,
                Color.white,
                new Vector3(0, 15, 0),
                Quaternion.identity,
                1f,
                0,
                false,
                1f,
                1
            )
        );

        owner.ChangeUnitsLight(
            owner.spotLight,
            20,
            2,
            Color.white,
            0.04f,
            0.1f
        );

        owner.statusEffects.Remove(this);
        Destroy(gameObject);

        yield break;
    }
}