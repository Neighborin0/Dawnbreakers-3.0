using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enrage", menuName = "Assets/Actions/Enrage")]
public class Enrage : Action
{
    private void OnEnable()
    {
        ActionName = "Enrage";
        damageText = damage.ToString();


        cost = 25f;
        lightCost = 5f;
        heavyCost = 45f;



        statAmount = 2;
        lightStatAmount = 1;
        heavyStatAmount = 3;

        actionType = ActionType.STATUS;
        targetType = TargetType.SELF;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Raises <sprite name=\"ATK RED2\"> by {CombatTools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(targets.gameObject, "Smoke", new Color(48, 1, 0), new Color(48, 1, 0), new Vector3(0, unit.GetComponent<SpriteRenderer>().bounds.max.y - 1, -2f), Quaternion.identity, 2, 0, false, 3, -1));
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.ATK, CombatTools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
