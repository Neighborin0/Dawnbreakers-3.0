using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RandomEnemyBehavior : EnemyBehavior
{

    public override IEnumerator DoBehavior(Unit baseUnit)
    {
        var battlesystem =  BattleSystem.Instance;
        var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
        if (battlesystem.numOfUnits[num].IsPlayerControlled)
        {
            int move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
            Tools.DetermineActionData(baseUnit, move, num);
            baseUnit.state = PlayerState.READY;
            battlesystem.DisplayEnemyIntent(baseUnit.actionList[move], baseUnit);
            yield return new WaitUntil(() => baseUnit.stamina.slider.value >= baseUnit.stamina.slider.maxValue);
            Tools.DetermineActionData(baseUnit, move, num);
            baseUnit.stamina.DoCost(baseUnit.actionList[move].cost);
            battlesystem.AddAction(baseUnit.actionList[move]);
        }
        else
        {
            StartCoroutine(Tools.RepeatBehavior(baseUnit));
        } 
    }

}



