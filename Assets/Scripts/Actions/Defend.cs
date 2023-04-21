using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;
using Unity.VisualScripting.FullSerializer;

[CreateAssetMenu(fileName = "Defend", menuName = "Assets/Actions/Defend")]
public class Defend : Action
{
    private int storedDEF;
    private void OnEnable()
    {
        ActionName = "Defend";
        accuracy = 1;
        cost = 25f;
        actionType = ActionType.STATUS;
        PriorityMove = true;
        targetType = TargetType.ALLIES;
        duration = 5f;
        description = $"Applies <color=#00F0FF>Fortify</color> for {duration} seconds.\n<color=#00F0FF>Fortify</color> doubles the user's <color=#0000FF>DEF</color>.";
    }

    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Defend_001", Color.blue, new Vector3(0, 5, -2f), 0.2f));
        LabCamera.Instance.MoveToUnit(targets, 7.8f, -2, 10);
        yield return new WaitForSeconds(0.1f);
        Director.Instance.StartCoroutine(LabCamera.Instance.DoSlowHorizontalSweep());
        yield return new WaitForSeconds(1f);
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        BattleSystem.Instance.SetStatChanges(Stat.DEF, unit.defenseStat, false, targets);
        BattleLog.Instance.StartCoroutine(Tools.ChangeLightIntensityTimed(Light, 150, 15, 0.04f, 0.06f));
        Director.Instance.StartCoroutine(BattleSystem.Instance.SetTempEffect(targets, "DEF", this));
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }

   public override void OnEnded(Unit otherUnit)
    {
        otherUnit = unit;
        BattleSystem.Instance.SetStatChanges(Stat.DEF, -otherUnit.defenseStat / 2, false, targets);
        Debug.LogError("On Ended");
    }
}
