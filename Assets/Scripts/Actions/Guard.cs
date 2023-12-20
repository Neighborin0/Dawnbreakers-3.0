using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;


[CreateAssetMenu(fileName = "Guard", menuName = "Assets/Actions/Guard")]
public class Guard : Action
{
    private void OnEnable()
    {
        ActionName = "Guard";

        cost = 20f;
        lightCost = 0;
        heavyCost = 40f;

        statAmount = 5;
        lightStatAmount = 3;
        heavyStatAmount = 7; 

        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.ALLY;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Applies +{CombatTools.DetermineTrueActionValue(this) + unit.defenseStat} <sprite name=\"FORTIFY\"> to self or allies.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Defend_001", Color.blue, Color.white, new Vector3(0, 5, -2f), Quaternion.identity, 0.8f, 0, true, 0, 10));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f);
        yield return new WaitForSeconds(0.8f);
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        //BattleSystem.Instance.SetTempEffect(targets, "DEF", true, duration, CombatTools.DetermineTrueActionValue(this) + unit.defenseStat);
        BattleSystem.Instance.SetStatChanges(Stat.ARMOR, CombatTools.DetermineTrueActionValue(this) + unit.defenseStat, false, targets);
        yield return new WaitForSeconds(1.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        yield return new WaitForSeconds(0.5f);
        this.Done = true;
        yield break;
    }
}
