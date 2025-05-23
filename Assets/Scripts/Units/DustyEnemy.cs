﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using static ActionTypeButton;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.CanvasScaler;

public class DustyEnemy : Unit
{
    [SerializeField]
    private GameObject TutorialIcon3;
    void Awake()
    {
        unitName = "Dusty";
        maxHP = 40;
        attackStat = 3;
        defenseStat = 8;
        actionCostMultiplier = 1f;
        currentHP = maxHP;
        IsPlayerControlled = false;
        resistances = new DamageType[] { DamageType.SLASH };
        weaknesses = new DamageType[] { DamageType.STRIKE };
        behavior = this.gameObject.AddComponent<DustyBehavior>();
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
                if (turn == 0)
                {
                    var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
                    foreach (var skill in Aurelia.skillUIs)
                    {
                        var actionContainer = skill.GetComponent<ActionContainer>();
                        actionContainer.Disabled = false;
                        actionContainer.button.interactable = true;
                    }
                        baseUnit.BattlePhaseEnd += PrePipTutorial;
                }

                if (turn == 2)
                {
                    baseUnit.BattlePhaseEnd -= PrePipTutorial;
                    baseUnit.BattlePhaseEnd += PipTutorial;

                }

                CombatTools.SetupEnemyAction(baseUnit, turn);
                turn++;
            }
            else
            {
                baseUnit.BattlePhaseClose += EndThisMF;
            }
        }

        private static IEnumerator LateDisable()
        {
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            if (!Aurelia.actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault()))
            {
                Aurelia.actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault());
                BattleSystem.Instance.SetupHUD(Aurelia, null);
            }

            yield return new WaitForSeconds(0.2f);
            foreach (var skill in Aurelia.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null && actionContainer.action.ActionName != "Sweep")
                {
                    actionContainer.Disabled = true;
                    actionContainer.button.interactable = false;
                }
            }
            Aurelia.OnActionSelected += DisableNonSweepOptions;
            Aurelia.OnPerformActionStarted += RevertActions;
            yield break;
        }

        private static void RevertActions(Unit unit)
        {
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            Aurelia.OnActionSelected += MakeSureDefendStaysEnabled;
            for (int i = 0; i < 50; i++)
            {
                Aurelia.OnActionSelected -= DisableNonSweepOptions;
                foreach (var skill in Aurelia.skillUIs)
                {
                    var actionContainer = skill.GetComponent<ActionContainer>();
                    if (actionContainer.action != null && actionContainer.action.ActionName == "Defend")
                    {
                        actionContainer.Disabled = false;
                        actionContainer.button.interactable = true;
                    }
                }
                Aurelia.OnPerformActionStarted -= RevertActions;
            }
           
        }
        private static void MakeSureDefendStaysEnabled(Unit unit, ActionContainer container)
        {
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            foreach (var skill in Aurelia.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null && actionContainer.action.ActionName == "Defend")
                {
                    actionContainer.Disabled = false;
                    actionContainer.button.interactable = true;
                }
            }
        }
        private static void DisableNonSweepOptions(Unit unit, ActionContainer container)
        {
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            foreach (var skill in Aurelia.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null && actionContainer.action.ActionName != "Sweep")
                {
                    actionContainer.Disabled = true;
                    actionContainer.button.interactable = false;
                }
            }
        }

        private void PrePipTutorial(Unit obj)
        {
            obj.BattlePhaseEnd -= PrePipTutorial;
            BattleSystem.Instance.canvas.gameObject.SetActive(false);
            BattleSystem.Instance.BattlePhasePause = true;
            StartCoroutine(PrePipTutorialText());
        }

        private IEnumerator blackScreenFadeCoroutine;
        private IEnumerator PrePipTutorialText()
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaPrePipExplanation"), true, false, false, false, true, false);
            yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
            StartCoroutine(AudioManager.Instance.Fade(0f, AudioManager.Instance.currentMusicTrack, 2, false));
            BattleSystem.Instance.BattlePhasePause = true;
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0);
            Director.Instance.blackScreen.transform.SetAsLastSibling();
            Director.Instance.blackScreen.gameObject.SetActive(true);

            if (blackScreenFadeCoroutine != null)
                StopCoroutine(blackScreenFadeCoroutine);

            blackScreenFadeCoroutine = Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, false);
            Director.Instance.StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, false));
            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a >= 1);
            yield return new WaitForSeconds(0.1f);

            var TutorialText = Director.Instance.PipTutorialText;
            TutorialText.gameObject.SetActive(true);
            var TextColor = new Color(0, 237, 255, 0);

            TutorialText.fontSharedMaterial.SetColor("_GlowColor", TextColor);
            TutorialText.fontSharedMaterial.SetFloat("_GlowPower", 0.5f);
            TutorialText.color = TextColor;
            TutorialText.text = "Burn quickly...";
            AudioManager.QuickPlay("low_hum_001");

            Director.Instance.StartCoroutine(Tools.FadeText(TutorialText, 0.001f, true, false));
            yield return new WaitForSeconds(2f);

            Director.Instance.StartCoroutine(Tools.FadeText(TutorialText, 0.01f, false, true));
            Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "low_hum_001", 2f, true));
            yield return new WaitUntil(() => TutorialText.color.a <= 0);

            TutorialText.gameObject.SetActive(false);

            if (blackScreenFadeCoroutine != null)
                StopCoroutine(blackScreenFadeCoroutine);

            blackScreenFadeCoroutine = Tools.FadeObject(Director.Instance.blackScreen, 0.001f, false, false);

            Director.Instance.StartCoroutine(blackScreenFadeCoroutine);
            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a <= 0);
            Director.Instance.blackScreen.gameObject.SetActive(false);
            StartCoroutine(AudioManager.Instance.Fade(0.35f, AudioManager.Instance.currentMusicTrack, 2, false));
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");

            Aurelia.OnActionSelected += DisableNonSlash;
            Aurelia.OnActionSelected += ForceLightAction;
            Aurelia.BattlePhaseEnd += GetRidOfLightAction;

            foreach (var skill in Aurelia.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null && actionContainer.action.ActionName != "Slash")
                {
                    actionContainer.Disabled = true;
                    actionContainer.button.interactable = false;
                }
            }

            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaForcedLightAction"), true, false, false, false, true, false);
            yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
            BattleSystem.Instance.canvas.gameObject.SetActive(true);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
            BattleSystem.Instance.playerUnits[0].StartDecision();
            BattleSystem.Instance.BattlePhasePause = false;



        }

        private static void DisableNonSlash(Unit unit, ActionContainer container)
        {
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            foreach (var skill in Aurelia.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null && actionContainer.action.ActionName != "Slash")
                {
                    actionContainer.Disabled = true;
                    actionContainer.button.interactable = false;
                    break;
                }
            }
        }


        private void ForceLightAction(Unit unit, ActionContainer actionContainerParent)
        {
            actionContainerParent.lightButton.ModifyAction();
        }

        private IEnumerator DelayedLightChange(Unit unit)
        {
            unit.spotLight.intensity = 1;
            yield return new WaitForSeconds(0.1f);
            unit.spotLight.intensity = 1;
        }

        private void GetRidOfLightAction(Unit unit)
        {
            unit.OnActionSelected -= ForceLightAction;
        }

        private void PipTutorial(Unit obj)
        {
            obj.BattlePhaseEnd -= PipTutorial;
            BaseUnit.knockbackModifider = 25;
            turn = 3;
            BattleSystem.Instance.canvas.gameObject.SetActive(false);
            BattleSystem.Instance.BattlePhasePause = true;
            StartCoroutine(PipTutorialText());
        }

        private IEnumerator PipTutorialText()
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaHitByLightAttack"), true, false, false, false, true, false);
            yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
            BattleSystem.Instance.BattlePhasePause = true;         
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0);
            Director.Instance.blackScreen.transform.SetAsLastSibling();
            Director.Instance.blackScreen.gameObject.SetActive(true);

            if (blackScreenFadeCoroutine != null)
                StopCoroutine(blackScreenFadeCoroutine);

            blackScreenFadeCoroutine = Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, false);
            StartCoroutine(blackScreenFadeCoroutine);
            StartCoroutine(AudioManager.Instance.Fade(0f, AudioManager.Instance.currentMusicTrack, 2, false));
            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a >= 1);
            yield return new WaitForSeconds(0.1f);

            var TutorialText = Director.Instance.PipTutorialText;
            TutorialText.gameObject.SetActive(true);
            var TextColor = new Color(255, 0, 0, 0);
            TutorialText.fontSharedMaterial.SetColor("_GlowColor", TextColor);
            TutorialText.fontSharedMaterial.SetFloat("_GlowPower", 0.5f);
            TutorialText.color = TextColor;
            TutorialText.text = "...Burn stronger...";
            AudioManager.QuickPlay("low_hum_001");

            Director.Instance.StartCoroutine(Tools.FadeText(TutorialText, 0.01f, true, false));
            yield return new WaitForSeconds(2f);

            Director.Instance.StartCoroutine(Tools.FadeText(TutorialText, 0.01f, false, true));
            Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "low_hum_001", 2f, true));
            yield return new WaitUntil(() => TutorialText.color.a <= 0);

            TutorialText.gameObject.SetActive(false);


            if (blackScreenFadeCoroutine != null)
                StopCoroutine(blackScreenFadeCoroutine);

            blackScreenFadeCoroutine = Tools.FadeObject(Director.Instance.blackScreen, 0.001f, false, false);

            Director.Instance.StartCoroutine(blackScreenFadeCoroutine);

            Director.Instance.UnlockedPipSystem = true;
            Director.Instance.timeline.pipCounter.gameObject.SetActive(true);
            CombatTools.ReturnPipCounter().pipCount = 0;
            foreach (Transform pip in CombatTools.ReturnPipCounter().gameObject.transform)
            {
                Destroy(pip.gameObject);
            }




            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a <= 0);
            StartCoroutine(AudioManager.Instance.Fade(0.35f, AudioManager.Instance.currentMusicTrack, 2, false));
            Director.Instance.blackScreen.gameObject.SetActive(false);

            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            Aurelia.OnActionSelected += ForceHeavyAction;
            Aurelia.BattlePhaseEnd += GetRidOfHeavyAction;


            BattleSystem.Instance.BattlePhasePause = false;
            Director.Instance.StartCoroutine(LateDisable());

            yield return new WaitForSeconds(0.1f);
            BattleSystem.Instance.canvas.gameObject.SetActive(true);
            var tutorialIcon = Instantiate(GetComponent<DustyEnemy>().TutorialIcon3, Director.Instance.canvas.transform);
            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
            tutorialIcon.GetComponent<MoveableObject>().Move(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
            Director.Instance.blackScreen.gameObject.SetActive(true);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);


        }

        private void ForceHeavyAction(Unit unit, ActionContainer actionContainerParent)
        {
            actionContainerParent.heavyButton.ModifyAction();
            Director.Instance.StartCoroutine(DelayedLightChange(unit));
        }


        private void GetRidOfHeavyAction(Unit unit)
        {
            unit.OnActionSelected -= ForceHeavyAction;
        }



        private void EndThisMF(Unit obj)
        {
            StartCoroutine(ForceBattleEnd());
        }

        private IEnumerator ForceBattleEnd()
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaMeeting(2)"), true, false, false, true);
            BattleSystem.Instance.DoPostBattleDialogue = false;
            yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING); 
            CombatTools.TurnOffCriticalUI(BaseUnit);
            StartCoroutine(BattleSystem.Instance.TransitionToMap(true));
            LabCamera.Instance.uicam.gameObject.SetActive(true);
            BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
            MapController.Instance.ReEnteredMap += BattleLog.Instance.DoPostBattleDialouge;
            SceneManager.sceneLoaded += AddDusty;
            BattleSystem.Instance.StopUpdating = true;
          
        }

        private void AddDusty(Scene scene, LoadSceneMode mode)
        {
            Director.AddUnitToParty("Dusty");
            SceneManager.sceneLoaded -= AddDusty;

        }
    }
}
