using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "Summon", menuName = "Assets/Actions/Summon")]
public class Summon : Action
{
    private void OnEnable()
    {
        ActionName = "Summon";
        cost = 75f;
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
    }

    public override string GetDescription()
    {
        description = $"Summons a fearsome foe...";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        if (CombatTools.DetermineAllies(unit).Count < 3)
        {
            var summon = Instantiate(Director.Instance.Unitdatabase.Where(obj => obj.name == unit.summonables[UnityEngine.Random.Range(0, unit.summonables.Length)]).SingleOrDefault());
            summon.IsSummon = true;
            if (unit.IsPlayerControlled)
            {
                summon.IsPlayerControlled = true;
                foreach (var BSP in BattleSystem.Instance.playerPositions)
                {
                    var BattlePoint = BSP.GetComponent<BattleSpawnPoint>();
                    if(!BattlePoint.Occupied)
                    {
                        summon.transform.position = BSP.position;
                        summon.transform.SetParent(BSP.transform);
                        summon.transform.localScale = new Vector3(9f, 9f, 9f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        BattleSystem.Instance.playerUnits.Add(summon);
                        break;
                    }
                   
                }
            }
            else
            {
                foreach (var BSP in BattleSystem.Instance.enemyPositions)
                {
                    var BattlePoint = BSP.GetComponent<BattleSpawnPoint>();
                    if (!BattlePoint.Occupied)
                    {
                        summon.transform.position = BSP.position;
                        summon.transform.SetParent(BSP.transform);
                        summon.transform.localScale = new Vector3(9f, 9f, 9f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        BattleSystem.Instance.enemyUnits.Add(summon);
                        break;
                    }

                }
            }
            BattleSystem.Instance.numOfUnits.Add(summon);
            LabCamera.Instance.ReadjustCam();
        }
        Done = true;
        yield break;
    }

  
}
