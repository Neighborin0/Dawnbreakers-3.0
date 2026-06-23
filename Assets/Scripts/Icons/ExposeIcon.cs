
using System.Collections;
using UnityEngine;

public class ExposeIcon : EffectIcon
{
    private const int DelayBonusPerStack = 15;

    private int totalAppliedDelay;

    public override void Initalize(
        Unit unit,
        bool doFancyStatChanges,
        float duration = 0f,
        float storedValue = 0f,
        float numberofStacks = 0f)
    {
        iconName = "EXPOSE";
        TimedEffect = true;
        canBeStacked = true;

        base.Initalize(
            unit,
            doFancyStatChanges,
            duration,
            storedValue,
            numberofStacks
        );

        /*
         * Apply the gameplay modifier for the starting stacks.
         */
        int initialStacks =
            Mathf.Max(1, NumberofStacks);

        int delayToApply =
            DelayBonusPerStack * initialStacks;

        owner.knockbackModifider +=
            delayToApply;

        totalAppliedDelay +=
            delayToApply;

        PlayApplicationVFX();
    }

    public override void AddStacks(
        float addedDuration,
        float addedStoredValue,
        int addedStacks)
    {
        int safeStackAmount =
            Mathf.Max(1, addedStacks);

        base.AddStacks(
            addedDuration,
            addedStoredValue,
            safeStackAmount
        );

        /*
         * Each added stack contributes another +15 delay.
         */
        int addedDelay =
            DelayBonusPerStack *
            safeStackAmount;

        owner.knockbackModifider +=
            addedDelay;

        totalAppliedDelay +=
            addedDelay;

        PlayApplicationVFX();
    }

    private void PlayApplicationVFX()
    {
        if (owner == null ||
            Director.Instance == null)
        {
            return;
        }

        Director.Instance.StartCoroutine(
            CombatTools.PlayVFX(
                owner.gameObject,
                "StatDownVFX",
                Color.white,
                Color.white,
                new Vector3(0f, 15f, 0f),
                Quaternion.identity,
                0.2f,
                0f,
                false,
                1f,
                1f
            )
        );
    }

    public override string GetDescription()
    {
        return
            $"Effective hits apply " +
            $"<color=#FFFFFF>+{totalAppliedDelay} " +
            $"Timeline Delay</color>.";
    }

    public override IEnumerator End()
    {
        if (owner != null)
        {
            owner.knockbackModifider =
                Mathf.Max(
                    0,
                    owner.knockbackModifider -
                    totalAppliedDelay
                );
        }

        totalAppliedDelay = 0;

        if (Director.Instance != null &&
            owner != null)
        {
            Director.Instance.StartCoroutine(
                CombatTools.PlayVFX(
                    owner.gameObject,
                    "StatDownVFX",
                    Color.white,
                    Color.white,
                    new Vector3(0f, 15f, 0f),
                    Quaternion.identity,
                    0.2f,
                    0f,
                    false,
                    1f,
                    1f
                )
            );

            owner.ChangeUnitsLight(
                owner.spotLight,
                20f,
                2f,
                Color.white,
                0.04f,
                0.1f
            );
        }

        if (owner != null)
        {
            owner.statusEffects.Remove(this);
        }

        Destroy(gameObject);

        yield break;
    }
}

