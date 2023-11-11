using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "WarCry", menuName = "Assets/Actions/WarCry")]
public class WarCry : Action
{
    private int playerDef;
    private void OnEnable()
    {
        ActionName = "War Cry";
        cost = 30f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        duration = 5f;
        statAmount = 3;
        damageText = damage.ToString();
    }

    public override string GetDescription()
    {
        description = $"Applies +{statAmount} <sprite name=\"VIGOR\"> to all allies for {duration} seconds.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", new Color(1, 0, 0, 0.1f), new Color(1, 0, 0, 0.1f), new Vector3(0, 0, -2f), 1.1f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCryParticles", Color.red, Color.red, new Vector3(0, 0, -2f), 1.1f));
        yield return new WaitForSeconds(0.2f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", new Color(1, 0, 0, 0.1f), new Color(1, 0, 0, 0.1f), new Vector3(0, 0, -2f), 1.1f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCryParticles", Color.red, Color.red, new Vector3(0, 0, -2f), 1.1f));
        yield return new WaitForSeconds(0.2f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", new Color(1, 0, 0, 0.1f), new Color(1, 0, 0, 0.1f), new Vector3(0, 0, -2f), 1.1f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCryParticles", Color.red, Color.red, new Vector3(0, 0, -2f), 1.1f));
        yield return new WaitForSeconds(0.4f);
        foreach (var x in Tools.DetermineAllies(unit))
        {
            var Light = x.spotLight;
            unit.ChangeUnitsLight(Light, 150, 15, Color.red, 0.04f, 0.08f);
            BattleSystem.Instance.SetStatChanges(Stat.ATK, statAmount, false, x);
            var battleSystem = BattleSystem.Instance;
            battleSystem.SetTempEffect(x, "ATK", true, duration, statAmount, 0);
        }
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }

}
