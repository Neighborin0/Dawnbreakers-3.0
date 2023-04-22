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
    public BattleLog BL;
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
    public LineRenderer targetLine;
    public bool HasStarted = false;

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
    [SerializeField]
    private List<Sprite> statSprites;

    public Vector3 cameraPos1Units;
    public Vector3 cameraPos2Units;
    public Vector3 cameraPos3Units;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
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
        BL = Director.Instance.BL;
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
            ResetBattleLog();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (var unit in Tools.GetAllUnits())
            {
                if (unit.stamina.slider.value == unit.stamina.slider.maxValue && unit.IsPlayerControlled)
                {
                    StartCoroutine(AdvanceASecond());
                    break;
                }
            }
        }
    }

    public BattleStates state;
    IEnumerator AdvanceASecond()
    {
        Tools.UnpauseAllStaminaTimers();
        state = BattleStates.WAITING;
        yield return new WaitForSeconds(1f);
        Tools.PauseAllStaminaTimers();
        state = BattleStates.DECISION_PHASE;
    }

    void StartBattle()
    {
        statStorers = new List<StatStorer>();
        Director.Instance.timeline.gameObject.SetActive(true);
        for (int i = 0; i <= playerUnits.Count - 1; i++)
        {
            playerUnits[i].gameObject.SetActive(true);
            playerUnits[i].transform.localScale = new Vector3(9f, 9f, 9f);
            playerUnits[i].transform.position = playerPositions[i].position;
            playerUnits[i].transform.SetParent(playerPositions[i].transform);
            var BSP = playerPositions[i].GetComponent<BattleSpawnPoint>();
            BSP.unit = playerUnits[i];
            BSP.Occupied = true;
            SetupHUD(playerUnits[i], playerPositions[i]);
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
            SetupHUD(enemiesToLoad[i], enemyPositions[i]);
            var BSP = enemyPositions[i].GetComponent<BattleSpawnPoint>();
            BSP.unit = enemiesToLoad[i];
            BSP.Occupied = true;
            enemyUnits.Add(enemy);
            numOfUnits.Add(enemy);
        }
        StartCoroutine(Transition());
        print(state);
    }

    IEnumerator Transition()
    {
        StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, false));
        yield return new WaitUntil(() => Director.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        LabCamera.Instance.ReadjustCam();
        yield return new WaitForSeconds(3f);
        Director.Instance.BL.Move(true);
        Director.Instance.timeline.Move(true);
        BattleLog.SetRandomAmbientTextActive();
        BL.CreateRandomAmbientText();
        BL.gameObject.SetActive(true);
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
        playerUnits[0].StartDecision();
        LabCamera.Instance.MovingTimeDivider = 1;
    }

    public void ResetBattleLog()
    {
        BattleLog.DisableCharacterStats();
        BattleLog.SetRandomAmbientTextActive();
        BattleLog.ClearBattleText();
        foreach (var z in Tools.GetAllUnits())
        {
            z.IsHighlighted = false;
            z.isDarkened = false;

        }

    }
    public IEnumerator TransitionToMap(bool LevelUpScreen = true)
    {
        yield return new WaitForSeconds(1f);
        foreach (Transform child in Director.Instance.timeline.transform)
        {

            Destroy(child.gameObject);
        }
        Director.Instance.timeline.gameObject.SetActive(false);
        Director.Instance.party.Clear();
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
                DontDestroyOnLoad(unit);
                //unit.gameObject.SetActive(false);
            }
        }
        if (LevelUpScreen)
        {
            Director.Instance.DisplayCharacterTab();
        }
        else
        {
            Director.Instance.StartCoroutine(Director.Instance.DoLoad("MAP2"));
            yield return new WaitUntil(() => Director.Instance.blackScreen.color == new Color(0, 0, 0, 1));
            foreach (var unit in Tools.GetAllUnits())
            {
                unit.StaminaHighlightIsDisabled = true;
                unit.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator lineCoroutine;
    public void DisplayEnemyIntent(Action action, Unit unit)
    {
        unit.intentUI.textMesh.text = action.ActionName;
        unit.intentUI.damageNums.text = (action.damage + unit.attackStat - action.targets.defenseStat).ToString();
        unit.intentUI.action = action;
        unit.intentUI.costNums.text = action.cost * unit.actionCostMultiplier < 100 ? $"{action.cost * unit.actionCostMultiplier}%" : $"100%";
        if (action.targets != unit)
        {
            var lineInstance = Instantiate(targetLine, unit.transform);
            lineInstance.SetPosition(0, new Vector3(unit.transform.position.x, unit.transform.position.y, unit.transform.position.z - 0.1f));
            lineInstance.SetPosition(1, unit.transform.position);
            lineCoroutine = Tools.SmoothMoveLine(lineInstance, action.targets.transform.position, 0.01f);
            StartCoroutine(lineCoroutine);
            if (action.targetType == Action.TargetType.ALL_ENEMIES)
            {
                foreach (var x in Tools.DetermineEnemies(unit))
                {
                    var li = Instantiate(targetLine);
                    li.SetPosition(0, unit.transform.position);
                    li.SetPosition(1, new Vector3(x.transform.position.x, x.transform.position.y, unit.transform.position.z));
                }
            }
        }
        if (unit.intentUI.action.actionType == Action.ActionType.STATUS)
        {
            unit.intentUI.damageParent.SetActive(false);
        }
        else
        {
            unit.intentUI.damageParent.SetActive(true);
        }
        unit.intentUI.gameObject.SetActive(true);
    }
    public void SetStatChanges(Stat statToRaise, float AmountToRaise, bool multiplicative, Unit target)
    {
        if (BattleSystem.Instance != null)
        {
            var battleSystem = BattleSystem.Instance;
            var popup = Instantiate(battleSystem.statPopUp, new Vector3(target.GetComponent<SpriteRenderer>().bounds.center.x - 1.5f, target.GetComponent<SpriteRenderer>().bounds.max.y + 4, target.transform.position.z), Quaternion.identity);
            battleSystem.StartCoroutine(battleSystem.ChangeStat(statToRaise, AmountToRaise, multiplicative, target, popup));
            if (AmountToRaise < 1)
            {
                battleSystem.StartCoroutine(Tools.SmoothMove(popup, 0.01f, 60, 0, -0.005f));
            }
            else
            {
                battleSystem.StartCoroutine(Tools.SmoothMove(popup, 0.01f, 60, 0, 0.005f));
            }
        }
    }


    public void DoTextPopup(Unit target, string text, Color color)
    {
        var popup = Instantiate(statPopUp, new Vector3(target.GetComponent<SpriteRenderer>().bounds.center.x - 1.5f, target.GetComponent<SpriteRenderer>().bounds.max.y + 4, target.transform.position.z), Quaternion.identity);
        var popupText = popup.GetComponentInChildren<TextMeshProUGUI>();
        popupText.outlineWidth = 0.1f;
        popupText.outlineColor = Color.black;
        popupText.color = color;
        popupText.text = text;
        var img = popup.GetComponentInChildren<Image>();
        img.gameObject.SetActive(false);
        StartCoroutine(Tools.SmoothMove(popup, 0.01f, 60, 0, 0.005f, 0, true));
    }

    public IEnumerator SetTempEffect(Unit unit, string Icon, Action action)
    {
        var icon = Instantiate(Director.Instance.iconDatabase.Where(obj => obj.name == Icon).SingleOrDefault(), unit.namePlate.IconGrid.transform);
        if (action != null)
        {
            var newAction = Instantiate(action);
            var i = icon.GetComponent<EffectIcon>();
            i.timerText.text = newAction.duration.ToString();
            var manIHateUnityScalingSometimesAndIDontWantToBeFuckedWithThisSoHaveThisLongAssVariable = i.timerText.GetComponent<RectTransform>();
            i.action = newAction;
            manIHateUnityScalingSometimesAndIDontWantToBeFuckedWithThisSoHaveThisLongAssVariable.sizeDelta = new Vector2(70.24f, 21.96f);
            i.owner = unit;
            print(newAction.duration);
            var timer = newAction.duration;
            i.timerText.text = newAction.duration.ToString();
            yield return new WaitUntil(() => !i.isPaused);
            while (timer > 0 && !i.ForceEnd)
            {
                yield return new WaitUntil(() => !i.isPaused);
                timer--;
                i.timerText.text = timer.ToString();
                yield return new WaitForSeconds(1f);
            }
            Destroy(icon);
        }

        yield break;


    }

    public IEnumerator ChangeStat(Stat statToRaise, float AmountToRaise, bool multiplicative, Unit target, GameObject popup)
    {
        Vector3 origShapePos = Vector3.zero;
        var number = popup.GetComponentInChildren<TextMeshProUGUI>();
        number.outlineColor = Color.black;
        number.outlineWidth = 0.2f;
        number.color = Color.white;
        var img = popup.GetComponentInChildren<Image>();
        var particleSystem = target.GetComponent<ParticleSystem>();
        particleSystem.GetComponent<ParticleSystemRenderer>().material = Instantiate(particleSystem.GetComponent<ParticleSystemRenderer>().material);
        var particleMaterial = particleSystem.GetComponent<ParticleSystemRenderer>().material;
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
                number.SetText(AmountToRaise.ToString());
                img.sprite = statSprites[0];
                particleMaterial.SetColor("_EmissionColor", Color.red * 10);
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
                number.SetText(AmountToRaise.ToString());
                img.sprite = statSprites[1];
                particleMaterial.SetColor("_EmissionColor", Color.blue * 10);
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
                number.SetText(AmountToRaise.ToString());
                particleMaterial.SetColor("_EmissionColor", Color.yellow * 10);
                img.sprite = statSprites[2];
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
                number.color = Color.green;
                img.gameObject.SetActive(false);
                particleMaterial.SetColor("_EmissionColor", Color.green * 10);
                break;

        }
        if (AmountToRaise < 0)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            particleSystem.gravityModifier = 5f;
            var sh = particleSystem.shape;
            origShapePos = sh.position;
            sh.position = new Vector3(sh.position.x, 1.4f, sh.position.z);
#pragma warning restore CS0618 // Type or member is obsolete
        }
        particleSystem.Play();
        print("stats should be popping up");
        yield return new WaitForSeconds(1f);
        Destroy(popup);
        particleSystem.Stop();
        yield return new WaitForSeconds(0.5f);
        if (AmountToRaise < 0)
        {
            particleSystem.gravityModifier = -5f;
            var sh = particleSystem.shape;
            sh.position = origShapePos;
        }
        yield break;

    }

    public static void SetUIOff(Unit unit)
    {
        int i = 0;
        if (unit.skillUIs != null && unit.state != PlayerState.DECIDING)
        {
            foreach (var skill in unit.skillUIs)
            {


                unit.skillUIs[i].SetActive(false);
                var actionContainer = unit.skillUIs[i].GetComponent<ActionContainer>();
                actionContainer.targetting = false;
                i++;
            }
        }
    }



    public static void SetUIOn(Unit unit)
    {
        int i = 0;
        //BattleSystem.SetUIOff();
        foreach (var x in Tools.GetAllUnits())
        {
            BattleSystem.SetUIOff(x);
        }
        foreach (var action in unit.actionList)
        {
            unit.state = PlayerState.DECIDING;
            LabCamera.Instance.MoveToUnit(unit);
            unit.skillUIs[i].SetActive(true);
            var assignedAction = unit.skillUIs[i].GetComponent<ActionContainer>();
            assignedAction.targetting = false;
            assignedAction.button.interactable = true;
            assignedAction.button.enabled = true;
            assignedAction.action = action;
            assignedAction.damageNums.text = (action.damage + unit.attackStat).ToString();
            assignedAction.durationNums.text = (action.duration).ToString();
            assignedAction.costNums.text = action.cost * unit.actionCostMultiplier < 100 ? $"{action.cost * unit.actionCostMultiplier}%" : $"100%";
            assignedAction.costNums.color = Color.yellow;
            assignedAction.textMesh.text = action.ActionName;
            if (assignedAction.action.actionType == Action.ActionType.STATUS)
            {
                assignedAction.damageParent.SetActive(false);
            }
            else
                assignedAction.damageParent.SetActive(true);
            if (assignedAction.action.duration > 0)
            {
                assignedAction.durationParent.SetActive(true);
            }
            else
                assignedAction.durationParent.SetActive(false);
            i++;
        }
    }

    public void AddAction(Action action)
    {
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
        foreach (var line in GameObject.FindObjectsOfType<LineRenderer>())
        {
            if (line != null)
            {
                StopCoroutine(lineCoroutine);
                line.enabled = false;
            }
        }
        Director.Instance.BL.Move(false);
        foreach (var x in Tools.GetAllUnits())
        {
            x.ExitDecision();
            if (x.intentUI != null)
            {
                x.intentUI.gameObject.SetActive(false);
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
                if (action.unit.GetComponentInChildren<LineRenderer>() != null)
                {
                    Destroy(action.unit.GetComponentInChildren<LineRenderer>().gameObject);
                }
                if(action.unit.IsPlayerControlled)
                {
                    action.unit.state = PlayerState.WAITING;
                }
              else
                {
                    action.unit.state = PlayerState.IDLE;
                }
                action.OnActivated();
                yield return new WaitUntil(() => action.Done);
                yield return new WaitForSeconds(1f);
            }
        }
        yield return new WaitForSeconds(1f);
        foreach (var x in Tools.GetAllUnits())
        {
            x.DoActionEnded();
        }
        yield return new WaitForSeconds(0.5f);
        state = BattleStates.DECISION_PHASE;
        //ActionsToPerform.Clear();
        ActionsToPerform = new List<Action>();
        BattleLog.ClearAmbientText();
        BattleLog.DisableCharacterStats();
        BattleLog.ClearBattleText();
        BattleLog.SetRandomAmbientTextActive();
        BL.CreateRandomAmbientText();
        foreach (var line in GameObject.FindObjectsOfType<LineRenderer>())
        {
            if (line != null)
                line.enabled = true;
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
            }
            x.DoBattlePhaseEnd();
        }
        if (enemyUnits.Count != 0 && playerUnits.Count != 0)
        {
            foreach (var x in playerUnits)
            {
                if (x.state == PlayerState.IDLE)
                    Director.Instance.BL.Move(true);
            }

        }
        Tools.UnpauseAllStaminaTimers();
        LabCamera.Instance.state = LabCamera.CameraState.SWAY;
        unit.DoBattlePhaseClose();
        yield break;
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
                    TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);
                }

                else
                {
                    TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -50);
                    battlebar.transform.SetParent(Director.Instance.EnemyBattleBarGrid.transform);
                    battlebar.nameText.transform.localScale = new Vector3(-1, 1, 1);
                    battlebar.stamina.transform.localScale = new Vector3(-1, 1, 1);
                    battlebar.healthbar.transform.localScale = new Vector3(-1, 1, 1);
                    battlebar.transform.localScale = new Vector3(0, 0, 0);
                    TL.num.text = Tools.CheckNames(unit);
                }
                battlebar.unit = x;
                battlebar.portrait.sprite = x.charPortraits.Find(obj => obj.name == "neutral");
                battlebar.nameText.text = Tools.CheckNames(x);
                x.transform.rotation = LabCamera.Instance.camTransform.rotation;
                //battlebar.healthbar.unit = x;
                //x.health = battlebar.healthbar; 
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
                NP.nameText.outlineColor = Color.black;
                NP.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x + unit.offset.x, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z) / canvas.scaleFactor;



            }
            if (!x.IsPlayerControlled && x.intentUI == null)
            {
                var intentContainer = Instantiate(intent, canvasParent.transform);
                intentContainer.transform.localScale = new Vector3(0.025f, 0.03f, -25f);
                intentContainer.transform.SetPositionAndRotation(new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x, x.GetComponent<SpriteRenderer>().bounds.max.y + 3f, x.transform.position.z) / canvas.scaleFactor, LabCamera.Instance.transform.rotation);
                x.intentUI = intentContainer;
                intentContainer.unit = x;
                x.namePlate.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x - 1.8f, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z) / canvas.scaleFactor;
                x.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        int i = 0;
        if (unit.IsPlayerControlled && !unit.IsSummon)
        {
            float scaleX = 1f;
            float scaleY = 1f;
            unit.stamina.slider.value = unit.stamina.slider.maxValue;
            var layout = Instantiate(ActionLayout, canvasParent.transform);


            if (unit.GetComponentInParent<BattleSpawnPoint>() == playerPositions[0].GetComponent<BattleSpawnPoint>())
            {
                layout.transform.position = new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x + 8f, unit.transform.position.y + 2.5f, -25f) / canvas.scaleFactor;
            }
            else if (unit.GetComponentInParent<BattleSpawnPoint>() == playerPositions[1].GetComponent<BattleSpawnPoint>())
            {
                layout.transform.position = new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x + 9.4f, unit.transform.position.y + 4.3f, -25f) / canvas.scaleFactor;
            }
            else
            {
                layout.transform.position = new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x + 7.3f, unit.transform.position.y, -25f) / canvas.scaleFactor;
            }

            foreach (var x in unit.actionList)
            {
                var container = Instantiate(genericActionContainer, layout.transform) as ActionContainer;
                container.action = unit.actionList[i];
                unit.skillUIs[i] = container.gameObject;
                container.baseUnit = unit;
                container.transform.localScale = new Vector3(scaleX, scaleY, -25f);
                i++;
            }

        }
     
    }



}
