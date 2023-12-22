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
        //speedStat = 9;
        actionCostMultiplier = 1f;
        currentHP = maxHP;
        IsPlayerControlled = false;
        resistances = new DamageType[] { DamageType.SLASH };
        weaknesses = new DamageType[] { DamageType.STRIKE };
        behavior = this.gameObject.AddComponent<DustyBehavior>();
        if (!Director.Instance.DevMode)
            BattleStarted += DoCharacterText;
        IsHidden = true;
    }

    public void Start()
    {
        var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
        foreach (var skill in Aurelia.skillUIs)
        {
            var actionContainer = skill.GetComponent<ActionContainer>();        
            actionContainer.Disabled = false;
            actionContainer.button.interactable = true;
        }
    }
    private void DoCharacterText(Unit obj)
    {
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaMeeting(1)"), true, false);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
        }
        BattlePostStarted -= DoCharacterText;
    }

    public class DustyBehavior : EnemyBehavior
    {
        private Unit BaseUnit;

        public override void DoBehavior(Unit baseUnit)
        {
            BaseUnit = baseUnit;
            if (turn != 3)
            {
                if(turn == 1) 
                {
                    Director.Instance.UnlockedPipSystem = true;
                    Director.Instance.timeline.pipCounter.gameObject.SetActive(true);
                    CombatTools.ReturnPipCounter().ResetPips();
                }

                if (turn == 2)
                {
                    var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
                    if (!Aurelia.actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault()))
                    {

                        Aurelia.actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault());
                        BattleSystem.Instance.SetupHUD(Aurelia, null);
                        foreach (var skill in Aurelia.skillUIs)
                        {
                            var actionContainer = skill.GetComponent<ActionContainer>();
                            if (actionContainer.action.ActionName != "Sweep")
                            {
                                actionContainer.Disabled = true;
                                actionContainer.button.interactable = false;
                            }
                        }
                    }
                }
                CombatTools.SetupEnemyAction(baseUnit, turn);
                turn++;
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
            MapController.Instance.ReEnteredMap += BattleLog.Instance.DoPostBattleDialouge;
            SceneManager.sceneLoaded += AddDusty;
            //Tools.PauseStaminaTimer();
            BattleSystem.Instance.StopUpdating = true;
            CombatTools.TurnOffCriticalUI(BaseUnit);
        }

        private void AddDusty(Scene scene, LoadSceneMode mode)
        {
            Director.AddUnitToParty("Dusty");
            SceneManager.sceneLoaded -= AddDusty;

        }
    }
}
