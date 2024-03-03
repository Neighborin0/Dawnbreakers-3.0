using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Awaken", menuName = "Assets/Actions/Awaken")]
public class Awaken : Action
{
    private void OnEnable()
    {
        ActionName = "Awaken";


        
        cost = 75f;
        lightCost = 80f;
        heavyCost = 100f;


        damageText = damage.ToString();
        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;
        description = "...";

        Done = false;
    }

    public override string GetDescription()
    {
        description = "...";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f, false, true);
        AudioManager.QuickPlay("ui_woosh_002");
        yield return new WaitForSeconds(0.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(10));
        AudioManager.QuickPlay("glint_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "AwakenLight", Color.white, Color.white, new Vector3(-1.95f, 4.02f, 0f), Quaternion.identity, 10f, 0, true, 0, 8));
        yield return new WaitForSeconds(0.9f);
        var Light = targets.spotLight;
        targets.ChangeUnitsLight(Light, 150, 15, Color.red, 0.04f, 0.9f);
        LabCamera.Instance.Shake(1f, 0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 5f, false, targets);
        yield return new WaitForSeconds(1.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
        AudioManager.QuickPlay("ui_woosh_002");
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

}
