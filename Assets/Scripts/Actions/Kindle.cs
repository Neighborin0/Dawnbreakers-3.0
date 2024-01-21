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

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Increases self and allies <sprite name=\"ATK RED2\"> by 2.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "KindleLight", Color.red, Color.red, new Vector3(-2.95f, 5.02f, 0f), Quaternion.identity, 10f, 0, true, 0, 8));
        AudioManager.QuickPlay("glint_001");

        yield return new WaitForSeconds(1f);
        foreach (var x in CombatTools.DetermineAllies(unit))
        {
            Director.Instance.StartCoroutine(ActuallyDoFastStatChangesUnlikePokemon(x));
        }   
        yield return new WaitForSeconds(0.5f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        this.Done = true;
        LabCamera.Instance.ResetPosition();
        yield break;
    }

    private IEnumerator ActuallyDoFastStatChangesUnlikePokemon(Unit x)
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 2f, false, x);
        yield return new WaitForSeconds(1f);
    }
}
