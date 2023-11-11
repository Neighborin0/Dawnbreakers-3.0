using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "Mend", menuName = "Assets/Actions/Mend")]
public class Mend : Action
{
    private void OnEnable()
    {
        ActionName = "Mend";
        damage = 2;
        cost = 30f;
        limited = true;
        numberofUses = 3;
        statAmount = 5;
        targetType = TargetType.ALLIES;
        actionType = ActionType.STATUS;
        
    }

    public override string GetDescription()
    {
        description = $"Heals allies by {statAmount} <sprite name=\"HP\">.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, Vector3.zero,0,8, -40, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.HP, statAmount, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
