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
    public List<LabLight> lablights;
    public float mainLightValue;
    public bool BossNode = false;
    public float DefaultVignetteIntensity;

    //Tutorial Stuff
    public bool TutorialNode = false;
    public List<TextMeshProUGUI> TutorialText;
    //public Image TutorialButton;
    public GameObject TutorialParent;

    public Vector3 cameraPos1Units;
    public Vector3 cameraPos2Units;
    public Vector3 cameraPos3Units;
    public Vector3 bossNodeCamPos;
    public Vector3 unitScale = new Vector3(11f, 11f, 11f);
    [NonSerialized]
    public bool DoPostBattleDialogue = true;

    public bool DustyIsDead = false;
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
        if (BattleSystem.Instance.effectsSetting.sharedProfile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.value = 0.28f;
            DefaultVignetteIntensity = vignette.intensity.value;
        }


    }
    void Start()
    {
        state = BattleStates.START;
        LabCamera.Instance.state = LabCamera.CameraState.SWAY;
        if (TutorialNode)
        {
            AudioManager.Instance.Stop(AudioManager.Instance.currentMusicTrack);
        }
        StartBattle();
    }
    void Update()
    {
        if (state != BattleStates.WON && state != BattleStates.DEAD)
        {
            if (playerUnits.Count == 0 && enemyUnits.Count != 0)
            {
                TransitionToState(BattleStates.DEAD, (isMapTransition) => TransitionToDeath());
            }
            else if (enemyUnits.Count == 0 && playerUnits.Count != 0)
            {
                TransitionToState(BattleStates.WON, (isMapTransition) => TransitionToMap(isMapTransition));
            }
        }

    }

    void TransitionToState(BattleStates nextState, Func<bool?, IEnumerator> transitionAction)
    {
        state = nextState;
        if (!StopUpdating)
        {
            CombatTools.EndAllTempEffectTimers();
            StopUpdating = true;
            if (battleCo != null)
            {
                StopCoroutine(battleCo);
            }

            battleCo = transitionAction(nextState == BattleStates.WON);
            StartCoroutine(battleCo);
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

                if (playerUnits[i].CustomScale != Vector3.zero)
                    playerUnits[i].transform.localScale = playerUnits[i].CustomScale;
                else
                    playerUnits[i].transform.localScale = unitScale;

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
                if (enemy.CustomScale != Vector3.zero)
                    enemy.transform.localScale = enemy.CustomScale;
                else
                    enemy.transform.localScale = unitScale;

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
        StopCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        if (TutorialNode)
        {
            LabCamera.Instance.state = LabCamera.CameraState.IDLE;
            TutorialParent.gameObject.SetActive(true);
            LabCamera.Instance.transform.position = new Vector3(0, 10000, -93);
            for (int i = 0; i < TutorialText.Count; i++)
            {

                StartCoroutine(Tools.FadeText(TutorialText[i], 0.01f, true, false));
                yield return new WaitForSeconds(3f);

            }
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < TutorialText.Count; i++)
            {
                if (i != TutorialText.Count - 1)
                {
                    StartCoroutine(Tools.FadeText(TutorialText[i], 0.01f, false, false));
                }
            }
            yield return new WaitForSeconds(1.7f);
            StartCoroutine(Tools.SmoothMoveUI(TutorialText[TutorialText.Count - 1].GetComponent<RectTransform>(), 0, 0, 0.025f));

            yield return new WaitForSeconds(1.8f);
            TutorialText[TutorialText.Count - 1].GetComponent <LabShaker>().Shake();
            yield return new WaitForSeconds(3.5f);
            TutorialText[TutorialText.Count - 1].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.01f);
            TutorialParent.gameObject.SetActive(false);
            AudioManager.Instance.Play("Coronus_Battle", 0.00000001f);
            yield return new WaitForSeconds(0.001f);
            StartCoroutine(AudioManager.Instance.Fade(0.35f, "Coronus_Battle", 0.1f, false));
            yield return new WaitForSeconds(1.5f);

            LabCamera.Instance.GetComponent<MoveableObject>().Move(false, 0.01f, 150);
            yield return new WaitUntil(() => LabCamera.Instance.transform.position.y <= BattleSystem.Instance.cameraPos1Units.y + 1);
            LabCamera.Instance.GetComponent<MoveableObject>().Stop();
        }
        else
        {
            for (int i = 0; i < TutorialText.Count; i++)
            {
                TutorialText[i].gameObject.SetActive(false);
            }
        }
        if (!TutorialNode)
            LabCamera.Instance.camTransform.position = Camera.main.transform.position;

        if(!TutorialNode)
            LabCamera.Instance.ReadjustCam(30f);
        else
            LabCamera.Instance.ReadjustCam();



        if (!TutorialNode)
            yield return new WaitForSeconds(2f);

        if (TutorialNode)
            yield return new WaitForSeconds(0.5f);
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
                Tools.ToggleUiBlocker(true, true, true);
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
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        AudioManager.QuickPlay("matriarch_laugh_001");
        yield return new WaitForSeconds(1f);
        if (OptionsManager.Instance.IntensityLevel == 0)
        {
            CombatTools.PauseStaminaTimer();
            MapController.Instance.gameObject.transform.SetParent(this.transform);
            OptionsManager.Instance.StartCoroutine(OptionsManager.Instance.DoLoad("Prologue Ending", null));
        }
        else
        {
            //RunTracker.Instance.DisplayStats();
            Tools.ToggleUiBlocker(false, false);
            CombatTools.PauseStaminaTimer();
        }
        yield break;
    }
    public IEnumerator TransitionToMap(bool? levelUpScreen = true)
    {
        state = BattleStates.WON;
        yield return new WaitForSeconds(1f);
        Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Director.Instance.canvas.worldCamera = LabCamera.Instance.uicam;
        Director.Instance.canvas.planeDistance = 20;
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
        if (levelUpScreen ?? true)
        {
            StartCoroutine(AudioManager.Instance.Fade(0.1f, AudioManager.Instance.currentMusicTrack, 2, false));
            StartCoroutine(EndBattle());
        }
        else
        {
            // Used in Dusty fight
            StartCoroutine(AudioManager.Instance.Fade(0f, AudioManager.Instance.currentMusicTrack, 0, false));
            OptionsManager.Instance.Load("MAP2", "Coronus_Map", 1, 0.5f);
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

    public IEnumerator EndBattle()
    {
        LabCamera.Instance.MoveToUnit(playerUnits[0], Vector3.zero, 0, 9f, -50, 0.8f);
        yield return new WaitForSeconds(0.4f);
        Director.Instance.DisplayCharacterTab(true);
        if (DoPostBattleDialogue)
            BattleLog.Instance.DoRandomLevelUpScreenDialogue();
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

    public void DisplayIntent(Action action, Unit unit)
    {

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
        unit.intentUI.portrait.sprite = action.targets.charPortraits[0];
        Tools.SetImageColorAlphaToZero(unit.intentUI.GetComponent<Image>());
        Tools.SetTextColorAlphaToZero(unit.intentUI.textMesh);
        Tools.SetTextColorAlphaToZero(unit.intentUI.damageNums);
        Tools.SetTextColorAlphaToZero(unit.intentUI.costNums);
        Tools.SetImageColorAlphaToZero(unit.intentUI.portraitParent);
        Tools.SetImageColorAlphaToZero(unit.intentUI.portrait);
        StartCoroutine(Tools.FadeObject(unit.intentUI.GetComponent<Image>(), 0.005f, true, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.textMesh, 0.005f, true, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.damageNums, 0.005f, true, false));
        StartCoroutine(Tools.FadeText(unit.intentUI.costNums, 0.005f, true, false));
        StartCoroutine(Tools.FadeObject(unit.intentUI.portraitParent, 0.005f, true, false));
        StartCoroutine(Tools.FadeObject(unit.intentUI.portrait, 0.005f, true, false));

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
        popupText.outlineWidth = 0.15f;
        popupText.outlineColor = Color.black;
        popupText.color = color;
        popupText.text = text;
        popupText.fontSize = 1.5f;
        var labPopUp = popup.GetComponent<LabPopup>();
        StartCoroutine(labPopUp.Rise(0.01f));
        StartCoroutine(labPopUp.DestroyPopUp(1.2f));
    }

    public void SetTempEffect(Unit unit, string Icon, bool DoFancyStatChanges, float duration = 0, float storedValue = 0, float numberofStacks = 0)
    {
        var icon = Instantiate(Director.Instance.iconDatabase.Where(obj => obj.name == Icon).SingleOrDefault(), unit.namePlate.IconGrid.transform);
        var i = icon.GetComponent<EffectIcon>();

        foreach (Transform x in unit.namePlate.IconGrid.transform)
        {
            var EI = x.gameObject.GetComponent<EffectIcon>();
            if (EI.iconName.Contains(i.iconName) && EI != i)
            {
                unit.statusEffects.Remove(EI);
                EI.DoFancyStatChanges = false;
                EI.DestoryEffectIcon();
                break;
            }
        }
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
                target.namePlate.UpdateArmor(target.armor);
                number.SetText(AmountToRaise.ToString() + " <sprite name=\"FORTIFY\">");
                Color armorParticleColor = new Color(0, 144, 255) * 0.2f;
                number.outlineColor = armorParticleColor;
                DoStatVFX(AmountToRaise, armorParticleColor, target);
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
            target.ChangeUnitsLight(Light, 150, 15, number.outlineColor, 0.04f, 0.1f);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(popup.DestroyPopUp());
        yield break;

    }

    public void DoStatVFX(float AmountToRaise, Color color, Unit target)
    {
        if (AmountToRaise > 0)
        {
           // Color colorAdder = new Color(10, 10, 10);
            float colorMult = 5;
            StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatUpVFX", color, color, new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y, 0), Quaternion.identity, 1.3f, 0, false, 1, 10, 0.0001f, "statUp_Loop_002"));
            StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatPulse", color * colorMult, color * colorMult, new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.min.y - 1.5f, 0), Quaternion.identity, 0.8f, 0, false, 1, 10, 0.0001f, "statUp_Loop_002"));
        }
        else
        {
            StartCoroutine(CombatTools.PlayVFX(target.gameObject, "StatDownVFX", color, color, new Vector3(0, 15, 0), Quaternion.identity, 1f, 0, false, 1, 10, 0.0001f, "statDown_Loop_001"));
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
                    if (actionContainer.action != null && actionContainer.action.actionStyle != Action.ActionStyle.STANDARD && actionContainer.targetting)
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
                if (skill.GetComponent<ActionContainer>() != null && skill.GetComponent<ActionContainer>().action != null)
                {
                    var actionContainer = skill.GetComponent<ActionContainer>();

                    actionContainer.action.ResetAction();
                    if (!unit.IsPlayerControlled)
                    {
                        foreach (var action in unit.actionList)
                        {
                            action.ResetAction();
                        }
                    }
                }
            }
        }
        Director.Instance.StartCoroutine(CombatTools.TurnOnDirectionalLight(10));
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

            }
            if (assignedAction.Disabled)
                assignedAction.button.interactable = false;

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

    IEnumerator battleCo;
    IEnumerator actionCo;

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



    public IEnumerator PerformAction()
    {
        Director.Instance.timeline.slider.value = 0;
        LabCamera.Instance.ResetPosition();
        CombatTools.PauseStaminaTimer();
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
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
            x.DoOnPreformActionStarted();
        }
        state = BattleStates.BATTLE;
        yield return new WaitForSeconds(1f);
        foreach (var x in Tools.GetAllUnits())
        {
            x.state = PlayerState.WAITING;
        }
        ActionsToPerform = ActionsToPerform.OrderBy(x => 100 - CombatTools.DetermineTrueCost(x))
         .ThenBy(x => x.unit.IsPlayerControlled)
        .ThenBy(x => x.actionType)
        .Reverse().ToList();
        CombatTools.UnpauseStaminaTimer();
        for (int i = 0; i < ActionsToPerform.Count; i++)
        {
            var action = ActionsToPerform[i];
            if (action.unit == null)
            {
                ActionsToPerform.Remove(action);
                continue;
            }

            CombatTools.UnpauseStaminaTimer();

            yield return new WaitUntil(() => (100 - Director.Instance.timeline.slider.value) <= (100 - CombatTools.DetermineTrueCost(action) * action.unit.actionCostMultiplier) || Director.Instance.timeline.slider.value == Director.Instance.timeline.slider.maxValue);
            CombatTools.PauseStaminaTimer();
            if (Director.Instance.timeline.slider.value < Director.Instance.timeline.slider.maxValue)
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

                Director.Instance.timeline.actionDisplayer.baseText.text = action.ActionName;
                Director.Instance.timeline.StartFadeAction(true);

                action.Done = false;
                action.unit.state = PlayerState.IDLE;
                action.OnActivated();

                foreach(var timelineChild in Director.Instance.timeline.children)
                {
                    if (timelineChild.unit == action.unit)
                    {
                        print(timelineChild.unit.unitName);
                        Director.Instance.timeline.ReplaceMainPortraitWithMiniPortrait(timelineChild);
                        timelineChild.CanClear = true;
                        timelineChild.GetComponent<LabUIInteractable>().CanHover = false;
                        timelineChild.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.2f);
                        timelineChild.portrait.color = new Color(1, 1, 1, 0.2f);
                       
                    }
                }
              

                if (action.limited)
                {
                    foreach (var skillUI in action.unit.skillUIs)
                    {
                        if (action.ActionName == skillUI.GetComponent<ActionContainer>().action.ActionName)
                        {
                            skillUI.GetComponent<ActionContainer>().numberofUses--;
                        }
                    }
                }

                yield return new WaitUntil(() => action.Done);
                yield return new WaitUntil(() => !BattlePhasePause);
                yield return new WaitForSeconds(0.2f);
              
                action.ResetAction();
                yield return new WaitForSeconds(0.01f);

                Director.Instance.timeline.StartFadeAction(false);
                foreach (var unit in Tools.GetAllUnits())
                {
                    unit.DoActionEnded();
                }

                if (CheckDeathState())
                {
                    BattlePhasePause = true;
                    CombatTools.PauseStaminaTimer();
                    StopCoroutine(actionCo);
                    yield break;
                }
                else
                {
                    yield return new WaitUntil(() => !BattlePhasePause);
                    yield return new WaitForSeconds(0.1f);
                }

                ActionsToPerform = ActionsToPerform
                    .OrderBy(x => 100 - CombatTools.DetermineTrueCost(x))
                    .ThenBy(x => x.unit.IsPlayerControlled)
                    .ThenBy(x => x.actionType)
                    .Reverse()
                    .ToList();
            }
        }
        yield return new WaitUntil(() => !BattlePhasePause);
        yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
        //All Actions Are Done
        yield return new WaitUntil(() => !CheckDeathState());
        actionCo = ForcePerformActionClose();
        Director.Instance.StartCoroutine(actionCo);
    }

    public IEnumerator ForcePerformActionClose()
    {
        yield return new WaitForSeconds(0.2f);
        CombatTools.UnpauseStaminaTimer();
        BattleSystem.Instance.ActionsToPerform = new List<Action>();
        StartCoroutine(Director.Instance.timeline.ResetTimeline());
        yield return new WaitUntil(() => Director.Instance.timeline.slider.value <= 0);
        foreach (var y in Tools.GetAllUnits())
        {

            foreach (var skill in y.skillUIs)
            {
                var actionContainer = skill.GetComponent<ActionContainer>();
                if (actionContainer.action != null)
                {
                    actionContainer.action.actionStyle = Action.ActionStyle.STANDARD;
                    actionContainer.lightButton.state = ActionTypeButton.ActionButtonState.LIGHT;
                    actionContainer.heavyButton.state = ActionTypeButton.ActionButtonState.HEAVY;
                }

            }
            if (!y.IsPlayerControlled)
            {
                y.behavior.DoBehavior(y);
                if (y.intentUI != null)
                {
                    yield return new WaitForSeconds(0.01f);
                    y.intentUI.gameObject.SetActive(true);
                    y.FadeIntent(false);
                }

            }

            y.DoBattlePhaseEnd();
        }
        //State just before player gets control
        BattleLog.Instance.ResetBattleLog();
        CombatTools.TickAllEffectIcons();
        yield return new WaitUntil(() => !BattlePhasePause);
        if (BattleSystem.Instance.enemyUnits.Count != 0 && BattleSystem.Instance.playerUnits.Count != 0)
        {
            BattleSystem.Instance.state = BattleStates.DECISION_PHASE;
            for (int i = 0; i < BattleSystem.Instance.playerUnits.Count; i++)
            {
                BattleSystem.Instance.playerUnits[i].state = PlayerState.IDLE;
                if (i == 0)
                {
                    yield return new WaitUntil(() => !BattlePhasePause);
                    BattleSystem.Instance.playerUnits[i].StartDecision();
                }
            }
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
        }
        foreach (var y in Tools.GetAllUnits())
        {
            if (!y.DoesntLoseArmorAtStartOfRound)
            {
                y.namePlate.UpdateArmor(y.armor, true);
            }

            else
                y.DoesntLoseArmorAtStartOfRound = false;


            y.DoBattlePhaseClose();

        }
        CombatTools.ReturnPipCounter().AddPip();

        if (BattleSystem.Instance.state != BattleStates.DECISION_PHASE && BattleSystem.Instance.state != BattleStates.WON && BattleSystem.Instance.state != BattleStates.DEAD && BattleSystem.Instance.state != BattleStates.TALKING && BattleSystem.Instance.enemyUnits.Count > 0 && BattleSystem.Instance.playerUnits.Count > 0)
        {
            BattleSystem.Instance.state = BattleStates.IDLE;
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
            if (x.intentUI == null)
            {
                var intentContainer = Instantiate(intent, canvasParent.transform);
                intentContainer.transform.localScale = new Vector3(0.025f, 0.03f, -25f);
                intentContainer.transform.SetPositionAndRotation(new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x + x.enemyintentOffset.x, x.GetComponent<SpriteRenderer>().bounds.max.y + x.enemyintentOffset.y, x.transform.position.z + x.enemyintentOffset.z) / canvas.scaleFactor, LabCamera.Instance.transform.rotation);
                x.intentUI = intentContainer;
                intentContainer.unit = x;
            }
            if (!x.IsPlayerControlled)
            {
                x.namePlate.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x - 1.8f, x.GetComponent<SpriteRenderer>().bounds.min.y - 1f, x.transform.position.z) / canvas.scaleFactor;
                x.namePlate.IconGrid.transform.position = new Vector3(x.GetComponent<SpriteRenderer>().bounds.center.x, x.namePlate.IconGrid.transform.position.y, x.transform.position.z) / canvas.scaleFactor;
                x.GetComponent<SpriteRenderer>().flipX = true;
                x.unitName = CombatTools.CheckNames(x);
            }
        }
        if (unit.IsPlayerControlled && !unit.IsSummon)
        {
            BattleLog.Instance.CreateActionLayout(unit);
        }

    }



}