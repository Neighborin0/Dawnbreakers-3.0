using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;


[CreateAssetMenu(fileName = "Brace", menuName = "Assets/Actions/Brace")]
public class Brace : Action
{
    private void OnEnable()
    {
        ActionName = "Brace";

        cost = 45f;
        lightCost = 30f;
        heavyCost = 60f;

        statAmount = 4;
        lightStatAmount = 3;
        heavyStatAmount = 6; 

        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Applies +{CombatTools.DetermineTrueActionValue(this) + unit.defenseStat} <sprite name=\"FORTIFY\"> to self.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Defend_001", new Color(0, 108, 191), Color.white, new Vector3(0, 5, -2f), Quaternion.identity, 0.8f, 0, true, 0, 0.22f));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.8f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "ShieldParticles", new Color(0, 108, 191), Color.blue, new Vector3(0, 0, -1f), Quaternion.identity, 0.8f, 0, true, 0, 0.22f));
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        //BattleSystem.Instance.SetTempEffect(targets, "DEF", true, duration, CombatTools.DetermineTrueActionValue(this) + unit.defenseStat);
        BattleSystem.Instance.SetStatChanges(Stat.ARMOR, CombatTools.DetermineTrueActionValue(this) + unit.defenseStat, false, targets);
        yield return new WaitForSeconds(1.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
        LabCamera.Instance.ResetPosition();
        yield return new WaitForSeconds(0.5f);
        this.Done = true;
        yield break;
    }
}
