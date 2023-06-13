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
        accuracy = 1;
        cost = 25f;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        PriorityMove = true;
        targetType = TargetType.ALLIES;
        duration = 5f;
        description = $"Applies +50 <sprite name=\"FORTIFY\"> for {duration} seconds.";
    }

    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Defend_001", Color.blue, new Vector3(0, 5, -2f), 0.2f));
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false, 0.5f);
        yield return new WaitForSeconds(0.8f);
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        //BattleLog.Instance.StartCoroutine(Tools.ChangeLightIntensityTimed(Light, 150, 15, 0.04f, 0.06f));
        foreach(Transform icon in targets.namePlate.IconGrid.transform)
        {
            var EI = icon.gameObject.GetComponent<EffectIcon>();
            if(EI.iconName == "Fortify")
            {
                EI.DoFancyStatChanges = false;
                EI.DestoryEffectIcon();
                break;
            }
        }
        Director.Instance.StartCoroutine(BattleSystem.Instance.SetTempEffect(targets, "DEF", this, true, unit.defenseStat));
        BattleSystem.Instance.SetStatChanges(Stat.DEF, unit.defenseStat, false, targets);
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }

   public override void OnEnded(Unit otherUnit, float storedValue, bool DoFancyStatChanges)
    {
        if (DoFancyStatChanges)
        {
            BattleSystem.Instance.SetStatChanges(Stat.DEF, -storedValue, false, targets);
        }
           
        else
            targets.defenseStat -= (int)storedValue;
    }
}
