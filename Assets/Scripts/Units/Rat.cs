using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rat : Unit
{
    private int[] ActionVariance01 = { 40, 45, 50, 55, 60 };
    private int[] ActionVariance02 = { 20, 25, 30, 35, 40 };
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
        Tools.ModifyAction(this, "Strike", 0, ActionVariance01[UnityEngine.Random.Range(0, ActionVariance01.Length)]);
        Tools.ModifyAction(this, "Enrage", 1, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
        Tools.ModifyAction(this, "Screech", 2, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);

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
        private int[] ActionVariance01 = { 40, 45, 50, 55, 60 };
        private int[] ActionVariance02 = { 20, 25, 30, 35, 40 };
        private Action lastAction = null;
        private int turn = 0;
        public override void DoBehavior(Unit baseUnit)
        {
            var battlesystem = BattleSystem.Instance;
            int move;

            Tools.ModifyAction(baseUnit, "Strike", 0, ActionVariance01[UnityEngine.Random.Range(0, ActionVariance01.Length)]);
            Tools.ModifyAction(baseUnit, "Enrage", 1, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
            Tools.ModifyAction(baseUnit, "Screech", 2, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
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
                Tools.SetupEnemyAction(baseUnit, move, null);
            }
            else if (lastAction.actionType == Action.ActionType.ATTACK)
            {
                move = UnityEngine.Random.Range(1, baseUnit.actionList.Count);
                Tools.SetupEnemyAction(baseUnit, move, null);
            }
        }
    }
}
