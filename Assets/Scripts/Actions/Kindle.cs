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
        lightCost = 60;
        heavyCost = 60;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
    }

    public override string GetDescription()
    {
        description = $"Heals allies by 5 <sprite name=\"HP\">.\nIncreases allies <sprite name=\"ATK RED2\"> by 2.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        unit.ChangeUnitsLight(unit.spotLight, 150, 15, Color.green, 0.04f, 0.1f);


        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "BeaconLine", Color.green, Color.white, new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 8));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "BeaconCircle", Color.green, Color.white, new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 8));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "Beacon2", Color.green, Color.white, new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 8));

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
