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
    private void OnEnable()
    {
        ActionName = "Kindle";
        cost = 60f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
    }

    public override string GetDescription()
    {
        description = $"Heals allies by 5 <sprite name=\"HP\">. Increases allies <sprite name=\"ATK RED2\"> by 2.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(unit, 0, 0, -55);
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        unit.spotLight.color = Color.green;
        unit.ChangeUnitsLight(unit.spotLight, 150, 15, 0.04f, 0.1f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "Beacon", Color.green, Color.white, new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 4));
        yield return new WaitForSeconds(1f);
        foreach (var x in Tools.DetermineAllies(unit))
        {
            Director.Instance.StartCoroutine(ActuallyDoFastStatChangesUnlikePokemon(x));
        }   
        yield return new WaitForSeconds(1.5f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
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
