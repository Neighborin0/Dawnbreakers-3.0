using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class TutorialEnemy : Unit
{
    [SerializeField]
    private GameObject TutorialIcon1;
    [SerializeField]
    private GameObject TutorialIcon2;
    void Awake()
    {
        unitName = "Husk";
        maxHP = 24;
        attackStat = 0;
        defenseStat = 0;
        //speedStat = 0;
        currentHP = maxHP;
        IsPlayerControlled = false;
    }
    void Start()
    {
        behavior = gameObject.AddComponent<TutorialEnemyBehavior>();
        BattleStarted += CreateTutorialIcon;
        GetComponent<TutorialEnemyBehavior>().TutorialIcon2 = TutorialIcon2;
    }

    private void CreateTutorialIcon(Unit obj)
    {
        StartCoroutine(TutorialDialogue(obj));
    }

    private IEnumerator TutorialDialogue(Unit obj)
    {
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("AureliaTutorialHuskEncounter(1)"), true, false);
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        Tools.ToggleUiBlocker(false, true);
        yield return new WaitForSeconds(0.3f);
        if (Director.Instance.DevMode != true)
        {
            var tutorialIcon = Instantiate(TutorialIcon1, Director.Instance.canvas.transform);
            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
            tutorialIcon.GetComponent<MoveableObject>().Move(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
            Director.Instance.blackScreen.gameObject.SetActive(true);
        }
        //BattleSystem.Instance.SetTempEffect(this, "revitalize", false, 0, 0, 0);
        BattleStarted -= CreateTutorialIcon;
    }

    public class TutorialEnemyBehavior : EnemyBehavior
    {
        [SerializeField]
        public GameObject TutorialIcon2;
        bool DisabledAction = false;
        public override void DoBehavior(Unit baseUnit)
        {

            var battlesystem = BattleSystem.Instance;
            var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
            //So player can't die during Tutorial Fight
            if (Aurelia.currentHP <= 10)
            {
                CombatTools.SetupEnemyAction(baseUnit, 2);
            }
            else
            {
                if (turn == 1)
                {
                    if (!Aurelia.actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault()))
                    {

                        Aurelia.actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault());
                        battlesystem.SetupHUD(Aurelia, null);
                        if (Director.Instance.DevMode != true)
                        {
                            Tools.ToggleUiBlocker(false, true);
                            var tutorialIcon = Instantiate(TutorialIcon2, Director.Instance.canvas.transform);
                            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
                            tutorialIcon.GetComponent<MoveableObject>().Move(true);
                        }                       
                    }

                    Director.Instance.StartCoroutine(LateDisable());

                }
                else
                {
                    foreach (var skill in Aurelia.skillUIs)
                    {
                        var actionContainer = skill.GetComponent<ActionContainer>();
                        if (actionContainer.action != null && actionContainer.action.ActionName == "Defend")
                        {
                            actionContainer.Disabled = true;
                            actionContainer.button.interactable = false;
                        }
                        else
                        {
                            actionContainer.Disabled = false;
                            actionContainer.button.interactable = true;
                        }

                    }
                }
                CombatTools.SetupEnemyAction(baseUnit, turn);
                if (turn != 1)
                {
                    turn += 1;
                }
                else
                {
                    turn = 0;
                }
            }
        }

    }

    private static IEnumerator LateDisable()
    {
        yield return new WaitForSeconds(0.2f);
        var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
        Aurelia.OnActionSelected += DisableSlash;
        Aurelia.BattlePhaseEnd += RevertActions;
        foreach (var skill in Aurelia.skillUIs)
        {
            var actionContainer = skill.GetComponent<ActionContainer>();
            if (actionContainer.action != null && actionContainer.action.ActionName != "Defend")
            {
                actionContainer.Disabled = true;
                actionContainer.button.interactable = false;
            }
            else
            {
                actionContainer.Disabled = false;
                actionContainer.button.interactable = true;
            }

        }
    }

    private static void RevertActions(Unit unit)
    {
        var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
        Aurelia.OnActionSelected -= DisableSlash;
        Aurelia.OnActionSelected += DisableDefend;
        foreach (var skill in Aurelia.skillUIs)
        {
            var actionContainer = skill.GetComponent<ActionContainer>();
            if (actionContainer.action != null && actionContainer.action.ActionName == "Defend")
            {
                actionContainer.Disabled = true;
                actionContainer.button.interactable = false;
            }
            else
            {
                actionContainer.Disabled = false;
                actionContainer.button.interactable = true;
            }

        }
    }

    private static void DisableSlash(Unit unit, ActionContainer container)
    {
        var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
        foreach (var skill in Aurelia.skillUIs)
        {
            var actionContainer = skill.GetComponent<ActionContainer>();
            if (actionContainer.action != null && actionContainer.action.ActionName != "Defend")
            {
                actionContainer.Disabled = true;
                actionContainer.button.interactable = false;
                break;
            }
        }
    }

    private static void DisableDefend(Unit unit, ActionContainer container)
    {
        var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
        foreach (var skill in Aurelia.skillUIs)
        {
            var actionContainer = skill.GetComponent<ActionContainer>();
            if (actionContainer.action != null && actionContainer.action.ActionName == "Defend")
            {
                actionContainer.Disabled = true;
                actionContainer.button.interactable = false;
            }
            else
            {
                actionContainer.Disabled = false;               
            }

        }
    }

    private static void RevertDefend(Unit unit)
    {
        var Aurelia = CombatTools.CheckAndReturnNamedUnit("Aurelia");
        Aurelia.OnActionSelected -= DisableSlash;
        Aurelia.OnActionSelected -= DisableDefend;
        Aurelia.BattlePhaseEnd -= RevertDefend;
        Aurelia.BattlePhaseEnd -= RevertActions;
    }
}
