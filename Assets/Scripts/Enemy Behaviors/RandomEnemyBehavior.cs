using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RandomEnemyBehavior : EnemyBehavior
{
    public override void DoBehavior(Unit baseUnit)
    {
        var battlesystem =  BattleSystem.Instance;
        var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
        if (battlesystem.numOfUnits[num].IsPlayerControlled)
        {
            int move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
            Tools.SetupEnemyAction(baseUnit, move);
        } 
    }

}



