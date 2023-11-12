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
            behavior = this.gameObject.AddComponent<TutorialHuskMatriarchBehavior>();
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
    }

    public class TutorialHuskMatriarchBehavior : EnemyBehavior
    {
        public override void DoBehavior(Unit baseUnit)
        {
            var battlesystem = BattleSystem.Instance;
            int move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
            if (battlesystem.playerUnits[1] != null)
            {
                Tools.SetupEnemyAction(baseUnit, move, battlesystem.playerUnits[1]);
            }
            else
            {
                Tools.SetupEnemyAction(baseUnit, move);
            }

        }

    }
}

