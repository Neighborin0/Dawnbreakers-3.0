using System.Linq;
using UnityEngine;

public class HuskEasyA : Unit
{
    private void Awake()
    {
        unitName = "HuskEasy";

        maxHP = 18;
        currentHP = maxHP;

        attackStat = 0;
        defenseStat = 0;
        IsPlayerControlled = false;

        weaknesses = new[]
        {
            DamageType.STRIKE,
            DamageType.LIGHT
        };

        HuskEasyA[] existingHusks =
            FindObjectsOfType<HuskEasyA>();

        bool anotherHuskAlreadyExists =
            existingHusks.Any(husk => husk != this);

        if (anotherHuskAlreadyExists)
        {
            behavior =
                gameObject.AddComponent<HuskEasyBEnemyBehavior>();
        }
        else
        {
            behavior =
                gameObject.AddComponent<HuskEasyAEnemyBehavior>();
        }
    }

    public class HuskEasyAEnemyBehavior : EnemyBehavior
    {
        public override void DoBehavior(Unit baseUnit)
        {
            if (!HasRequiredActions(baseUnit))
                return;

            int move;

            switch (turn)
            {
                case 0:
                    // Husk A opens with Lunge.
                    move = 0;
                    turn = 1;
                    break;

                case 1:
                    // Then uses Swipe.
                    move = 1;
                    turn = 2;
                    break;

                default:
                    move = Random.Range(
                        0,
                        baseUnit.actionList.Count
                    );

                    turn = 0;
                    break;
            }

            CombatTools.SetupEnemyAction(
                baseUnit,
                move,
                null
            );
        }
    }

    public class HuskEasyBEnemyBehavior : EnemyBehavior
    {
        public override void DoBehavior(Unit baseUnit)
        {
            if (!HasRequiredActions(baseUnit))
                return;

            int move;

            switch (turn)
            {
                case 0:
                    // Husk B opens with Swipe.
                    move = 1;
                    turn = 1;
                    break;

                case 1:
                    // Then uses Lunge.
                    move = 0;
                    turn = 2;
                    break;

                default:
                    move = Random.Range(
                        0,
                        baseUnit.actionList.Count
                    );

                    turn = 0;
                    break;
            }

            CombatTools.SetupEnemyAction(
                baseUnit,
                move,
                null
            );
        }
    }

    private static bool HasRequiredActions(Unit unit)
    {
        if (unit == null ||
            unit.actionList == null ||
            unit.actionList.Count < 2)
        {
            Debug.LogError(
                "Easy Husk requires at least two actions: " +
                "Lunge at index 0 and Swipe at index 1."
            );

            return false;
        }

        return true;
    }
}