using System;
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
                if(turn == 0)
                {
                    baseUnit.BattlePhaseEnd += PrePipTutorial;
                }

                if(turn == 2) 
                {
                    baseUnit.BattlePhaseEnd += PipTutorial;
                    baseUnit.knockbackModifider = 50;
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
            yield return new WaitForSeconds(0.2f);
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            foreach (var skill in Aurelia.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null && actionContainer.action.ActionName != "Sweep")
                {
                    actionContainer.Disabled = true;
                    actionContainer.button.interactable = false;
                    break;
                }
            }
        }

        private void PrePipTutorial(Unit obj)
        {
            obj.BattlePhaseClose -= PrePipTutorial;
            BattleSystem.Instance.BattlePhasePause = true;
            StartCoroutine(PrePipTutorialText());
        }
        private IEnumerator PrePipTutorialText()
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaPrePipExplanation"), true, false, false, false, true, false);
           yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
            BattleSystem.Instance.BattlePhasePause = true;
            Director.Instance.blackScreen.color = new Color(0,0,0,0);
            Director.Instance.blackScreen.transform.SetAsLastSibling();
            Director.Instance.blackScreen.gameObject.SetActive(true);
            StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, false));   
            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a >= 1);
            yield return new WaitForSeconds(0.1f);

            var TutorialText = Director.Instance.PipTutorialText;
            TutorialText.gameObject.SetActive(true);
            var TextColor = new Color(0, 237, 255, 0);
            TutorialText.fontSharedMaterial.SetColor("_GlowColor", TextColor);
            TutorialText.fontSharedMaterial.SetFloat("_GlowPower", 0.5f);
            TutorialText.color = TextColor;
            TutorialText.text = "Burn quickly...";

            StartCoroutine(Tools.FadeText(TutorialText, 0.001f, true, false));
            yield return new WaitForSeconds(2f);

            StartCoroutine(Tools.FadeText(TutorialText, 0.01f, false, true));
            yield return new WaitUntil(() => TutorialText.color.a <= 0);
            StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, true));

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

            Aurelia.OnActionSelected += ForceLightAction;
            Aurelia.BattlePhaseEnd += GetRidOfLightAction;

            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaForcedLightAction"), true, false, false, false, true, false);
            yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
            BattleSystem.Instance.playerUnits[0].StartDecision();
            BattleSystem.Instance.BattlePhasePause = false;


        }

        private void ForceLightAction(Unit unit, ActionContainer actionContainerParent)
        {
            actionContainerParent.lightButton.ModifyAction();
        }

        private void GetRidOfLightAction(Unit unit)
        {
            unit.OnActionSelected -= ForceLightAction;
        }

        private void PipTutorial(Unit obj)
        {
            obj.BattlePhaseClose -= PipTutorial;
            BattleSystem.Instance.BattlePhasePause = true;
            StartCoroutine(PipTutorialText());
        }

        private IEnumerator PipTutorialText()
        {
            BattleSystem.Instance.BattlePhasePause = true;
            foreach (var player in BattleSystem.Instance.playerUnits)
            {
                if (player.state != PlayerState.DECIDING)
                    player.state = PlayerState.IDLE;
            }

            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0);
            Director.Instance.blackScreen.transform.SetAsLastSibling();
            Director.Instance.blackScreen.gameObject.SetActive(true);
            StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, false));
            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a >= 1);
            yield return new WaitForSeconds(0.1f);

            var TutorialText = Director.Instance.PipTutorialText;
            TutorialText.gameObject.SetActive(true);
            var TextColor = new Color(255, 0, 0, 0);
            TutorialText.fontSharedMaterial.SetColor("_GlowColor", TextColor);
            TutorialText.fontSharedMaterial.SetFloat("_GlowPower", 0.5f);
            TutorialText.color = TextColor;
            TutorialText.text = "Burn brighter...";

            StartCoroutine(Tools.FadeText(TutorialText, 0.01f, true, false));
            yield return new WaitForSeconds(2f);

            StartCoroutine(Tools.FadeText(TutorialText, 0.01f, false, true));
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, true, true));

            Director.Instance.UnlockedPipSystem = true;
            Director.Instance.timeline.pipCounter.gameObject.SetActive(true);
            CombatTools.ReturnPipCounter().pipCount = 0;
            foreach (Transform pip in CombatTools.ReturnPipCounter().gameObject.transform)
            {
                Destroy(pip.gameObject);
            }

            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            if (!Aurelia.actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault()))
            {
                Aurelia.actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Sweep").SingleOrDefault());
                BattleSystem.Instance.SetupHUD(Aurelia, null);
            }

            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a <= 0);
            Director.Instance.StartCoroutine(LateDisable());
            Aurelia.OnActionSelected += ForceHeavyAction;
            Aurelia.BattlePhaseEnd += GetRidOfHeavyAction;

            var tutorialIcon = Instantiate(GetComponent<DustyEnemy>().TutorialIcon3, Director.Instance.canvas.transform);
            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
            tutorialIcon.GetComponent<MoveableObject>().Move(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
            Director.Instance.blackScreen.gameObject.SetActive(true);

        }

        private void ForceHeavyAction(Unit unit, ActionContainer actionContainerParent)
        {
            var action = actionContainerParent.action;
            var newAction = Instantiate(action);
            CombatTools.ReturnPipCounter().TakePip();
            newAction.actionStyle = Action.ActionStyle.HEAVY;
            actionContainerParent.action = newAction;
            actionContainerParent.UpdateOnStyleSwitch();
            actionContainerParent.SetStyleLight(true);

            var target = actionContainerParent.baseUnit;
            var Light = actionContainerParent.baseUnit.GetComponentInChildren<Light>();
            Color heavyColor = new(225, 27, 0);
            Light.color = heavyColor * 0.01f;
            Light.intensity = 1f;
            StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatUpVFX", heavyColor, heavyColor, new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y, 0), Quaternion.identity, float.PositiveInfinity, 0, true, 0, 0.1f, 0.01f));
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
