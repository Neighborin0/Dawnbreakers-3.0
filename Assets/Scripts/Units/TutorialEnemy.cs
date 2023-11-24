using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : Unit
{
    [SerializeField]
    private GameObject TutorialIcon1;
    [SerializeField]
    private GameObject TutorialIcon2;
    void Awake()
    {
        unitName = "Husk";
        maxHP = 30;
        attackStat = 5;
        defenseStat = 0;
        speedStat = 0;
        currentHP = maxHP;
        IsPlayerControlled = false;
        Tools.ModifyAction(this, "Awaken", 0, 100f);
        Tools.ModifyAction(this, "Strike", 1, 100f);
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
        }
        //BattleSystem.Instance.SetTempEffect(this, "revitalize", false, 0, 0, 0);
        BattleStarted -= CreateTutorialIcon;
    }

    public class TutorialEnemyBehavior : EnemyBehavior
    {
        private int turn;
        [SerializeField]
        public GameObject TutorialIcon2;
        bool DisabledAction = false;
        public override void DoBehavior(Unit baseUnit)
        {

            var battlesystem = BattleSystem.Instance;
            var Aurelia = Tools.CheckAndReturnNamedUnit("Aurelia");
            //So player can't die during Tutorial Fight
            if (Aurelia.currentHP <= 10)
            {
                Tools.SetupEnemyAction(baseUnit, 2);
            }
            else
            {

                Aurelia.skillUIs[0].GetComponent<ActionContainer>().Disabled = false;
                if (turn == 1)
                {
                    if (!Aurelia.actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault()))
                    {
                        Aurelia.actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault());
                        battlesystem.SetupHUD(Aurelia, null);
                        Aurelia.skillUIs[0].GetComponent<ActionContainer>().Disabled = true;
                        if (Director.Instance.DevMode != true)
                        {
                            Tools.ToggleUiBlocker(false, true);
                            var tutorialIcon = Instantiate(TutorialIcon2, Director.Instance.canvas.transform);
                            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
                            tutorialIcon.GetComponent<MoveableObject>().Move(true);
                        }
                    }
                }
                Tools.SetupEnemyAction(baseUnit, turn);
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
}
