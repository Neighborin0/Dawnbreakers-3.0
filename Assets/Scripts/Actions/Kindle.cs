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
        cost = 60f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
    }

    public override string GetDescription()
    {
        description = $"Heals allies by 5 <sprite name=\"HP\"> .Increases allies <sprite name=\"ATK RED2\"> by 2.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(unit, 5, -1, 5);
        foreach (var x in Tools.DetermineAllies(unit))
        {
            Director.Instance.StartCoroutine(ActuallyDoFastStatChangesUnlikePokemon(x));
        }   
        yield return new WaitForSeconds(1.5f);
        this.Done = true;
        LabCamera.Instance.ResetPosition();
        yield break;
    }

    private IEnumerator ActuallyDoFastStatChangesUnlikePokemon(Unit x)
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 2f, false, x);
        yield return new WaitForSeconds(1f);
        BattleSystem.Instance.SetStatChanges(Stat.HP, 5f, false, x);
    }
}
