using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;
[CreateAssetMenu(fileName = "Kindle", menuName = "Assets/Actions/Kindle")]
public class Kindle : Action
{
    private int playerDef;
    private void OnEnable()
    {
        ActionName = "Kindle";
        cost = 100f;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
        PriorityMove = false;
        targetType = TargetType.SELF;
        description = $"Heals allies by 10 <sprite name=\"HP\"> Increases allies <sprite name=\"ATK RED2\"> by 2.";
    }

    public override IEnumerator ExecuteAction()
    {
        foreach (var x in Tools.DetermineAllies(unit))
        {
            Director.Instance.StartCoroutine(ActuallyDoFastStatChangesUnlikePokemon(x));
        }   
        yield return new WaitForSeconds(1.3f);
        this.Done = true;
        yield break;
    }

    private IEnumerator ActuallyDoFastStatChangesUnlikePokemon(Unit x)
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 3f, false, x);
        yield return new WaitForSeconds(1f);
        BattleSystem.Instance.SetStatChanges(Stat.HP, 10f, false, x);
    }
}
