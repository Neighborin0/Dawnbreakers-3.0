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

using System.Text.RegularExpressions;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using static System.Collections.Specialized.BitVector32;

public enum BattleStates { START, DECISION_PHASE, BATTLE, WON, DEAD, IDLE, TALKING }
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
    public GameObject statPopUp;
    public ActionContainer genericActionContainer;
    public GameObject canvasParent;


    public List<Transform> playerPositions;
    public List<Transform> enemyPositions;

    public Healthbar hp;
    public Stamina MainStaminaBar;
    public Stamina staminaBar;
    public NamePlate namePlate;
    public IntentContainer intent;
    public Canvas canvas;
    public GameObject dot;
    public bool HasStarted = false;
    public bool BattlePhasePause = false;
    public bool Paused = false;
    public TextMeshProUGUI pauseText;

    public List<Action> ActionsToPerform;
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
    public bool BossNode = false;
    private IEnumerator EnemyIntentFade;

    //Tutorial Stuff
    public bool TutorialNode = false;
    public TextMeshProUGUI TutorialText;
    //public Image TutorialButton;
    public GameObject TutorialParent;

    public Vector3 cameraPos1Units;
    public Vector3 cameraPos2Units;
    public Vector3 cameraPos3Units;
    public Vector3 bossNodeCamPos;
    [NonSerialized]
    public bool DoPostBattleDialogue = true;
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
                if (!StopUpdating)
                {
                    CombatTools.EndAllTempEffectTimers();
                    StopUpdating = true;
                    if (battleCo != null)
                        StopCoroutine(battleCo);

                    battleCo = TransitionToDeath();
                    StartCoroutine(battleCo);
                }

            }
            else if (enemyUnits.Count == 0 && playerUnits.Count != 0)
            {
                state = BattleStates.WON;
                if (!StopUpdating)
                {
                    CombatTools.EndAllTempEffectTimers();
                    StopUpdating = true;
                    if (battleCo != null)
                        StopCoroutine(battleCo);

                    battleCo = TransitionToMap();
                    StartCoroutine(battleCo);
                }
            }
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
                    //SPD = playerUnits[i].speedStat,
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
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogWarning("So this is where the error happening");
        }
    }

    IEnumerator Transition()
    {
        yield return new WaitForSeconds(1f);
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.statusEffects.Clear();
            SetupHUD(unit, unit.transform);
        }
        if (!Director.Instance.UnlockedPipSystem)
        {
            Director.Instance.timeline.pipCounter.gameObject.SetActive(false);
        }
        else
        {
            Director.Instance.timeline.pipCounter.gameObject.SetActive(true);
        }
        StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        if (TutorialNode)
        {
            IEnumerator fadeCoroutineText;
            LabCamera.Instance.state = LabCamera.CameraState.IDLE;
            TutorialParent.gameObject.SetActive(true);
            LabCamera.Instance.transform.position = new Vector3(0, 10000, -93);
            fadeCoroutineText = Tools.FadeText(TutorialText, 0.04f, true, false);
            StartCoroutine(fadeCoroutineText);
            yield return new WaitForSeconds(10f);
            StopCoroutine(fadeCoroutineText);
            StartCoroutine(Tools.FadeText(TutorialText, 0.01f, false, false));
            yield return new WaitForSeconds(2f);
            TutorialParent.gameObject.SetActive(false);
            LabCamera.Instance.GetComponent<MoveableObject>().Move(false, 0.01f, 100);
            yield return new WaitUntil(() => LabCamera.Instance.transform.position.y <= BattleSystem.Instance.cameraPos1Units.y + 0.01f);
            LabCamera.Instance.GetComponent<MoveableObject>().Stop();


        }
        LabCamera.Instance.ReadjustCam();

        if (!TutorialNode)
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
                unit.behavior.DoBehavior(unit);
            }
            if (TutorialNode)
            {
                if (unit.IsPlayerControlled)
                {
                    unit.currentHP = (int)(unit.currentHP * 0.5f);
                    unit.health.backSlider.value = (int)(unit.currentHP * 0.5f);
                }
            }
            unit.StaminaHighlightIsDisabled = false;
            unit.DoBattleStarted();
        }
        yield return new WaitForSeconds(0.5f);
        OptionsManager.Instance.blackScreen.gameObject.SetActive(false);
        if (state != BattleStates.TALKING)
        {
            if (TutorialNode || BossNode)
            {
                playerUnits[0].StartDecision(false);
            }
            else
            {
                playerUnits[0].StartDecision(true);
            }
        }

        LabCamera.Instance.MovingTimeDivider = 1;
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.DoPostBattleStarted();
        }
        OptionsManager.Instance.CanPause = true;
    }

    public IEnumerator TransitionToDeath()
    {
        LabCamera.Instance.ResetPosition();
        yield return new WaitForSeconds(1f);
        if (OptionsManager.Instance.IntensityLevel == 0)
        {
            CombatTools.PauseStaminaTimer();
            MapController.Instance.gameObject.transform.SetParent(this.transform);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
            OptionsManager.Instance.StartCoroutine(OptionsManager.Instance.DoLoad("Prologue Ending"));
        }
        else
        {
            //RunTracker.Instance.DisplayStats();
            Tools.ToggleUiBlocker(false, false);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
            CombatTools.PauseStaminaTimer();
        }
        yield break;
    }
    public IEnumerator TransitionToMap(bool LevelUpScreen = true)
    {
        yield return new WaitForSeconds(1f);
        foreach (Transform child in Director.Instance.timeline.transform)
        {
            if (child.GetComponent<TimeLineChild>() != null)
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
                unit.state = PlayerState.IDLE;
                foreach (var i in statStorers)
                {
                    if (i.unitName == unit.unitName)
                    {
                        unit.attackStat = i.ATK;
                        unit.maxHP = i.HP;
                        unit.defenseStat = i.DEF;
                        //unit.speedStat = i.SPD;
                    }
                }
                CombatTools.TurnOffCriticalUI(unit);
                Director.Instance.party.Add(unit);
                DontDestroyOnLoad(unit.gameObject);
            }
        }
        if (LevelUpScreen)
        {
            Director.Instance.DisplayCharacterTab(true);
            LabCamera.Instance.MoveToUnit(playerUnits[0], Vector3.zero, 0, 8, -40, 0.5f);
            if (DoPostBattleDialogue)
                BattleLog.Instance.DoRandomLevelUpScreenDialogue();
        }
        else
        {
            //Used in Dusty fight
            OptionsManager.Instance.Load("MAP2");
            yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
            foreach (var unit in Tools.GetAllUnits())
            {
                Director.Instance.timeline.RefreshTimeline();
                CombatTools.ReturnPipCounter().ResetPips();
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
        print(action.ActionName);
        if (action.targets == null)
            Debug.LogError("TARGETS ARE NULL????");
        else
            print(action.targets.unitName);
        unit.intentUI.textMesh.text = action.ActionName;
        if (CombatTools.DetermineTrueActionValue(action) != 0)
            unit.intentUI.damageNums.text = $"<sprite name=\"{action.damageType}\">" + ((int)((CombatTools.DetermineTrueActionValue(action) + unit.attackStat) * CombatTools.ReturnTypeMultiplier(action.targets, action.damageType))).ToString();
        unit.intentUI.action = action;
        unit.intentUI.costNums.text = CombatTools.DetermineTrueCost(action) * unit.actionCostMultiplier < 100 ? $"{CombatTools.DetermineTrueCost(action) * unit.actionCostMultiplier}%" : $"100%";
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
        StartCoroutine(labPopUp.DestroyPopUp(0.9f));
    }

    public void SetTempEffect(Unit unit, string Icon, bool DoFancyStatChanges, float duration = 0, float storedValue = 0, float numberofStacks = 0)
    {
        foreach (Transform x in unit.namePlate.IconGrid.transform)
        {
            var EI = x.gameObject.GetComponent<EffectIcon>();
            print(EI.iconName);
            print(Icon);
            if (EI.iconName == Icon)
            {
                unit.statusEffects.Remove(EI);
                EI.DoFancyStatChanges = false;
                EI.DestoryEffectIcon();
                break;
            }
        }
        var icon = Instantiate(Director.Instance.iconDatabase.Where(obj => obj.name == Icon).SingleOrDefault(), unit.namePlate.IconGrid.transform);
        var i = icon.GetComponent<EffectIcon>();
        i.duration = duration;
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
            case Stat.ARMOR:
                if (!multiplicative)
                {
                    target.armor += (int)Math.Ceiling(AmountToRaise);
                }
                else
                {
                    target.armor = (int)Math.Ceiling(target.armor * AmountToRaise);
                    if (AmountToRaise > 1)
                        AmountToRaise = target.armor / AmountToRaise;
                    else
                        AmountToRaise = -target.armor;
                }
                target.namePlate.UpdateArmor();
                number.SetText(AmountToRaise.ToString() + " <sprite name=\"DEF BLUE\">");
                number.outlineColor = Color.blue;
                DoStatVFX(AmountToRaise, Color.blue, target);
                break;
            /*case Stat.SPD:
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
            */
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
            target.ChangeUnitsLight(Light, 150, 15, number.outlineColor, 0.04f, 0.1f);
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
            StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatUpVFX", color, color, new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y, 0), Quaternion.identity, 1f, 0, false, 1));
        }
        else
        {
            StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatDownVFX", color, color, new Vector3(0, 15, 0), Quaternion.identity, 1f, 0, false, 1));
        }
    }


    public static void SetUIOff(Unit unit)
    {
        if (unit.skillUIs != null && unit.IsPlayerControlled)
        {
            if (unit.state == PlayerState.DECIDING)
            {
                unit.state = PlayerState.IDLE;               
                Director.Instance.timeline.RemoveTimelineChild(unit);
                
                foreach (var skill in unit.skillUIs)
                {
                    var actionContainer = skill.GetComponent<ActionContainer>();
                    if (actionContainer.action.actionStyle != Action.ActionStyle.STANDARD && actionContainer.targetting)
                    {
                        CombatTools.ReturnPipCounter().AddPip();
                        actionContainer.action.actionStyle = Action.ActionStyle.STANDARD;
                    }
                    actionContainer.SetActive(false);
                }
            }
                
            foreach (var skill in unit.skillUIs)
            {
                skill.SetActive(false);
                var actionContainer = skill.GetComponent<ActionContainer>();
              
                actionContainer.action.ResetAction();
                if(!unit.IsPlayerControlled)
                {
                    foreach (var action in unit.actionList)
                    {
                        action.ResetAction();
                    }
                }
            }
        }
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(0.05f));
    }



    public static void SetUIOn(Unit unit)
    {
        int i = 0;
        
        foreach (var x in Tools.GetAllUnits())
        {
            SetUIOff(x);
        }
        foreach (var action in unit.actionList)
        {
            unit.state = PlayerState.DECIDING;
            LabCamera.Instance.MoveToUnit(unit, Vector3.zero);
            BattleLog.Instance.DisplayCharacterStats(unit);
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
            var newAction = Instantiate(action);
            assignedAction.button.enabled = true;
            assignedAction.action = newAction;
            assignedAction.action.actionStyle = Action.ActionStyle.STANDARD;
            assignedAction.damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + unit.attackStat).ToString();
            assignedAction.durationNums.text = "<sprite name=\"Duration\">" + (newAction.duration).ToString();
            assignedAction.costNums.text = CombatTools.DetermineTrueCost(action) * unit.actionCostMultiplier < 100 ? $"{CombatTools.DetermineTrueCost(action) * unit.actionCostMultiplier}%" : $"100%";
            assignedAction.costNums.color = Color.yellow;
            assignedAction.textMesh.text = newAction.ActionName;

            assignedAction.SetActionStyleButtonsActive(false);           

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

    public void AddAction(Action action)
    {
        BattleLog.Instance.ClearAllBattleLogText();
        if (action.unit != null)
        {
            ActionsToPerform.Add(action);
            if (CombatTools.CheckIfAllUnitsAreReady())
            {
                actionCo = PerformAction();
                StartCoroutine(actionCo);
            }
        }
    }

    IEnumerator battleCo;
    IEnumerator actionCo;




    public IEnumerator PerformAction()
    {
        ActionsToPerform = ActionsToPerform.OrderBy(x => 100 - CombatTools.DetermineTrueCost(x)).ToList();
        ActionsToPerform = ActionsToPerform.OrderBy(x => x.unit.IsPlayerControlled).ToList();
        ActionsToPerform.Reverse();
        print(ActionsToPerform);
        Director.Instance.timeline.slider.value = 0;
        LabCamera.Instance.ResetPosition();
        CombatTools.PauseStaminaTimer();
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
                x.FadeIntent(true);
            }
            if (x.skillUIs != null)
            {
                SetUIOff(x);
                x.IsHighlighted = false;
            }
        }
        state = BattleStates.BATTLE;
        yield return new WaitForSeconds(1f);
        print("Action should be performed");
        foreach (var action in ActionsToPerform.ToList())
        {
         
            CombatTools.UnpauseStaminaTimer();
            if (ActionsToPerform.Count > 0)
            {
                yield return new WaitUntil(() => (100 - Director.Instance.timeline.slider.value) <= (100 - CombatTools.DetermineTrueCost(action) * action.unit.actionCostMultiplier) || Director.Instance.timeline.slider.value == 100);
                if (Director.Instance.timeline.slider.value < Director.Instance.timeline.slider.maxValue)
                {
                    CombatTools.PauseStaminaTimer();
                    if (action.unit != null)
                    {
                        if (action.targets == null)
                        {
                            switch (action.targetType)
                            {
                                case Action.TargetType.ENEMY:
                                    action.targets = CombatTools.GetRandomEnemy(action.unit);
                                    break;
                                case Action.TargetType.ALLY:
                                    action.targets = CombatTools.GetRandomAlly(action.unit);
                                    break;
                            }
                        }
                        action.unit.state = PlayerState.IDLE;
                        action.OnActivated();

                        var TL = Director.Instance.timeline.ReturnTimelineChild(action.unit);
                        TL.CanClear = true;
                        TL.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
                        TL.portrait.color = new Color(1, 1, 1, 0.5f);


                        if (action.limited)
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
                        yield return new WaitForSeconds(0.4f);
                        ActionsToPerform.Remove(action);
                        ActionsToPerform = ActionsToPerform.OrderBy(x => 100 - CombatTools.DetermineTrueCost(x)).ToList();
                        ActionsToPerform = ActionsToPerform.OrderBy(x => x.unit.IsPlayerControlled).ToList();
                        ActionsToPerform.Reverse();
                        action.ResetAction();
                        yield return new WaitForSeconds(1f);
                        foreach (var x in Tools.GetAllUnits())
                        {
                            x.DoActionEnded();
                        }
                        if (CheckDeathState())
                        {
                            StopCoroutine(actionCo);
                        }
                        else
                            yield return new WaitUntil(() => !BattlePhasePause);

                    }
                }
            }
            else
                break;
          
        }
        //All Actions Are Done
        CombatTools.UnpauseStaminaTimer();
        yield return new WaitForSeconds(0.5f);
        ActionsToPerform = new List<Action>();
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            Destroy(child.gameObject);
        }
        Director.Instance.timeline.children.Clear();
        yield return new WaitForSeconds(0.5f);
        foreach (var x in Tools.GetAllUnits())
        {
            foreach (var skill in x.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                actionContainer.action.actionStyle = Action.ActionStyle.STANDARD;
                actionContainer.lightButton.state = ActionTypeButton.ActionButtonState.LIGHT;
                actionContainer.heavyButton.state = ActionTypeButton.ActionButtonState.HEAVY;
            }
                if (!x.IsPlayerControlled)
            {
                x.behavior.DoBehavior(x);
                if (x.intentUI != null)
                {
                    yield return new WaitForSeconds(0.1f);
                    x.intentUI.gameObject.SetActive(true);
                    x.FadeIntent(false);
                }
                
            }
           
            x.DoBattlePhaseEnd();
        }
        StartCoroutine(Director.Instance.timeline.ResetTimeline());
        //State just before player gets control
        BattleLog.Instance.ResetBattleLog();
        if (enemyUnits.Count != 0 && playerUnits.Count != 0)
        {
            foreach (var x in playerUnits)
            {
                state = BattleStates.DECISION_PHASE;
                x.StartDecision();
                break;
            }
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
        }
        Director.Instance.timelinespeedDelay = OptionsManager.Instance.UserTimelineSpeedDelay;
        foreach (var x in Tools.GetAllUnits())
        {
            x.armor = 0;
            x.namePlate.UpdateArmor();
            x.DoBattlePhaseClose();

        }
        CombatTools.ReturnPipCounter().AddPip();
        CombatTools.TickAllEffectIcons();
        if (state != BattleStates.DECISION_PHASE && state != BattleStates.WON && state != BattleStates.DEAD && state != BattleStates.TALKING && enemyUnits.Count > 0 && playerUnits.Count > 0)
        {
            state = BattleStates.IDLE;
        }
        yield break;
    }

    private bool CheckDeathState()
    {
        bool StopBattle = false;
        if (playerUnits.Count == 0 && enemyUnits.Count != 0)
        {
            state = BattleStates.DEAD;
            StopBattle = true;
            if (!StopUpdating)
            {
                CombatTools.EndAllTempEffectTimers();
                StopUpdating = true;
                if (battleCo != null)
                    StopCoroutine(battleCo);

                battleCo = TransitionToDeath();
                StartCoroutine(battleCo);
            }

        }
        else if (enemyUnits.Count == 0 && playerUnits.Count != 0)
        {
            StopBattle = true;
            state = BattleStates.WON;
            if (!StopUpdating)
            {
                CombatTools.EndAllTempEffectTimers();
                StopUpdating = true;
                if (battleCo != null)
                    StopCoroutine(battleCo);

                battleCo = TransitionToMap();
                StartCoroutine(battleCo);
            }
        }
        return StopBattle;
    }
    
    public void SetupHUD(Unit unit, Transform position)
    {
        foreach (var x in Tools.GetAllUnits())
        {
            if (x.health == null)
            {
                x.transform.rotation = LabCamera.Instance.camTransform.rotation;
                var HP = Instantiate(hp, canvasParent.transform);
                x.health = HP;
                HP.unit = x;
                HP.transform.SetPositionAndRotation(new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x + unit.offset.x, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z / canvas.scaleFactor), LabCamera.Instance.transform.rotation);
                HP.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                var NP = Instantiate(namePlate, canvasParent.transform);
                x.namePlate = NP;
                NP.unit = x;
                NP.defText.text = "0";
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
            BattleLog.Instance.CreateActionLayout(unit);
        }

    }



}
