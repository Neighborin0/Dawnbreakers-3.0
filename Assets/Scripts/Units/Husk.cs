using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Husk : Unit
{
    private int[] ActionVariance01 = {40 , 45, 50 ,55, 60 };
    void Awake()
    {
        unitName = "Husk";
        defenseStat = 0;
        //speedStat = UnityEngine.Random.Range(7, 9);
        //speedStat = 7;
        currentHP = maxHP;
        IsPlayerControlled = false;
        weaknesses = new DamageType[] { DamageType.SLASH, DamageType.PIERCE };
        CombatTools.ModifyAction(this, "Strike", 0, ActionVariance01[UnityEngine.Random.Range(0 , ActionVariance01.Length)]);
        if (BattleSystem.Instance.enemyUnits.Where(obj => obj.unitName.Contains("Matriarch")).SingleOrDefault())
        {
            behavior = this.gameObject.AddComponent<TutorialHuskMatriarchBehavior>();
            maxHP = UnityEngine.Random.Range(17, 22);
            attackStat = UnityEngine.Random.Range(4, 6);
            print("using Matriarch behavior");
        }
        else
        {
            behavior = this.gameObject.AddComponent<TutorialHuskMatriarchBehavior>();
            maxHP = UnityEngine.Random.Range(17, 22);
            attackStat = UnityEngine.Random.Range(0, 2);
            print("using regular behavior");
        }
    }

    public class TutorialHuskMatriarchBehavior : EnemyBehavior
    {
        private int[] ActionVariance01 = { 40, 45, 50, 55, 60 };
        public override void DoBehavior(Unit baseUnit)
        {
            int move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
            CombatTools.ModifyAction(baseUnit, "Strike", 0, ActionVariance01[UnityEngine.Random.Range(0, ActionVariance01.Length)]);
            if (CombatTools.CheckAndReturnNamedUnit("Dusty") != null)
            {
                CombatTools.SetupEnemyAction(baseUnit, move, CombatTools.CheckAndReturnNamedUnit("Dusty"));
            }
            else
            {
                CombatTools.SetupEnemyAction(baseUnit, move, null);
            }

        }

    }
}

