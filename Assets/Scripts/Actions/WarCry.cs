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
        cost = 20f;
        lightCost = 0f;
        heavyCost = 40f;
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;


        duration = 3;
        
        statAmount = 3;

        lightStatAmount = 2;
        heavyStatAmount = 4;
        damageText = damage.ToString();
        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Applies +{CombatTools.DetermineTrueActionValue(this)} <sprite name=\"VIGOR\"> to all allies for {duration} rounds.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(10));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        yield return new WaitForSeconds(0.3f);
        AudioManager.QuickPlay("warcry_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCry2", new Color(1, 0, 0, 0.1f), new Color(1, 0, 0, 0.1f), new Vector3(0, 2f, -2f), Quaternion.identity, 1.5f));
        yield return new WaitForSeconds(1.5f);
        foreach (var x in CombatTools.DetermineAllies(unit))
        {
            var Light = x.spotLight;
            unit.ChangeUnitsLight(Light, 150, 15, Color.red, 0.04f, 0.08f);
            BattleSystem.Instance.SetStatChanges(Stat.ATK, CombatTools.DetermineTrueActionValue(this), false, x);
            var battleSystem = BattleSystem.Instance;
            battleSystem.SetTempEffect(x, "ATK", true, duration, CombatTools.DetermineTrueActionValue(this), 0);
        }
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
        LabCamera.Instance.ResetPosition();
        yield return new WaitForSeconds(0.5f);
        this.Done = true;
        yield break;
    }

}
