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
        //speedStat = 10;
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior = this.gameObject.AddComponent<MatriarchBehaviorLV0>();
        summonables = TutorialSummons;
        IsHidden = true;
        resistances = new DamageType[] { DamageType.STRIKE};
        weaknesses = new DamageType[] { DamageType.SLASH, DamageType.PIERCE,};
        BattleStarted += DoCharacterText;
        OnPlayerUnitDeath += Gloat;
    }

    private void DoCharacterText(Unit obj)
    {
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("MatriarchIntro"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        BattleSystem.Instance.SetTempEffect(this, "INDOMITABLE", false, 0, 0, 0);
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
        StartCoroutine(Director.Instance.timeline.ResetTimeline());
        OnPlayerUnitDeath -= Gloat;
    }
}
public class MatriarchBehaviorLV0 : EnemyBehavior
{
    private BattleSystem battlesystem;
    private Unit BaseUnit;
    int move = 0;

    public override void DoBehavior(Unit baseUnit)
    {
        battlesystem = BattleSystem.Instance;
        BaseUnit = baseUnit;
        //Spawns an enemy
        switch (turn)
        {
            case 0:
                {
                    CombatTools.SetupEnemyAction(baseUnit, turn);
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
                    CombatTools.SetupEnemyAction(baseUnit, move);
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
                    CombatTools.SetupEnemyAction(baseUnit, move);
                    turn++;
                }
                break;
            //Destroys Dusty and taunts player
            case 3:
                {
                    baseUnit.BattlePhaseClose += PreDustyDeath;
                    CombatTools.SetupEnemyAction(baseUnit, turn, CombatTools.CheckAndReturnNamedUnit("Dusty"));
                    turn++;
                }
                break;
            //Destroys Aurelia
            case 4:
                {
                    CombatTools.SetupEnemyAction(baseUnit, 3, CombatTools.CheckAndReturnNamedUnit("Aurelia"));
                    turn = 1;
                }
                break;

        }
    }

    private void PreDustyDeath(Unit obj)
    {
        obj.BattlePhaseClose -= PreDustyDeath;
        StartCoroutine(PreIncinerateText());
    }
    private IEnumerator PreIncinerateText()
    {
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("MatriarchPreIncinerate"), true, false);
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        foreach(var player in BattleSystem.Instance.playerUnits)
        {
            if (player.state != PlayerState.DECIDING)
                player.state = PlayerState.IDLE;
        }
        Director.Instance.timeline.ResetTimeline();

    }
}

