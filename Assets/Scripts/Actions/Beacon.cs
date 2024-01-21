using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
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
        lightCost = 75f;
        heavyCost = 75f;

        targetType = TargetType.SELF;
        actionType = ActionType.STATUS;
        damageText = damage.ToString();

        Done = false;
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
        int numofUnitsToAdd = 3 - CombatTools.DetermineAllies(unit).Count;
        Director.Instance.StartCoroutine(CombatTools.TurnOffDirectionalLight(0.01f));
        LabCamera.Instance.MoveToUnit(unit, Vector3.zero, 0f, 10, -55, 0.5f);
        unit.ChangeUnitsLight(unit.spotLight, 150, 15, new Color(1, 0.86f, 0.55f), 0.04f, 0.1f);
        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "BeaconLight", new Color(1, 0.86f, 0.55f), new Color(1, 0.86f, 0.55f), new Vector3(-2.95f, 5.02f, 0f), Quaternion.identity, 10f, 0, true, 0, 8));
        AudioManager.QuickPlay("glint_001");



        yield return new WaitForSeconds(1f);
        for (int i = 0; i < numofUnitsToAdd; i++)
        {
            unit.summonables ??= fallbackSummons;

            var summon = Instantiate(Director.Instance.Unitdatabase.Where(obj => obj.name == unit.summonables[UnityEngine.Random.Range(0, unit.summonables.Length)]).SingleOrDefault());
            summon.transform.localScale = new Vector3(9f, 9f, 9f);
            summon.GetComponent<BoxCollider>().isTrigger = true;
            summon.GetComponent<Rigidbody>().useGravity = false;
           
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
                        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(summon.gameObject, "SummonVFX", new Color(1, 0.86f, 0.55f), new Color(1, 0.86f, 0.55f), new Vector3(0, -1.82f, -2f), Quaternion.identity, 10f, 0, false, 0, 10, 0.0001f, "statUp_Loop_001"));
                        LabCamera.Instance.MoveToUnit(summon, Vector3.zero ,0f, 15, -55, 0.5f);
                        Director.Instance.StartCoroutine(Tools.ChangeObjectEmissionToMinIntensity(summon.gameObject, 0.01f));
                        summon.ChangeUnitsLight(summon.spotLight, 150, 15, new Color(1, 0.5409836f, 0, 1), 0.04f, 0.1f);
                        yield return new WaitForSeconds(0.5f);
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        yield return new WaitForSeconds(0.5f);
                        summon.OverrideEmission = false;
                        break;

                    }

                }
            }
            else
            {
                foreach (var BSP in BattleSystem.Instance.enemyPositions)
                {
                    var BattlePoint = BSP.GetComponent<BattleSpawnPoint>();
                    summon.transform.position = new Vector3(BSP.position.x, -7, BSP.position.z);
                    summon.GetComponent<BoxCollider>().isTrigger = false;
                    summon.GetComponent<Rigidbody>().useGravity = false;
                    summon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                    summon.GetComponent<SpriteRenderer>().flipX = true;
                    LabCamera.Instance.MoveToUnit(summon, Vector3.zero, 0, 15, -40, 0.5f);
                    if (!BattlePoint.Occupied)
                    {
                        AudioManager.QuickPlay("summon_001");
                        BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(BSP.gameObject, "SummonVFX", new Color(1, 0.86f, 0.55f), new Color(1, 0.86f, 0.55f), new Vector3(0, -1.82f, 0), Quaternion.identity, 10f, 0, false, 0, 10, 0.0001f, "statUp_Loop_001"));
                        var UnitLight = summon.spotLight;
                        UnitLight.transform.position = new Vector3(BSP.position.x, BSP.position.y, BSP.position.z - 0.28f);
                        LabCamera.Instance.MoveToUnit(unit, Vector3.zero, 0, 12, -70, 0.5f);
                        summon.ChangeUnitsLight(UnitLight, 150, 15, new Color(1, 0.86f, 0.55f), 0.04f, 2.4f);
                        yield return new WaitForSeconds(1f);
                        Director.Instance.StartCoroutine(Tools.SmoothMoveObjectVertically(summon.gameObject.transform, 7.8f, 0.1f));
                        yield return new WaitForSeconds(1f);  
                        BattleSystem.Instance.enemyUnits.Add(summon);
                        BattlePoint.Occupied = true;
                        BattlePoint.unit = summon;

                       
                        yield return new WaitForSeconds(1f);
                        summon.transform.SetParent(BSP.transform);
                        summon.transform.localScale = new Vector3(9f, 9f, 9f);
                        summon.GetComponent<Rigidbody>().mass = 10000;
                        BattleSystem.Instance.SetupHUD(summon, BSP);
                        summon.unitName = CombatTools.CheckNames(summon);
                        Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "statUp_Loop_001", 0.5f, true));
                        break;
                    }

                }
            }
            BattleSystem.Instance.numOfUnits.Add(summon);  
            yield return new WaitForSeconds(0.2f);
        }
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.01f));
        LabCamera.Instance.ResetPosition();
        Done = true;
        yield break;
    }


}
