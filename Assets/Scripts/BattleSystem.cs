using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public enum BattleStates { START, DECISION_PHASE, BATTLE, WON, DEAD, IDLE, TALKING, WAITING }
public class BattleSystem : MonoBehaviour
{
    public struct StatStorer
    {
        public string unitName;
        public int DEF;
        public int ATK;
        public int SPD;
        public int HP;
    }

    public static BattleSystem Instance { get; private set; }
    //public Transform BattleOrderpos;
    public GameObject statPopUp;
    public ActionContainer genericActionContainer;
    public GameObject canvasParent;


    public List<Transform> playerPositions;
    public List<Transform> enemyPositions;

    public Healthbar hp;
    public StaminaBar staminaBar;
    public NamePlate namePlate;
    public IntentContainer intent;
    public Canvas canvas;
    //public LineRenderer targetLine;
    public GameObject dot;
    public bool HasStarted = false;
    public bool BattlePhasePause = false;

    public List<Action> QueuedActions;
    public List<Unit> speedlist;
    public List<Action> ActionsToPerform;
    public List<Action> PriorityActions;
    public List<Unit> numOfUnits;
    public List<Unit> playerUnits;
    public List<Unit> enemiesToLoad;
    public List<Unit> enemyUnits;
    public List<StatStorer> statStorers;
    public bool StopUpdating = false;
    public GridLayoutGroup ActionLayout;
    public Volume effectsSetting;
    public Light mainLight;
    public float mainLightValue;

    public Vector3 cameraPos1Units;
    public Vector3 cameraPos2Units;
    public Vector3 cameraPos3Units;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        if (BattleSystem.Instance.effectsSetting.sharedProfile.TryGet<ChromaticAberration>(out var CA))
        {
            CA.intensity.value = 0f;
        }
    }
    void Start()
    {
        state = BattleStates.START;
        LabCamera.Instance.state = LabCamera.CameraState.SWAY;
        StartBattle();
    }
    void Update()
    {
        if (state != BattleStates.WON | state != BattleStates.DEAD)
        {
            if (playerUnits.Count == 0 && enemyUnits.Count != 0)
            {
                state = BattleStates.DEAD;
                print("you lose");
            }
            else if (enemyUnits.Count == 0 && playerUnits.Count != 0)
            {
                state = BattleStates.WON;
                if (!StopUpdating)
                {
                    Tools.EndAllTempEffectTimers();
                    StopUpdating = true;
                    if (battleCo != null)
                        StopCoroutine(battleCo);

                    battleCo = TransitionToMap();
                    StartCoroutine(battleCo);
                    print("You win");
                }
            }
        }
        if (state == BattleStates.DECISION_PHASE && Input.GetMouseButtonUp(1))
        {
            BattleLog.Instance.ResetBattleLog();
        }

        if (Input.GetKeyDown(KeyCode.U) && Director.Instance.DevMode)
        {
            playerUnits[UnityEngine.Random.Range(0, playerUnits.Count - 1)].health.TakeDamage(99999, null);
        }
    }

    public BattleStates state;
  

    void StartBattle()
    {
        try
        {
            statStorers = new List<StatStorer>();
            Director.Instance.timeline.gameObject.SetActive(true);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(false);
            for (int i = 0; i <= playerUnits.Count - 1; i++)
            {
                playerUnits[i].gameObject.SetActive(true);
                playerUnits[i].transform.localScale = new Vector3(9f, 9f, 9f);
                playerUnits[i].transform.position = playerPositions[i].position;
                playerUnits[i].transform.SetParent(playerPositions[i].transform);
                var BSP = playerPositions[i].GetComponent<BattleSpawnPoint>();
                BSP.unit = playerUnits[i];
                BSP.Occupied = true;
                numOfUnits.Add(playerUnits[i]);
                var ss = new StatStorer
                {
                    unitName = playerUnits[i].unitName,
                    HP = playerUnits[i].maxHP,
                    ATK = playerUnits[i].attackStat,
                    DEF = playerUnits[i].defenseStat,
                    SPD = playerUnits[i].speedStat,
                };
                statStorers.Add(ss);
            }
            for (int i = 0; i <= enemiesToLoad.Count - 1; i++)
            {
                var enemy = Instantiate(enemiesToLoad[i], enemyPositions[i]);
                enemy.transform.localScale = new Vector3(9f, 9f, 9f);
                enemiesToLoad[i].gameObject.SetActive(true);
                var BSP = enemyPositions[i].GetComponent<BattleSpawnPoint>();
                BSP.unit = enemiesToLoad[i];
                BSP.Occupied = true;
                enemyUnits.Add(enemy);
                numOfUnits.Add(enemy);
            }
            StartCoroutine(Transition());
            BattleLog.Instance.ClearAllBattleLogText();
            print(state);
        }
        catch(Exception ex)
        {
           Debug.LogException(ex);
           Debug.LogWarning("So this is were the error happening");
        }
    }

    IEnumerator Transition()
    {
        yield return new WaitForSeconds(1f);
        foreach (var unit in Tools.GetAllUnits())
        {
            SetupHUD(unit, unit.transform);
        }
        StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));

        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        LabCamera.Instance.ReadjustCam();
        yield return new WaitForSeconds(1.5f);
        BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
        Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
        BattleLog.Instance.ResetBattleLog();
        this.canvas.gameObject.SetActive(true);
        state = BattleStates.DECISION_PHASE;
        foreach (var unit in Tools.GetAllUnits())
        {
            if (!unit.IsPlayerControlled)
            {
                StartCoroutine(unit.behavior.DoBehavior(unit));
            }
            unit.StaminaHighlightIsDisabled = false;
            unit.DoBattleStarted();
        }
        yield return new WaitForSeconds(0.5f);
        OptionsManager.Instance.blackScreen.gameObject.SetActive(false);
        playerUnits[0].StartDecision();
        LabCamera.Instance.MovingTimeDivider = 1;
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.DoPostBattleStarted();
        }
    }

   
    public IEnumerator TransitionToMap(bool LevelUpScreen = true)
    {
        yield return new WaitForSeconds(1f);
        foreach (Transform child in Director.Instance.timeline.transform)
        {
            if(child.GetComponent<TimeLineChild>() != null)
            Destroy(child.gameObject);
        }
        Director.Instance.timeline.gameObject.SetActive(false);
        Director.Instance.party.Clear();
        for (int i = 0; i < playerPositions.Count; i++)
        {
            playerPositions[i].DetachChildren();
        }
        foreach (var unit in playerUnits)
        {      
            if (!unit.IsSummon)
            {
                unit.DoBattleEnded();
                unit.StaminaHighlightIsDisabled = true;
                unit.stamina.slider.value = unit.stamina.slider.maxValue;
                unit.state = PlayerState.IDLE;
                foreach (var i in statStorers)
                {
                    if (i.unitName == unit.unitName)
                    {
                        unit.attackStat = i.ATK;
                        unit.maxHP = i.HP;
                        unit.defenseStat = i.DEF;
                        unit.speedStat = i.SPD;
                    }
                }
                Tools.TurnOffCriticalUI(unit);
                Director.Instance.party.Add(unit);
                DontDestroyOnLoad(unit.gameObject);
            }
        }
        if (LevelUpScreen)
        {
            Director.Instance.DisplayCharacterTab();
            LabCamera.Instance.MoveToUnit(playerUnits[0], 0, 8, 40, false, 0.5f);
        }
        else
        {
            OptionsManager.Instance.Load("MAP2");
            yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
            foreach (var unit in Tools.GetAllUnits())
            {
                unit.StaminaHighlightIsDisabled = true;
                unit.gameObject.SetActive(false);
            }
        }
    }

    public bool CheckPlayableState()
    {
        bool check = false;
        if (BattleSystem.Instance.state != BattleStates.BATTLE && BattleSystem.Instance.state != BattleStates.START && BattleSystem.Instance.state != BattleStates.WON && BattleSystem.Instance.state != BattleStates.DEAD && BattleSystem.Instance.state != BattleStates.TALKING)
            check = true;
        else
            check = false;
        return check;
    }

    public void DisplayEnemyIntent(Action action, Unit unit)
    {
        unit.intentUI.textMesh.text = action.ActionName;
        if(action.damage != 0)
        unit.intentUI.damageNums.text = " <sprite name=\"ATK\">" + (action.damage + unit.attackStat - action.targets.defenseStat).ToString();
        unit.intentUI.action = action;
        unit.intentUI.costNums.text = action.cost * unit.actionCostMultiplier < 100 ? $"{action.cost * unit.actionCostMultiplier}%" : $"100%";
        if (unit.intentUI.action.actionType == Action.ActionType.STATUS)
        {
            unit.intentUI.damageParent.SetActive(false);
        }
        else
        {
            unit.intentUI.damageParent.SetActive(true);
        }
        unit.intentUI.gameObject.SetActive(true);
        Tools.SetImageColorAlphaToZero(unit.intentUI.GetComponent<Image>());
        Tools.SetTextColorAlphaToZero(unit.intentUI.textMesh);
        Tools.SetTextColorAlphaToZero(unit.intentUI.damageNums);
        Tools.SetTextColorAlphaToZero(unit.intentUI.costNums);
        StartCoroutine(Tools.FadeObject(unit.intentUI.GetComponent<Image>(), 0.005f, true, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.textMesh, 0.005f, true, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.damageNums, 0.005f, true, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.costNums, 0.005f, true, false));

    }
    public void SetStatChanges(Stat statToRaise, float AmountToRaise, bool multiplicative, Unit target)
    {
        if (BattleSystem.Instance != null)
        {
            var battleSystem = BattleSystem.Instance;
            var popup = Instantiate(battleSystem.statPopUp, new Vector3(target.GetComponent<SpriteRenderer>().bounds.center.x, target.GetComponent<SpriteRenderer>().bounds.center.y + 2, target.transform.position.z), Quaternion.identity);
            battleSystem.StartCoroutine(battleSystem.ChangeStat(statToRaise, AmountToRaise, multiplicative, target, popup.GetComponent<LabPopup>()));
            BattleLog.Instance.StartCoroutine(popup.GetComponent<LabPopup>().Pop());
        }
    }


    public void DoTextPopup(Unit target, string text, Color color)
    {
        var popup = Instantiate(statPopUp, new Vector3(target.GetComponent<SpriteRenderer>().bounds.center.x, target.GetComponent<SpriteRenderer>().bounds.max.y, target.transform.position.z), Quaternion.identity);
        var popupText = popup.GetComponentInChildren<TextMeshProUGUI>();
        popupText.outlineWidth = 0.1f;
        popupText.outlineColor = Color.black;
        popupText.color = color;
        popupText.text = text;
        popupText.fontSize = 1.5f;
        var labPopUp = popup.GetComponent<LabPopup>();
        StartCoroutine(labPopUp.Rise());
        StartCoroutine(labPopUp.DestroyPopUp(0.6f));
    }

    public void SetTempEffect(Unit unit, string Icon, bool DoFancyStatChanges, float duration = 0, float storedValue = 0, float numberofStacks = 0)
    {

        var icon = Instantiate(Director.Instance.iconDatabase.Where(obj => obj.name == Icon).SingleOrDefault(), unit.namePlate.IconGrid.transform);
        var i = icon.GetComponent<EffectIcon>();
        if (unit.statusEffects.Contains(unit.statusEffects.Where(obj => obj.iconName == i.iconName).SingleOrDefault()))
        {
            i.DoFancyStatChanges = false;
            i.DestoryEffectIcon();
        }
        unit.statusEffects.Add(i);
        if (unit == null)
            Debug.LogError("OWNER SETUP BROKEN");
        else
            i.owner = unit;
        i.Initalize(unit, DoFancyStatChanges, duration, storedValue, numberofStacks);
    }

    public IEnumerator ChangeStat(Stat statToRaise, float AmountToRaise, bool multiplicative, Unit target, LabPopup popup)
    {
        var number = popup.GetComponentInChildren<TextMeshProUGUI>();
        number.outlineColor = Color.black;
        number.outlineWidth = 0.2f;
        number.color = Color.white;
        switch (statToRaise)
        {

            case Stat.ATK:
                if (!multiplicative)
                {
                    target.attackStat += (int)Math.Ceiling(AmountToRaise);
                }
                else
                {
                    target.attackStat = (int)Math.Ceiling(target.attackStat * AmountToRaise);
                    if (AmountToRaise > 1)
                        AmountToRaise = target.attackStat / AmountToRaise;
                    else
                        AmountToRaise = target.attackStat * AmountToRaise;
                }
                number.SetText(AmountToRaise.ToString() + " <sprite name=\"ATK RED\">");
                number.outlineColor = Color.red;
                DoStatVFX(AmountToRaise, Color.red, target);
                break;
            case Stat.DEF:
                if (!multiplicative)
                {
                    target.defenseStat += (int)Math.Ceiling(AmountToRaise);
                }
                else
                {
                    target.defenseStat = (int)Math.Ceiling(target.defenseStat * AmountToRaise);
                    if (AmountToRaise > 1)
                        AmountToRaise = target.defenseStat / AmountToRaise;
                    else
                        AmountToRaise = -target.defenseStat;
                }
                number.SetText(AmountToRaise.ToString() + " <sprite name=\"DEF BLUE\">");
                number.outlineColor = Color.blue;
                DoStatVFX(AmountToRaise, Color.blue, target);
                break;
            case Stat.SPD:
                if (!multiplicative)
                {
                    target.speedStat += (int)Math.Ceiling(AmountToRaise);
                }
                else
                {
                    target.speedStat = (int)Math.Ceiling(target.speedStat * AmountToRaise);
                    if (AmountToRaise > 1)
                        AmountToRaise = target.speedStat / AmountToRaise;
                    else
                        AmountToRaise = target.speedStat * AmountToRaise;
                }
                number.SetText(AmountToRaise.ToString() + " <sprite name=\"SPD YLW\">");
                number.outlineColor = Color.yellow;
                DoStatVFX(AmountToRaise, Color.yellow, target);
                break;
            case Stat.HP:
                if (!multiplicative)
                {
                    if (target.currentHP + (int)Math.Ceiling(AmountToRaise) > target.maxHP)
                    {
                        target.currentHP = target.maxHP;
                    }
                    else
                        target.currentHP += (int)Math.Ceiling(AmountToRaise);
                }
                else
                {
                    target.currentHP = (int)Math.Ceiling(target.currentHP * AmountToRaise);
                    if (AmountToRaise > 1)
                        AmountToRaise = target.currentHP / AmountToRaise;
                    else
                        AmountToRaise = target.currentHP * AmountToRaise;
                }
                number.SetText(AmountToRaise.ToString());
                number.outlineColor = Color.green;
                DoStatVFX(AmountToRaise, Color.green, target);
                break;

        }
        if (target.spotLight.intensity == 0)
        {
            var Light = target.spotLight;
            Light.color = number.outlineColor;
            target.ChangeUnitsLight(Light, 150, 15, 0.04f, 0.1f);
        }
        print("stats should be popping up");
        yield return new WaitForSeconds(1f);
        StartCoroutine(popup.DestroyPopUp());
        yield break;

    }

    public void DoStatVFX(float AmountToRaise, Color color, Unit target)
    {
        if (AmountToRaise > 0)
        {
            StartCoroutine(Tools.PlayVFX(target.gameObject, "StatUpVFX", color, color,new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y, 0), 1f, 0, false, true));
        }
        else
        { 
            StartCoroutine(Tools.PlayVFX(target.gameObject, "StatDownVFX", color, color ,new Vector3(0, 15, 0), 1f, 0, false, true));
        }
    }

  
    public static void SetUIOff(Unit unit)
    {
        int i = 0;
        if (unit.skillUIs != null && unit.IsPlayerControlled)
        {
            if(unit.state != PlayerState.WAITING)
             unit.state = PlayerState.IDLE;

            foreach (var skill in unit.skillUIs)
            {
                unit.skillUIs[i].SetActive(false);
                var actionContainer = unit.skillUIs[i].GetComponent<ActionContainer>();
                actionContainer.targetting = false;
                i++;
            }
            foreach (var z in Tools.GetAllUnits())
            {
                z.IsHighlighted = false;
                z.isDarkened = false;
            }
        }

    }



    public static void SetUIOn(Unit unit)
    {
        int i = 0;
        foreach (var x in Tools.GetAllUnits())
        {
            BattleSystem.SetUIOff(x);
        }
        if(unit.stamina.slider.value == unit.stamina.slider.maxValue)
        {
            foreach (var action in unit.actionList)
            {
                unit.state = PlayerState.DECIDING;
                LabCamera.Instance.MoveToUnit(unit);
                BattleLog.Instance.DisplayCharacterStats(unit, true);
                BattleLog.Instance.inventoryDisplay.gameObject.SetActive(false);
                unit.skillUIs[i].SetActive(true);
                var assignedAction = unit.skillUIs[i].GetComponent<ActionContainer>();
                assignedAction.targetting = false;
                if (!assignedAction.Disabled)
                {
                    if (assignedAction.limited)
                    {
                        if (assignedAction.numberofUses > 0)
                        {
                            assignedAction.button.interactable = true;
                        }
                        else
                        {
                            assignedAction.button.interactable = false;
                        }
                    }
                    else
                        assignedAction.button.interactable = true;
                } 
                assignedAction.button.enabled = true;
                assignedAction.action = action;
                assignedAction.damageNums.text = "<sprite name=\"ATK\">" + (action.damage + unit.attackStat).ToString();
                assignedAction.durationNums.text = "<sprite name=\"Duration\">" + (action.duration).ToString();
                assignedAction.costNums.text = action.cost * unit.actionCostMultiplier < 100 ? $"{action.cost * unit.actionCostMultiplier}%" : $"100%";
                assignedAction.costNums.color = Color.yellow;
                assignedAction.textMesh.text = action.ActionName;
                if (assignedAction.action.actionType == Action.ActionType.STATUS)
                {
                    assignedAction.damageParent.SetActive(false);
                }
                else
                    assignedAction.damageParent.SetActive(true);
                if (assignedAction.action.duration > 0 && assignedAction.action.actionType == Action.ActionType.STATUS)
                {
                    assignedAction.durationParent.SetActive(true);
                }
                else
                    assignedAction.durationParent.SetActive(false);
                i++;
            }
            unit.ActionLayout.gameObject.SetActive(true);
        }
    }

    public void AddAction(Action action)
    {
        BattleLog.Instance.ClearAllBattleLogText();
        if (action.unit != null)
        {
            var newAction = UnityEngine.Object.Instantiate(action);
            ActionsToPerform.Add(newAction);
            if (actionCo != null)
            {
                StopCoroutine(actionCo);
            }
            actionCo = PerformAction(newAction, newAction.unit);
            StartCoroutine(actionCo);
        }
    }

    IEnumerator battleCo;
    IEnumerator actionCo;




    public IEnumerator PerformAction(Action newaction, Unit unit)
    {
        LabCamera.Instance.ResetPosition();
        Tools.PauseAllStaminaTimers();
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        Director.Instance.timelinespeedDelay = 0.1f;
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            child.Return();
        }
        BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
        foreach (var x in Tools.GetAllUnits())
        {
            x.ExitDecision();
            if (x.intentUI != null)
            {
                StartCoroutine(FadeOutEnemyIntents(x));
            }
            if (x.skillUIs != null)
            {
                BattleSystem.SetUIOff(x);
                x.IsHighlighted = false;
            }
        }
        state = BattleStates.BATTLE;

        yield return new WaitForSeconds(1f);
        print("Action should be performed");
        foreach (var action in ActionsToPerform)
        {
            if (action.unit != null && action.targets != null)
            {
                if (action.unit.IsPlayerControlled)
                {
                    action.unit.state = PlayerState.WAITING;
                }
                else
                {
                    action.unit.state = PlayerState.IDLE;
                }
                action.OnActivated();
                if(action.limited)
                {
                    foreach (var act in action.unit.skillUIs)
                    {
                        if (action.ActionName == act.GetComponent<ActionContainer>().action.ActionName)
                        {
                            act.GetComponent<ActionContainer>().numberofUses--;
                        }
                    }
                }
                yield return new WaitUntil(() => action.Done);
                yield return new WaitForSeconds(0.7f);
            }
        }
        foreach (var x in Tools.GetAllUnits())
        {
            x.DoActionEnded();
        }
        yield return new WaitForSeconds(0.5f);
        ActionsToPerform = new List<Action>();
        BattleLog.Instance.ResetBattleLog();
        foreach (var line in GameObject.FindObjectsOfType<LineRenderer>())
        {
            if (line != null)
                line.enabled = true;
                line.gameObject.SetActive(true);
        }
        if (!unit.IsPlayerControlled)
        {
            StartCoroutine(unit.behavior.DoBehavior(unit));
        }
        foreach (var x in Tools.GetAllUnits())
        {

            if (x.intentUI != null)
            {
                yield return new WaitForSeconds(0.1f);
                x.intentUI.gameObject.SetActive(true);
                if (x.intentUI.action == null)
                    StartCoroutine(x.behavior.DoBehavior(x));
                Tools.SetImageColorAlphaToZero(x.intentUI.GetComponent<Image>());
                x.intentUI.GetComponent<UnityEngine.UI.Button>().interactable = true;
                StartCoroutine(Tools.FadeObject(x.intentUI.GetComponent<Image>(), 0.005f, true, false));
                StartCoroutine(Tools.FadeText(x.intentUI.textMesh, 0.005f, true, false));
                StartCoroutine(Tools.FadeText(x.intentUI.damageNums, 0.005f, true, false));
                StartCoroutine(Tools.FadeText(x.intentUI.costNums, 0.005f, true, false));
            }
            x.DoBattlePhaseEnd();
        }
        yield return new WaitUntil(() => !BattlePhasePause);
        if (enemyUnits.Count != 0 && playerUnits.Count != 0)
        {
            foreach (var x in playerUnits)
            {
                if (x.state == PlayerState.IDLE)
                {
                    state = BattleStates.DECISION_PHASE;
                    x.StartDecision();
                    break;
                }

            }
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);

        }     
        Director.Instance.timelinespeedDelay = OptionsManager.Instance.UserTimelineSpeedDelay;
        LabCamera.Instance.state = LabCamera.CameraState.SWAY;
        unit.DoBattlePhaseClose();
        if (state != BattleStates.DECISION_PHASE && state != BattleStates.WON && state != BattleStates.DEAD)
        {
            state = BattleStates.IDLE;
            Tools.UnpauseAllStaminaTimers();
        }
        yield break;
    }

    private IEnumerator FadeOutEnemyIntents(Unit unit)
    {
        unit.intentUI.GetComponent<UnityEngine.UI.Button>().interactable = false;
        StartCoroutine(Tools.FadeObject(unit.intentUI.GetComponent<Image>(), 0.005f, false, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.textMesh, 0.005f, false, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.damageNums, 0.005f, false, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.costNums, 0.005f, false, false));
        yield return new WaitForSeconds(1f);
        unit.intentUI.gameObject.SetActive(false);
    }

    public void SetupHUD(Unit unit, Transform position)
    {
        foreach (var x in Tools.GetAllUnits())
        {
            if (x.health == null || x.stamina == null)
            {
                var battlebar = Instantiate(Director.Instance.battlebar);
                var TL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
                Director.Instance.timeline.children.Add(TL);
                TL.portrait.sprite = x.charPortraits[0];
                TL.unit = x;
                x.timelinechild = TL;
                TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
                if (x.IsPlayerControlled)
                {
                    battlebar.transform.SetParent(Director.Instance.PlayerBattleBarGrid.transform);
                    battlebar.transform.localScale = new Vector3(0, 0, 0);
                    TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 85);
                    TL.playerPoint.SetActive(true);
                }

                else
                {
                    TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -60);
                    battlebar.transform.localScale = new Vector3(0, 0, 0);
                    TL.EnemyPoint.SetActive(true);
                }
                battlebar.unit = x;
                battlebar.portrait.sprite = x.charPortraits.Find(obj => obj.name == "neutral");
                battlebar.nameText.text = Tools.CheckNames(x);
                x.transform.rotation = LabCamera.Instance.camTransform.rotation;
                var stamina = battlebar.stamina;
                stamina.unit = x;
                x.stamina = stamina;
                x.stamina.Paused = true;
                var HP = Instantiate(hp, canvasParent.transform);
                x.health = HP;
                HP.unit = x;
                HP.transform.SetPositionAndRotation(new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x + unit.offset.x, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z / canvas.scaleFactor), LabCamera.Instance.transform.rotation);
                HP.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                var NP = Instantiate(namePlate, canvasParent.transform);
                x.namePlate = NP;
                NP.unit = x;
                NP.nameText.text = "";
                NP.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x + unit.offset.x, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z) / canvas.scaleFactor;
                x.GetComponent<SpriteRenderer>().flipX = false;

            }
            if (!x.IsPlayerControlled && x.intentUI == null)
            {
                var intentContainer = Instantiate(intent, canvasParent.transform);
                intentContainer.transform.localScale = new Vector3(0.025f, 0.03f, -25f);
                intentContainer.transform.SetPositionAndRotation(new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x + x.enemyintentOffset.x, x.GetComponent<SpriteRenderer>().bounds.max.y + x.enemyintentOffset.y, x.transform.position.z + x.enemyintentOffset.z) / canvas.scaleFactor, LabCamera.Instance.transform.rotation);
                x.intentUI = intentContainer;
                intentContainer.unit = x;
                x.namePlate.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x - 1.8f, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z) / canvas.scaleFactor;
                x.namePlate.IconGrid.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x, x.namePlate.IconGrid.transform.position.y, x.transform.position.z) / canvas.scaleFactor;
                x.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        if (unit.IsPlayerControlled && !unit.IsSummon)
        {
            unit.stamina.slider.value = unit.stamina.slider.maxValue;
            BattleLog.Instance.CreateActionLayout(unit);
        }

    }



}
