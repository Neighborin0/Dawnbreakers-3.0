using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Husk : Unit
{
    void Awake()
    {
        unitName = "Husk";
        maxHP = UnityEngine.Random.Range(25, 30);
        defenseStat = 0;
        speedStat = UnityEngine.Random.Range(7, 9);
        //speedStat = 7;
        currentHP = maxHP;
        IsPlayerControlled = false;

        if (BattleSystem.Instance.enemyUnits.Where(obj => obj.unitName == "Matriarch").SingleOrDefault())
        {
            behavior = this.gameObject.AddComponent<RandomEnemyBehavior>();
            maxHP = UnityEngine.Random.Range(17, 22);
            attackStat = UnityEngine.Random.Range(4, 5);
            print("using Matriarch behavior");
        }
        else
        {
            behavior = this.gameObject.AddComponent<RandomEnemyBehavior>();
            attackStat = UnityEngine.Random.Range(7, 12);
            print("using regular behavior");
        }
        StartingStamina = UnityEngine.Random.Range(45, 50);
    }

public class TutorialHuskMatriarchBehavior : EnemyBehavior
{

    public override IEnumerator DoBehavior(Unit baseUnit)
        {
            var battlesystem = BattleSystem.Instance;
            var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
            if (battlesystem.numOfUnits[num].IsPlayerControlled)
            {
                int move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
                Tools.DetermineActionData(baseUnit, move, num);
                if(battlesystem.playerUnits[1] != null)
                 baseUnit.actionList[move].targets = battlesystem.playerUnits[1];

                baseUnit.state = PlayerState.READY;
                battlesystem.DisplayEnemyIntent(baseUnit.actionList[move], baseUnit);
                yield return new WaitUntil(() => baseUnit.stamina.slider.value >= baseUnit.stamina.slider.maxValue);
                Tools.DetermineActionData(baseUnit, move, num);
                if (battlesystem.playerUnits[1] != null)
                    baseUnit.actionList[move].targets = battlesystem.playerUnits[1];
                baseUnit.stamina.DoCost(baseUnit.actionList[move].cost);
                battlesystem.AddAction(baseUnit.actionList[move]);
            }
            else
            {
                StartCoroutine(Tools.RepeatBehavior(baseUnit));
            }
        }

}




}

