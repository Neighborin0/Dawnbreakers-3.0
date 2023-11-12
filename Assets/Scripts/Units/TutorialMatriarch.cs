using System;
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
        IsHidden = true;
        BattleStarted += DoCharacterText;
        OnPlayerUnitDeath += Gloat;
    }
    private void DoCharacterText(Unit obj)
    {
        Tools.PauseStaminaTimer();
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
        Tools.UnpauseStaminaTimer();
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
        //Tools.PauseStaminaTimer();
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("MatriarchPreIncinerate"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        /*foreach (var x in BattleSystem.Instance.playerUnits)
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
            Tools.UnpauseStaminaTimer();
        }
        */
    }
    public override void DoBehavior(Unit baseUnit)
    {
        battlesystem = BattleSystem.Instance;
        BaseUnit = baseUnit;
        //Spawns an enemy
        switch (turn)
        {
            case 0:
                {
                    Tools.SetupEnemyAction(baseUnit, turn);
                    turn++;
                }
                break;
            case 1:
                //Attacks or Buffs
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
                    Tools.SetupEnemyAction(baseUnit, move);
                    turn++;
                }
                break;
            //Attacks or Buffs, if an enemy is dead, has a chance to spawn another enemy
            case 2:
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
                    Tools.SetupEnemyAction(baseUnit, move);
                    turn++;
                }
                break;
            //Destroys Dusty and taunts player
            case 3:
                {
                    StartCoroutine(PreIncinerateText());
                    Tools.SetupEnemyAction(baseUnit, turn, Tools.CheckAndReturnNamedUnit("Dusty"));
                    turn++;
                }
                break;
            //Destroys Aurelia
            case 4:
                {
                    Tools.SetupEnemyAction(baseUnit, 3, Tools.CheckAndReturnNamedUnit("Aurelia"));
                    turn = 1;
                }
                break;

        }
    }
}

