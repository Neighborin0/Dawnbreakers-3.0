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
        accuracy = 1;
        cost = 30f;
        actionType = ActionType.STATUS;
        PriorityMove = false;
        targetType = TargetType.SELF;
        duration = 7f;
        damageText = damage.ToString();
        description = $"Applies <color=#FF2E00>Vigor</color><sprite name=\"VIGOR\"> to all allies for {duration} seconds. <color=#FF2E00>Vigor</color><sprite name=\"VIGOR\"> increases <color=#FF0000>ATK</color><sprite name=\"ATK RED2\"> by 3.";
    }

    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(unit, 0, -6, 32, false);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", Color.red, new Vector3(0, 0, -2f), 0.2f));
        yield return new WaitForSeconds(0.05f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "WarCry", Color.red, new Vector3(0, 0, -2f), 0.2f));
        yield return new WaitForSeconds(0.5f);
        foreach (var x in Tools.DetermineAllies(unit))
        {
            var Light = x.spotLight;
            Light.color = Color.red;
            unit.ChangeUnitsLight(Light, 150, 15, 0.04f, 0.08f);
            BattleSystem.Instance.SetStatChanges(Stat.ATK, 3f, false, x);
            var battleSystem = BattleSystem.Instance;
            battleSystem.StartCoroutine(battleSystem.SetTempEffect(x, "ATK", this, true, 0));
        }
        yield return new WaitForSeconds(0.2f);
        Director.Instance.StartCoroutine(LabCamera.Instance.DoSlowHorizontalSweep());
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }

   public override void OnEnded(Unit unit, float storedValue, bool DoFancyStatChnages)
    {
        BattleSystem.Instance.SetStatChanges(Stat.ATK, -3f, false, unit);
    }
}
