using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering.PostProcessing;
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

    private string[] fallbackSummons =
   {
        "Husk",
   };

    public override IEnumerator ExecuteAction()
    {
        int numofUnitsToAdd = 3 - Tools.DetermineAllies(unit).Count;
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(unit, Vector3.zero, 0f, 10, -55, 0.5f);
        unit.ChangeUnitsLight(unit.spotLight, 150, 15, new Color(1, 0.5409836f, 0, 1), 0.04f, 0.1f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "BeaconLine", new Color(1, 0.5409836f, 0, 0), new Color(1, 0.5409836f, 0, 0), new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 8));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "BeaconCircle", new Color(1, 0.5409836f, 0, 0), new Color(1, 0.5409836f, 0, 0), new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 8));
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "Beacon2", new Color(1, 0.5409836f, 0, 0), new Color(1, 0.5409836f, 0, 0), new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 8));
        
      
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < numofUnitsToAdd; i++)
        {
            unit.summonables ??= fallbackSummons;

            var summon = Instantiate(Director.Instance.Unitdatabase.Where(obj => obj.name == unit.summonables[UnityEngine.Random.Range(0, unit.summonables.Length)]).SingleOrDefault());
            summon.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", Color.yellow * 10);

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
                        summon.GetComponent<Rigidbody>().mass = 10000;
                        BattleSystem.Instance.playerUnits.Add(summon);
                        BattlePoint.Occupied = true;
                        BattlePoint.unit = summon;
                        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(summon.gameObject, "Summon", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), 10f));
                        LabCamera.Instance.MoveToUnit(summon, Vector3.zero ,0f, 15, -55, 0.5f);
                        summon.ChangeUnitsLight(summon.spotLight, 150, 15, new Color(1, 0.5409836f, 0, 1), 0.04f, 0.1f);
                        yield return new WaitForSeconds(0.5f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }

                }
            }
            else
            {
                foreach (var BSP in BattleSystem.Instance.enemyPositions)
                {
                    var BattlePoint = BSP.GetComponent<BattleSpawnPoint>();
                    summon.GetComponent<SpriteRenderer>().flipX = true;
                    if (!BattlePoint.Occupied)
                    {
                        summon.transform.position = BSP.position;
                        summon.transform.SetParent(BSP.transform);
                        summon.transform.localScale = new Vector3(9f, 9f, 9f);     
                        summon.GetComponent<Rigidbody>().mass = 10000;
                        BattleSystem.Instance.enemyUnits.Add(summon);
                        BattlePoint.Occupied = true;
                        BattlePoint.unit = summon;
                        
                        
                        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(summon.gameObject, "Summon", Color.yellow, Color.yellow,  new Vector3(0, 0, -2f), 10f));
                        LabCamera.Instance.MoveToUnit(summon, Vector3.zero, 0f, 15, -55, 0.5f);
                        BattleSystem.Instance.StartCoroutine(Tools.ChangeObjectEmissionToMinIntensity(summon.gameObject, 0.01f));
                        summon.ChangeUnitsLight(summon.spotLight, 150, 15, new Color(1, 0.5409836f, 0, 1), 0.04f, 0.1f);
                        yield return new WaitForSeconds(0.5f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }

                }
            }
            BattleSystem.Instance.numOfUnits.Add(summon);  
            yield return new WaitForSeconds(0.2f);
        }
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        Done = true;
        yield break;
    }


}
