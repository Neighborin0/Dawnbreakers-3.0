using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "Beacon", menuName = "Assets/Actions/Beacon")]
public class Beacon : Action
{
    private void OnEnable()
    {
        ActionName = "Beacon";
        cost = 75f;
        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();
    }

    public override string GetDescription()
    {
        description = $"Attracts fearsome enemies...";
        return description;
    }
    public override IEnumerator ExecuteAction()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Tools.DetermineAllies(unit).Count < 3)
            {
                var summon = Instantiate(Director.Instance.Unitdatabase.Where(obj => obj.name == unit.summonables[UnityEngine.Random.Range(0, unit.summonables.Length)]).SingleOrDefault());
                summon.IsSummon = true;
                if (unit.IsPlayerControlled)
                {
                    summon.IsPlayerControlled = true;
                    foreach (var BSP in BattleSystem.Instance.playerPositions)
                    {
                        var BattlePoint = BSP.GetComponent<BattleSpawnPoint>();
                        if (!BattlePoint.Occupied)
                        {
                            summon.transform.position = BSP.position;
                            summon.transform.SetParent(BSP.transform);
                            summon.transform.localScale = new Vector3(9f, 9f, 9f);
                            BattleSystem.Instance.SetupHUD(summon, BSP);
                            BattleSystem.Instance.playerUnits.Add(summon);
                            summon.stamina.Paused = true;
                            BattlePoint.Occupied = true;
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
                            summon.stamina.Paused = true;
                            BattlePoint.Occupied = true;
                            break;
                        }

                    }
                }
                BattleSystem.Instance.numOfUnits.Add(summon);
                LabCamera.Instance.ReadjustCam();
            }
            yield return new WaitForSeconds(0.5f);
        }
        Done = true;
        yield break;
    }

  
}
