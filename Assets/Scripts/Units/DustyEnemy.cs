﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DustyEnemy : Unit
{
   
  
    void Awake()
    {
        unitName = "Dusty";
        maxHP = 40;
        attackStat = 5;
        defenseStat = 9;
        speedStat = 10;
        actionCostMultiplier = 1f;
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior =  this.gameObject.AddComponent<DustyBehavior>();

        if(!Director.Instance.DevMode)
        BattleStarted += DoCharacterText;
    }

    private void Start()
    {
        if (!Director.Instance.DevMode)
            this.stamina.Paused = true;
    }
    private void DoCharacterText(Unit obj)
    {
        Tools.PauseAllStaminaTimers();
        BattleLog.CharacterDialog(ConvserationHandler.DustyAureliaMeeting1 ,true, false);
        foreach(var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        BattleStarted -= DoCharacterText;
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
            BattleLog.CharacterDialog(ConvserationHandler.DustyAureliaMeeting2, true, false);
            yield return new WaitUntil(() => !BattleSystem.Instance.BL.characterdialog.IsActive());
            Director.Instance.BL.Move(false);
            if (!BattleSystem.Instance.playerUnits[0].actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault()))
            {
                Tools.AddNewActionToUnit(BattleSystem.Instance.playerUnits[0], "Sweep");
                BattleSystem.Instance.playerUnits[0].EnteredMap += BattleLog.Instance.DoPostBattleDialouge;
            }
            SceneManager.sceneLoaded += AddDusty;
            Tools.PauseAllStaminaTimers();
            BattleSystem.Instance.StopUpdating = true;
            Tools.TurnOffCriticalUI(BaseUnit);
            StartCoroutine(BattleSystem.Instance.TransitionToMap(true));
        }

        private void AddDusty(Scene scene, LoadSceneMode mode)
        {
            Director.AddUnitToParty("Dusty");   
            SceneManager.sceneLoaded -= AddDusty;

        }
    }
}