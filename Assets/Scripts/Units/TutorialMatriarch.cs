using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMatriarch : Unit
{
    private string[] TutorialSummons = new string[] { "Husk" };

    void Awake()
    {
        unitName = "Matriarch";
        maxHP = 168;
        attackStat = 12;
        defenseStat = 0;
        speedStat = 10;
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior = this.gameObject.AddComponent<MatriarchBehaviorLV0>();
        summonables = TutorialSummons;
        StartingStamina = 60;
        IsHidden = true;
        BattleStarted += DoCharacterText;
        OnPlayerUnitDeath += Gloat;
    }
    private void DoCharacterText(Unit obj)
    {
        Tools.PauseAllStaminaTimers();
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("MatriarchIntro"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        BattlePostStarted -= DoCharacterText;
    }

    private void Gloat(Unit obj)
    {
        StartCoroutine(GloatRoutine());
    }

    private IEnumerator GloatRoutine()
    {
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyPostDeath"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        Tools.UnpauseAllStaminaTimers();
        OnPlayerUnitDeath -= Gloat;
    }
}
    public class MatriarchBehaviorLV0 : EnemyBehavior
    {
        private int turn = 0;
        private BattleSystem battlesystem;
        private Unit BaseUnit;
        int move = 0;

    private IEnumerator PreIncinerateText()
    {
        Tools.PauseAllStaminaTimers();
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("MatriarchPreIncinerate"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        foreach (var x in BattleSystem.Instance.playerUnits)
        {
            if (x.stamina.slider.value == x.stamina.slider.maxValue)
            {
                BattleSystem.Instance.state = BattleStates.DECISION_PHASE;
                x.StartDecision();
                break;
            }

        }
        if (BattleSystem.Instance.state != BattleStates.DECISION_PHASE && BattleSystem.Instance.state != BattleStates.WON && BattleSystem.Instance.state != BattleStates.DEAD && BattleSystem.Instance.state != BattleStates.TALKING && BattleSystem.Instance.enemyUnits.Count > 0 && BattleSystem.Instance.playerUnits.Count > 0)
        {
            BattleSystem.Instance.state = BattleStates.IDLE;
            Tools.UnpauseAllStaminaTimers();
        }

    }
    public override IEnumerator DoBehavior(Unit baseUnit)
        {
            battlesystem = BattleSystem.Instance;
            BaseUnit = baseUnit;
            var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
            //Spawns an enemy
            if (battlesystem.numOfUnits[num].IsPlayerControlled)
            {
                if (turn == 0)
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
                //Attacks or Buffs
                else if (turn == 1)
                {
                    float prob = 0.7f;
                    if (UnityEngine.Random.Range(0f, 1f) < prob)
                    {
                        //Bash
                        move = 1;
                    }
                    else
                    {
                        //Kindle
                        move = 2;
                    }
                    Tools.DetermineActionData(baseUnit, move, num);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[move], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, move, num);
                    baseUnit.stamina.DoCost(baseUnit.actionList[move].cost);
                    battlesystem.AddAction(baseUnit.actionList[move]);
                    turn++;
                }
                //Attacks or Buffs, if an enemy is dead, has a chance to spawn another enemy
                else if (turn == 2)
                {
                    if (BattleSystem.Instance.enemyUnits.Count <= 2)
                    {
                        float prob1 = 0.25f;
                        float prob2 = 0.50f;
                        if (UnityEngine.Random.Range(0f, 1f) < prob1)
                        {
                            //Summon
                            move = 4;
                            print(baseUnit.actionList[4]);
                        }
                        else
                        {
                            if (UnityEngine.Random.Range(0f, 1f) < prob2 && move != 2)
                            {
                                move = 2;
                                print(baseUnit.actionList[2]);
                            }
                            else
                            {
                                //Bash
                                move = 1;
                                print(baseUnit.actionList[1]);
                            }
                        }
                    }
                    else
                    {
                        float prob = 0.7f;
                        if (UnityEngine.Random.Range(0f, 1f) < prob && move != 2)
                        {
                            move = 1;
                        }
                        else
                        {
                            move = 2;
                        }
                    }
                    Tools.DetermineActionData(baseUnit, move, num);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[move], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, move, num);
                    baseUnit.stamina.DoCost(baseUnit.actionList[move].cost);
                    battlesystem.AddAction(baseUnit.actionList[move]);
                    turn++;
                }
                //Destroys Dusty and taunts player
                else if (turn == 3)
                {
                    StartCoroutine(PreIncinerateText());
                    Tools.DetermineActionData(baseUnit, turn, num, true, BattleSystem.Instance.playerUnits[1]);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[turn], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, turn, num, true, BattleSystem.Instance.playerUnits[1]);
                    baseUnit.stamina.DoCost(baseUnit.actionList[turn].cost);
                    battlesystem.AddAction(baseUnit.actionList[turn]);
                    turn++;
                }
                //Destroys Aurelia
                else if (turn == 4)
                {
                    Tools.DetermineActionData(baseUnit, 3, num, true, BattleSystem.Instance.playerUnits[0]);
                    battlesystem.DisplayEnemyIntent(baseUnit.actionList[3], baseUnit);
                    baseUnit.state = PlayerState.READY;
                    yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                    Tools.DetermineActionData(baseUnit, 3, num, true, BattleSystem.Instance.playerUnits[0]);
                    baseUnit.stamina.DoCost(baseUnit.actionList[3].cost);
                    battlesystem.AddAction(baseUnit.actionList[3]);
                    turn = 1;
                }
            }
            else
            {
                StartCoroutine(Tools.RepeatBehavior(baseUnit));
            }
        }
    }

    