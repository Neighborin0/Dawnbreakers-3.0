using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rat : Unit
{
    void Awake()
    {
        unitName = "Vermin";
        maxHP = UnityEngine.Random.Range(26, 30);
        attackStat = UnityEngine.Random.Range(6, 11);
        defenseStat = UnityEngine.Random.Range(0, 2);
        speedStat = UnityEngine.Random.Range(9, 11);
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior = this.gameObject.AddComponent<VerminBehavior>();
        Tools.ModifyAction(this, "Strike", 0, 50f);
        Tools.ModifyAction(this, "Enrage", 1, 30f);
        Tools.ModifyAction(this, "Screech", 2, 30f);

    }

    void Start()
    {
        if (Tools.DetermineAllies(this).Count() < 1)
        {
            Tools.CheckAndReturnNamedUnit("Aurelia").EnteredMap += AddGlint;
        }
    }

    private void AddGlint(Unit unit)
    {
        Tools.AddNewActionToUnit(Tools.CheckAndReturnNamedUnit("Aurelia"), "Glint");
    }

    public class VerminBehavior : EnemyBehavior
    {
        private Action lastAction = null;
        private int turn = 0;
        public override void DoBehavior(Unit baseUnit)
        {
            var battlesystem = BattleSystem.Instance;
            int move;

            if (turn == 0)
            {
                move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
                lastAction = baseUnit.actionList[move];
                Tools.SetupEnemyAction(baseUnit, move);
                turn++;
            }
            else if (lastAction.actionType == Action.ActionType.STATUS)
            {
                move = 0;
                lastAction = baseUnit.actionList[move];
                Tools.SetupEnemyAction(baseUnit, move);
            }
            else if (lastAction.actionType == Action.ActionType.ATTACK)
            {
                move = UnityEngine.Random.Range(1, baseUnit.actionList.Count);
                Tools.SetupEnemyAction(baseUnit, move);
            }
        }
    }
}
