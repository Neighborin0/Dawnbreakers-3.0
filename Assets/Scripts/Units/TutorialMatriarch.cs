using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMatriarch : Unit
{
   private string[] TutorialSummons = new string[]{"Husk"};

    void Awake()
    {
        unitName = "Matriarch";
        maxHP = 168;
        attackStat = 5;
        defenseStat = 0;
        speedStat = 14;
        actionCostMultiplier = 100f;
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior =  this.gameObject.AddComponent<MatriarchBehaviorLV0>();
        summonables = TutorialSummons;
    }

    public class MatriarchBehaviorLV0 : EnemyBehavior
    {
        private int turn;
        private BattleSystem battlesystem;
        private Unit BaseUnit;

        public override IEnumerator DoBehavior(Unit baseUnit)
        {
            battlesystem = BattleSystem.Instance;
            BaseUnit = baseUnit;
            var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
            if (turn < 2)
            {
                if (battlesystem.numOfUnits[num].IsPlayerControlled)
                {
                    Tools.DetermineActionData(baseUnit, turn, num);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[turn], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, turn, num);
                    baseUnit.stamina.DoCost(baseUnit.actionList[turn].cost);
                    battlesystem.AddAction(baseUnit.actionList[turn]);
                    turn++;
                }
                else
                {
                    StartCoroutine(Tools.RepeatBehavior(baseUnit));
                }
            }
            else if(turn == 2)
            {
                if (battlesystem.numOfUnits[num].IsPlayerControlled)
                {
                    int whichAction = 2;
                    if(Tools.DetermineAllies(baseUnit).Count < 3)
                    {
                        whichAction = 3;
                    }
                    Tools.DetermineActionData(baseUnit, whichAction, num);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[whichAction], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, whichAction, num);
                    baseUnit.stamina.DoCost(baseUnit.actionList[whichAction].cost);
                    battlesystem.AddAction(baseUnit.actionList[whichAction]);
                    turn++;
                }
                else
                {
                    StartCoroutine(Tools.RepeatBehavior(baseUnit));
                }
                
            }
            else
            { 
                turn = 0;
                StartCoroutine(Tools.RepeatBehavior(baseUnit));
            }
        }
    }

}
