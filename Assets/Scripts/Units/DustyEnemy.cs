using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.CanvasScaler;

public class DustyEnemy : Unit
{

    void Awake()
    {
        unitName = "Dusty";
        maxHP = 40;
        attackStat = 5;
        defenseStat = 8;
        speedStat = 9;
        actionCostMultiplier = 1f;
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior = this.gameObject.AddComponent<DustyBehavior>();
        StartingStamina = 75;
        if (!Director.Instance.DevMode)
            BattleStarted += DoCharacterText;
        IsHidden = true;
    }
    private void DoCharacterText(Unit obj)
    {
        Tools.PauseAllStaminaTimers();
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaMeeting(1)"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        BattlePostStarted -= DoCharacterText;
    }

    public class DustyBehavior : EnemyBehavior
    {
        private int turn;
        private BattleSystem battlesystem;
        private Unit BaseUnit;

        public override IEnumerator DoBehavior(Unit baseUnit)
        {
            battlesystem = BattleSystem.Instance;
            BaseUnit = baseUnit;
            var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
            print(turn);
            if (turn != 3)
            {
                if (battlesystem.numOfUnits[num].IsPlayerControlled)
                {
                    Tools.DetermineActionData(baseUnit, turn, num);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[turn], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, turn, num);
                    baseUnit.stamina.DoCost(baseUnit.actionList[turn].cost);
                    battlesystem.AddAction(baseUnit.actionList[turn]);
                    turn++;
                }
                else
                {
                    StartCoroutine(Tools.RepeatBehavior(baseUnit));
                }
            }
            else
            {
                baseUnit.BattlePhaseClose += EndThisMF;
            }
        }

        private void EndThisMF(Unit obj)
        {
            StartCoroutine(ForceBattleEnd());
        }

        private IEnumerator ForceBattleEnd()
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaMeeting(2)"), true, false, false, true);
            BattleSystem.Instance.DoPostBattleDialogue = false;
            yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
            StartCoroutine(BattleSystem.Instance.TransitionToMap(true));
            LabCamera.Instance.uicam.gameObject.SetActive(false);
            BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
            Tools.AddNewActionToUnit(BattleSystem.Instance.playerUnits[0], "Sweep");
            MapController.Instance.ReEnteredMap += BattleLog.Instance.DoPostBattleDialouge;
            SceneManager.sceneLoaded += AddDusty;
            Tools.PauseAllStaminaTimers();
            BattleSystem.Instance.StopUpdating = true;
            Tools.TurnOffCriticalUI(BaseUnit);  
        }

        private void AddDusty(Scene scene, LoadSceneMode mode)
        {
            Director.AddUnitToParty("Dusty");
            SceneManager.sceneLoaded -= AddDusty;

        }
    }
}
