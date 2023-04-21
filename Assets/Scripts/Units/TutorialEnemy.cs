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
    }
    void Start()
    {
        behavior =  this.gameObject.AddComponent<TutorialEnemyBehavior>();
        this.stamina.Paused = true;
        this.OnDamaged += Damaged;
        this.BattleStarted += CreateTutorialIcon;
        StartCoroutine(BattleSystem.Instance.SetTempEffect(this, "revitalize", null));
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
        this.BattleStarted -= CreateTutorialIcon;
    }

    private void Damaged(Unit obj)
    {
        obj.ActionEnded += SetupRevitalize;
        obj.OnDamaged -= Damaged;
        
    }

    private void SetupRevitalize(Unit obj)
    {
        foreach (var x in obj.namePlate.IconGrid.GetComponentsInChildren<Image>())
        {
            Destroy(x);
            BattleSystem.Instance.DoTextPopup(this, "Revitalize", Color.yellow);
        }
        obj.BattlePhaseEnd += RefillStamina;
        obj.ActionEnded -= SetupRevitalize;
    }
    private void RefillStamina(Unit obj)
    {
        this.stamina.slider.value = this.stamina.slider.maxValue;
        this.BattlePhaseEnd -= RefillStamina;
    }

    public class TutorialEnemyBehavior : EnemyBehavior
    {
        private int turn;
        [SerializeField]
        public GameObject TutorialIcon2;
        public override IEnumerator DoBehavior(Unit baseUnit)
        {
          
            yield return new WaitUntil(() => baseUnit.stamina != null);
            var battlesystem = BattleSystem.Instance;
            var num = UnityEngine.Random.Range(0, battlesystem.numOfUnits.Count);
            if (battlesystem.numOfUnits[num].IsPlayerControlled)
            {
              
                Tools.DetermineActionData(baseUnit, turn, num);
                battlesystem.DisplayEnemyIntent(baseUnit.actionList[turn], baseUnit);
                baseUnit.state = PlayerState.READY;   
                if(turn == 1)
                {
                    if (!battlesystem.numOfUnits[num].actionList.Contains(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault()))
                    {
                        battlesystem.numOfUnits[num].actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == "Defend").SingleOrDefault());
                        battlesystem.SetupHUD(battlesystem.numOfUnits[num], null);
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
