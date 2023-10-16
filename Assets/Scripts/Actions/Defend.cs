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
    private void OnEnable()
    {
        ActionName = "Defend";
        cost = 20f;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        duration = 3f;
    }

    public override string GetDescription()
    {
        description = $"Applies +{(int)Math.Round(unit.defenseStat * 0.4f)} <sprite name=\"FORTIFY\"> for {duration} seconds.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "DefendSelf", Color.blue, Color.blue ,new Vector3(0, 1, -2f), 2f, 0, true, 0, 10)); 
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 6, -40, 0.5f);
        yield return new WaitForSeconds(0.8f);
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        BattleSystem.Instance.SetTempEffect(targets, "DEF", true, duration, (int)Math.Round(unit.defenseStat * 0.4f));
        BattleSystem.Instance.SetStatChanges(Stat.DEF, (int)Math.Round(unit.defenseStat * 0.4f), false, targets);
        yield return new WaitForSeconds(1.3f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
