﻿using System;
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

    private string[] fallbackSummons =
   {
        "Husk",
   };

    IEnumerator moveCoroutine;
    public override IEnumerator ExecuteAction()
    {
        int numofUnitsToAdd = 3 - Tools.DetermineAllies(unit).Count;
        Director.Instance.StartCoroutine(Tools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(unit, 0f, 10, -55, false, 0.5f);
        unit.spotLight.color = new Color(1, 0.5409836f, 0, 1);
        unit.ChangeUnitsLight(unit.spotLight, 150, 15, 0.04f, 0.1f);
        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "Beacon", new Color(1, 0.5409836f, 0, 0), new Color(1, 0.5409836f, 0, 0), new Vector3(-3.21f, 5.7f, 0f), 1f, 0, true, 0, 4));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < numofUnitsToAdd; i++)
        {
            unit.summonables ??= fallbackSummons;

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
                        summon.GetComponent<Rigidbody>().mass = 10000;
                        BattleSystem.Instance.playerUnits.Add(summon);
                        BattlePoint.Occupied = true;
                        BattlePoint.unit = summon;
                        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(summon.gameObject, "Summon", Color.yellow, Color.yellow, new Vector3(0, 0, -2f), 10f));
                        LabCamera.Instance.MoveToUnit(summon, 0f, 15, -55, false, 0.5f);
                        summon.spotLight.color = new Color(1, 0.5409836f, 0, 1);
                        summon.ChangeUnitsLight(summon.spotLight, 150, 15, 0.04f, 0.1f);
                        yield return new WaitForSeconds(0.5f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        summon.stamina.Paused = true;
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
                        BattleSystem.Instance.playerUnits.Add(summon);
                        BattlePoint.Occupied = true;
                        BattlePoint.unit = summon;
                        BattleSystem.Instance.StartCoroutine(Tools.PlayVFX(summon.gameObject, "Summon", Color.yellow, Color.yellow,  new Vector3(0, 0, -2f), 10f));
                        LabCamera.Instance.MoveToUnit(summon, 0f, 15, -55, false, 0.5f);
                        summon.spotLight.color = new Color(1, 0.5409836f, 0, 1);
                        summon.ChangeUnitsLight(summon.spotLight, 150, 15, 0.04f, 0.1f);
                        yield return new WaitForSeconds(0.5f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        summon.stamina.Paused = true;
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }

                }
            }
            BattleSystem.Instance.numOfUnits.Add(summon);  
            yield return new WaitForSeconds(0.5f);
        }
        Director.Instance.StartCoroutine(Tools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        Done = true;
        yield break;
    }


}