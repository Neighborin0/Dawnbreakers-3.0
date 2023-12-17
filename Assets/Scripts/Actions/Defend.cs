using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;


[CreateAssetMenu(fileName = "Defend", menuName = "Assets/Actions/Defend")]
public class Defend : Action
{
    private void OnEnable()
    {
        ActionName = "Defend";
        cost = 0f;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        //duration = 1;
        CanBeStyled = false;

        statAmount = 5;
    }

    public override string GetDescription()
    {
        description = $"Applies +{statAmount + unit.defenseStat} <sprite name=\"FORTIFY\"> to self";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "DefendSelf", Color.blue, Color.blue ,new Vector3(0, 1, -2f), Quaternion.identity, 0.8f, 0, true, 0, 10)); 
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f);
        yield return new WaitForSeconds(0.8f);
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        //BattleSystem.Instance.SetTempEffect(targets, "DEF", true, duration, statAmount + unit.defenseStat);
        BattleSystem.Instance.SetStatChanges(Stat.ARMOR, statAmount + unit.defenseStat, false, targets);
        yield return new WaitForSeconds(1.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
