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


        
        cost = 100f;
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
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 8, -40, 0.5f);
        AudioManager.QuickPlay("ui_woosh_002");
        yield return new WaitForSeconds(0.3f);
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        AudioManager.QuickPlay("glint_001");
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "AwakenLight", Color.white, Color.white, new Vector3(-1.95f, 4.02f, 0f), Quaternion.identity, 10f, 0, true, 0, 8));
        yield return new WaitForSeconds(0.9f);
        var Light = targets.spotLight;
        targets.ChangeUnitsLight(Light, 150, 15, Color.red, 0.04f, 1.6f);
        LabCamera.Instance.Shake(1f, 0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, 5f, false, targets);
        yield return new WaitForSeconds(1f);
        Light.color = Color.blue;
        LabCamera.Instance.Shake(1f, 0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, 5f, false, targets);
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        AudioManager.QuickPlay("ui_woosh_002");
        LabCamera.Instance.ResetPosition();
        this.Done = true;
    }

}
