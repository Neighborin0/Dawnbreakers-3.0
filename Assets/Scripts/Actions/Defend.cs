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

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Applies +{statAmount + unit.defenseStat} <sprite name=\"FORTIFY\"> to self.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(10));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "DefendSelf", new Color(0, 144, 255), new Color(0, 144, 255), new Vector3(0, 1, -2f), Quaternion.identity, 0.8f, 0, true, 0, 1));      
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.8f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "ShieldParticles", new Color(0, 144, 255), new Color(0, 144, 255), new Vector3(0, 0, -1f), Quaternion.identity, 0.8f, 0, true, 0, 1));
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        BattleSystem.Instance.SetStatChanges(Stat.ARMOR, statAmount + unit.defenseStat, false, targets);
        yield return new WaitForSeconds(1.5f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
        AudioManager.QuickPlay("ui_woosh_002");
        LabCamera.Instance.ResetPosition();
        yield return new WaitForSeconds(0.6f);
        this.Done = true;
        yield break;
    }
}
