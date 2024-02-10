using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Screech", menuName = "Assets/Actions/Screech")]
public class Screech : Action
{
    private void OnEnable()
    {
        ActionName = "Screech";
        damage = 2;
        cost = 10f;
        lightCost = 0f;
        heavyCost = 30f;
        damageText = damage.ToString();
        targetType = TargetType.ENEMY;
        actionType = ActionType.STATUS;
        statAmount = 2;
        lightStatAmount = 1;
        heavyStatAmount = 3;

        Done = false;
    }

    public override string GetDescription()
    {
        description = $"Decreases <sprite name=\"DEF BLUE2\"> by {CombatTools.DetermineTrueActionValue(this)}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(2));
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero, 0, 12, -70, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCry", new Color(0, 0, 1, 0.1f), new Color(0, 0, 1, 0.1f),  new Vector3(0, 0, -2f), Quaternion.identity, 1.1f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCryParticles", new Color(0, 0, 1), new Color(0, 0, 1), new Vector3(0, 0, -2f), Quaternion.identity, 1.1f));
        yield return new WaitForSeconds(0.2f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCry", new Color(0, 0, 1, 0.1f), new Color(0, 0, 1, 0.1f), new Vector3(0, 0, -2f), Quaternion.identity, 1.1f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCryParticles", new Color(0, 0, 1), new Color(0, 0, 1), new Vector3(0, 0, -2f), Quaternion.identity, 1.1f));
        yield return new WaitForSeconds(0.2f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCry", new Color(0, 0, 1, 0.1f), new Color(0, 0, 1, 0.1f), new Vector3(0, 0, -2f), Quaternion.identity, 1.1f));
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "WarCryParticles", new Color(0, 0, 1), new Color(0, 0, 1), new Vector3(0, 0, -2f), Quaternion.identity, 1.1f));
        yield return new WaitForSeconds(0.4f);
        BattleSystem.Instance.SetStatChanges(Stat.DEF, -CombatTools.DetermineTrueActionValue(this), false, targets);
        yield return new WaitForSeconds(1f);
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(2));
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
