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
        actionCostMultiplier = 1000f;
        IsPlayerControlled = false;
        StartingStamina = 50f;
    }
    void Start()
    {
        behavior = this.gameObject.AddComponent<TutorialEnemyBehavior>();
        this.BattleStarted += CreateTutorialIcon;
        this.GetComponent<TutorialEnemyBehavior>().TutorialIcon2 = TutorialIcon2;
    }

    private void CreateTutorialIcon(Unit obj)
    {
        if (Director.Instance.DevMode != true)
        {
            var tutorialIcon = Instantiate(TutorialIcon1, Director.Instance.canvas.transform);
            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
            StartCoroutine(Tools.SmoothMoveUI(tutorialIcon.GetComponent<RectTransform>(), 0, 0, 0.01f));
        }
        BattleSystem.Instance.SetTempEffect(this, "revitalize", false, 0, 0, 0);
        this.BattleStarted -= CreateTutorialIcon;
    }

    public class TutorialEnemyBehavior : EnemyBehavior
    {
        private int turn;
        [SerializeField]
        public GameObject TutorialIcon2;
        bool DisabledAction = false;
        public override IEnumerator DoBehavior(Unit baseUnit)
        {

            yield return new WaitUntil(() => baseUnit.stamina != null);
            var battlesystem = BattleSystem.Instance;
            var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
            if (battlesystem.numOfUnits[num].IsPlayerControlled)
            {

                Tools.DetermineActionData(baseUnit, turn, num);
                battlesystem.DisplayEnemyIntent(baseUnit.actionList[turn], baseUnit);
                battlesystem.numOfUnits[num].skillUIs[0].GetComponent<ActionContainer>().Disabled = false;
                baseUnit.state = PlayerState.READY;
                if (turn == 1)
                {
                    Debug.LogWarning(DisabledAction);
                    if (!battlesystem.numOfUnits[num].actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault()))
                    {
                        battlesystem.numOfUnits[num].actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault());
                        battlesystem.SetupHUD(battlesystem.numOfUnits[num], null);
                        battlesystem.numOfUnits[num].stamina.slider.value = 50f;
                        battlesystem.numOfUnits[num].skillUIs[0].GetComponent<ActionContainer>().Disabled = true;
                        if (Director.Instance.DevMode != true)
                        {
                            var tutorialIcon = Instantiate(TutorialIcon2, Director.Instance.canvas.transform);
                            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
                            StartCoroutine(Tools.SmoothMoveUI(tutorialIcon.GetComponent<RectTransform>(), 0, 0, 0.01f));
                            tutorialIcon.transform.localScale = new Vector3(1f, 1f, 1);
                        }   
                    }
                }
                yield return new WaitUntil(() => baseUnit.stamina.slider.value == baseUnit.stamina.slider.maxValue);
                Tools.DetermineActionData(baseUnit, turn, num);
                baseUnit.stamina.DoCost(baseUnit.actionList[turn].cost);
                battlesystem.AddAction(baseUnit.actionList[turn]);
                if (turn != 1)
                {
                    turn += 1;
                }
                else
                {
                    turn = 0;
                }
            }
            else
            {
                StartCoroutine(Tools.RepeatBehavior(baseUnit));
            }
        }
    }
}
