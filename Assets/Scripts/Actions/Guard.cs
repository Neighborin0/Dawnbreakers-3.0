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

[CreateAssetMenu(fileName = "Guard", menuName = "Assets/Actions/Guard")]
public class Guard : Action
{
    private void OnEnable()
    {
        ActionName = "Guard";
        cost = 30f;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.ALLIES;
        duration = 5f;
    }

    public override string GetDescription()
    {
        description = $"Applies +{unit.defenseStat} <sprite name=\"FORTIFY\"> for {duration} seconds.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(targets.gameObject, "Defend_001", Color.blue, new Vector3(0, 5, -2f), 0.2f)); 
        LabCamera.Instance.MoveToUnit(targets, 0, -6, 32, false, 0.5f);
        yield return new WaitForSeconds(0.8f);
        var Light = targets.GetComponentInChildren<Light>();
        Light.color = Color.blue;
        /*foreach(Transform icon in targets.namePlate.IconGrid.transform)
        {
            var EI = icon.gameObject.GetComponent<EffectIcon>();
            if(EI.iconName == "Fortify")
            {
                EI.DoFancyStatChanges = false;
                EI.DestoryEffectIcon();
                break;
            }
        }
        */
        BattleSystem.Instance.SetTempEffect(targets, "DEF", true, duration, unit.defenseStat);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, unit.defenseStat, false, targets);
        yield return new WaitForSeconds(1.3f);
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
