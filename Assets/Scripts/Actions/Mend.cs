using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "Mend", menuName = "Assets/Actions/Mend")]
public class Mend : Action
{
    private void OnEnable()
    {
        ActionName = "Mend";
        damage = 2;
        accuracy = 1;
        cost = 30f;
        statAmount = 2;
        targetType = TargetType.ALLIES;
        damageText = damage.ToString();
        actionType = ActionType.STATUS;
    }

    public override string GetDescription()
    {
        description = $"Decreases <sprite name=\"SPD YLW\"> by {statAmount}.";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        LabCamera.Instance.MoveToUnit(targets, 0, -8, 40, false, 0.5f);
        yield return new WaitForSeconds(0.3f);
        BattleSystem.Instance.SetStatChanges(Stat.SPD, -statAmount, false, targets);
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        this.Done = true;
        yield break;
    }
}
