using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RandomEnemyBehavior : EnemyBehavior
{
    public override void DoBehavior(Unit baseUnit)
    {
        int move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
        CombatTools.SetupEnemyAction(baseUnit, move);
    }

}



