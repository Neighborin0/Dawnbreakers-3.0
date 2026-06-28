using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

using static UnityEngine.UI.CanvasScaler;
using UnityEngine.Events;

public class Director : MonoBehaviour
{
    public List<Unit> party;
    public List<Unit> currentUnitsInScene;

    //databases
    //public List<Unit> characterdatabase;
    public List<Unit> Unitdatabase;
    public List<Item> itemDatabase;
    public List<GameObject> VFXList;
    public List<Action> actionDatabase;
    public List<GameObject> iconDatabase;
    public static List<DialogueHandler> dialogues;
    public List<MapTemplate> mapTemplates;
    public List<EnemyEncounterData> enemyEncounterData;
    //

    public CharacterTab characterTab;
    public DirectorInfo directorInfo;
    public static int LabyrinthLVL;
    public GameObject characterSlotpos;
    public CharacterSlot characterSlot;
    public GameObject CloseIndicator;
    public GameObject TabGrid;
    public GridLayoutGroup PlayerBattleBarGrid;
    public GameObject ItemTabGrid;
    public GridLayoutGroup EnemyBattleBarGrid;
    public BattleBar battlebar;
    public Canvas canvas;
    //public FPSCounter fPSCounter;
    public bool CharacterSlotsDisplayed;
    IEnumerator generalCoruntine;
    bool buttonsAreDisabled = false;
    public bool DevMode;
    public Image blackScreen;
    public Button backButton;
    public GameObject LevelUpText;
    public TextMeshProUGUI chooseYourItemText;
    public GameObject EffectPopUp;
    public GameObject ConfirmButton;
    public GameObject CharacterSlotButtonprefab;
    public TextMeshProUGUI PipTutorialText;
    public Image BossCircle;
    public BossIntro bossIntro;
    public Image CutsceneUiBlocker;
    public bool InBattle = false;
    public ItemHandler itemHandler;
    public ActionRewardManager actionRewardManager;


    //Timeline stuff
    public TimeLine timeline;
    public float staminaSPDDivider;
    public float WeaknessMultiplier = 1.3f;
    public float ResistanceMultiplier = 0.3f;

    //Weakness and Resistance Modifiers
    public int TimelineReduction = 50;
    public int TimelineAddition = 10;

    //Action Modifiers
    public int TimelineReductionNonStandardAction = 20;
    public int TimelineAdditionNonStandardAction = 10;

    //public TextMeshProUGUI LevelDropSubText;

    //psuedo save flags
    public bool UnlockedPipSystem = false;

    public LabCamera.CameraState previousCameraState;
    public static Director Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
#if UNITY_EDITOR

#else
             DevMode = true;
#endif
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var unit in party.ToList())
            {
                var startingUnit = Instantiate(unit);
                party.Remove(unit);
                DontDestroyOnLoad(startingUnit);
                party.Add(startingUnit);
                LoadDatabases();
                //RunTracker.Instance.partyMembersCollected.Add(startingUnit);
                startingUnit.gameObject.SetActive(false);
                if (!DevMode)
                    startingUnit.currentHP -= 15;
            }
        }

    }

    private void LoadDatabases()
    {
        dialogues = Resources.LoadAll<DialogueHandler>("Dialogue").ToList();
        mapTemplates = Resources.LoadAll<MapTemplate>("MapTemplates").ToList();
        enemyEncounterData = Resources.LoadAll<EnemyEncounterData>("EnemyEncounterData").ToList();
        /*
        actionDatabase = 
        characterdatabase = 
        Unitdatabase =
        itemDatabase =
        VFXList =
        */

    }
    private void Start()
    {
        if (DevMode)
        {
            UnlockedPipSystem = true;
        }
        SceneManager.sceneLoaded += CheckCams;
    }

    private void CheckCams(Scene scene, LoadSceneMode mode)
    {
        Tools.ClearAllEffectPopup();
        if (BattleSystem.Instance != null && canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = LabCamera.Instance.uicam;
            canvas.planeDistance = 20;
        }
        else if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    void Update()
    {

        //Speed Up
        if (Director.Instance.DevMode && SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Time.timeScale += 1;
                print(Time.timeScale);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Time.timeScale -= 1;
                print(Time.timeScale);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale != 0)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
        }
      
        if (Input.GetKeyDown(KeyCode.U) && Director.Instance.DevMode)
        {
            if(canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(false);
            }
            else
            {
                canvas.gameObject.SetActive(true);
            }
            if (LabCamera.Instance != null && LabCamera.Instance.uicam != null)
            {
                if (LabCamera.Instance.uicam.gameObject.activeSelf)
                {
                    LabCamera.Instance.uicam.gameObject.SetActive(false);
                }
                else
                    LabCamera.Instance.uicam.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && BattleSystem.Instance == null && !OptionsManager.Instance.blackScreen.gameObject.activeSelf && SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (SceneManager.GetActiveScene().name == "MAP" /*&& MapController.Instance.CanInput*/)
            {
                if (Director.Instance.CharacterSlotsDisplayed)
                {
                    if (BattleLog.Instance != null)
                        if (BattleLog.Instance.state != BattleLogStates.TALKING)
                            Director.Instance.DisplayCharacterTab(false);
                }
                else if (Director.Instance.ItemTabGrid.transform.childCount == 0)
                {
                    Director.Instance.DisableCharacterTab();
                }
            }

            if (SceneManager.GetActiveScene().name == "Rest" && BattleLog.Instance.state != BattleLogStates.TALKING)
            {
                if (Director.Instance.CharacterSlotsDisplayed)
                {
                    if (BattleLog.Instance != null)
                        if (BattleLog.Instance.state != BattleLogStates.TALKING)
                            Director.Instance.DisplayCharacterTab(false);
                }
                else if (Director.Instance.ItemTabGrid.transform.childCount == 0)
                {
                    Director.Instance.DisableCharacterTab();
                }
            }

        }

    }


    public void CreateCharacterSlots(List<Unit> units)
    {
        int i = 0;
        if (units != null)
        {
            foreach (var x in units)
            {
                if (x != null)
                {
                    CharacterSlot newcharacterSlot = Instantiate(characterSlot, Vector3.zero, Quaternion.identity);
                    newcharacterSlot.unit = x;
                    var healthbar = newcharacterSlot.slider;
                    healthbar.maxValue = x.maxHP;
                    if (x.currentHP < 1)
                    {
                        healthbar.value = x.maxHP;
                    }
                    else
                    {
                        healthbar.value = x.currentHP;
                    }
                    newcharacterSlot.transform.SetParent(characterSlotpos.transform, false);
                    if (x.charPortraits != null)
                    {
                        newcharacterSlot.portrait.sprite = x.charPortraits.Find(obj => obj.name == "neutral");
                    }
                    newcharacterSlot.ResetStats();
                    i++;
                }
            }

        }
    }

    public void CharacterSlotEnable(bool forceDisable = false)
    {
       
        if (forceDisable)
        {
            characterSlotpos.GetComponent<MoveableObject>().Move(true);
            CharacterSlotsDisplayed = false;
            /*if (MapController.Instance.mapControlBar != null)
                if (MapController.Instance.mapControlBar.activeInHierarchy)
                    MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(true);
            */


            if (RestSite.Instance != null)
            {
                foreach (var button in RestSite.Instance.buttons)
                {
                    button.GetComponent<MoveableObject>().Move(false);
                    button.interactable = false;
                }
            }
        }
        else
        {
            if (CharacterSlotsDisplayed)
            {
                characterSlotpos.GetComponent<MoveableObject>().Move(true);
                CharacterSlotsDisplayed = false;
                /*if (MapController.Instance.mapControlBar != null)
                    if (MapController.Instance.mapControlBar.activeInHierarchy)
                        MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(false);
                */

                if (RestSite.Instance != null)
                {
                    foreach (var button in RestSite.Instance.buttons)
                    {
                        button.GetComponent<MoveableObject>().Move(false);
                        button.interactable = false;
                    }
                }
            }
            else
            {
                characterSlotpos.GetComponent<MoveableObject>().Move(false);
                CharacterSlotsDisplayed = true;
                /*if (MapController.Instance.mapControlBar != null)
                    if (MapController.Instance.mapControlBar.activeInHierarchy)
                        MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(true);
                */


                if (RestSite.Instance != null)
                {
                    foreach (var button in RestSite.Instance.buttons)
                    {
                        button.GetComponent<MoveableObject>().Move(false);
                        button.interactable = false;
                    }
                }
            }
        }
    }


    public static void AddUnitToParty(string unitName)
    {
        var unitToAdd = Instantiate(Director.Instance.Unitdatabase.Where(obj => obj.name == unitName).SingleOrDefault());
        DontDestroyOnLoad(unitToAdd);
        unitToAdd.gameObject.SetActive(false);
        Director.Instance.party.Add(unitToAdd);
        //RunTracker.Instance.partyMembersCollected.Add(unitToAdd);
    }

    public void DisplayCharacterTab(bool LevelUp, bool Interactable = false)
    {
        CharacterSlotEnable();
        AudioManager.QuickPlay("ui_woosh_002");

        if (generalCoruntine != null)
        {
            StopCoroutine(generalCoruntine);
        }

        Tools.ToggleUiBlocker(false, true);

        MoveableObject tabGridMovement = TabGrid.GetComponent<MoveableObject>();

        if (tabGridMovement != null)
        {
            tabGridMovement.Move(true);
        }

        /*
         * Level-up mode needs the UI blocker on, matching the
         * old behavior.
         */
        if (LevelUp)
        {
            Tools.ToggleUiBlocker(true, true);

            MoveableObject levelUpTextMovement = LevelUpText.GetComponent<MoveableObject>();

            if (levelUpTextMovement != null)
            {
                levelUpTextMovement.Move(false);
            }

            MoveableObject confirmMovement = ConfirmButton.GetComponent<MoveableObject>();

            if (confirmMovement != null)
            {
                confirmMovement.Move(false);
            }

            Button confirmButton = ConfirmButton.GetComponent<Button>();

            if (confirmButton != null)
            {
                confirmButton.interactable = false;
            }
        }

        /*
         * If the tabs already exist, reactivate and refresh them.
         * This lets the same tabs be reused for level-up mode,
         * normal detail mode, or replacement mode.
         */
        if (TabGrid.transform.childCount > 0)
        {
            foreach (Transform child in TabGrid.transform)
            {
                if (child == null)
                    continue;

                child.gameObject.SetActive(true);

                CharacterTab tab =  child.GetComponent<CharacterTab>();

                if (tab == null || tab.unit == null)
                    continue;

                tab.Init(tab.unit, LevelUp,Interactable);
                Button tabButton = tab.GetComponent<Button>();

                if (tabButton != null)
                {
                    tabButton.interactable = Interactable;
                }
            }
            return;
        }

        /*
         * First-time creation.
         */
        foreach (Unit unit in party)
        {
            if (unit == null || unit.IsSummon)
                continue;

            CharacterTab tab = Instantiate(characterTab,TabGrid.transform, false);

            Button tabButton = tab.GetComponent<Button>();

            if (tabButton != null)
            {
                tabButton.interactable = Interactable;
            }

            tab.Init(unit, LevelUp,Interactable);
        }
    }

    public List<LabLine> FindObjectFromDialogueDatabase(string dialogueName)
    {
        var dialogueToReturn = dialogues.Where(obj => obj.name == dialogueName).SingleOrDefault();
        return dialogueToReturn.LabLines;
    }
   
    public void ToggleEveryButton()
    {
        if (!buttonsAreDisabled)
        {
            foreach (var button in GameObject.FindObjectsOfType<Button>())
            {
                button.interactable = false;
                buttonsAreDisabled = true;
            }
        }
        else
        {
            foreach (var button in GameObject.FindObjectsOfType<Button>())
            {
                button.interactable = true;
                buttonsAreDisabled = false;
            }
        }

    }


    public void DisableCharacterTab(bool MoveBattlelog = true)
    {
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(false);
        AudioManager.QuickPlay("ui_woosh_002");
       /* if (MapController.Instance.mapControlBar != null)
            if (MapController.Instance.mapControlBar.activeInHierarchy)
                MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(true);
       */

        if (RestSite.Instance != null)
        {
            foreach (var button in RestSite.Instance.buttons)
            {
                button.GetComponent<MoveableObject>().Move(true);
                button.interactable = true;
            }
        }


        foreach (var CT in FindObjectsOfType<CharacterTab>())
        {
            foreach (var actionDisplays in CT.actionDisplay.transform.GetComponentsInChildren<Button>())
            {
                actionDisplays.interactable = false;
            }
        }
        if (generalCoruntine != null)
            StopCoroutine(generalCoruntine);

        generalCoruntine = DisablingCharacterTab(MoveBattlelog);
        StartCoroutine(generalCoruntine);
    }

    public IEnumerator DisablingCharacterTab(bool MoveBattleLog)
    {
        characterSlotpos.GetComponent<MoveableObject>().Move(false);
        CharacterSlotsDisplayed = true;
        if (MoveBattleLog)
        {
            Tools.ToggleUiBlocker(true, true);
            BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
            BattleLog.Instance.ClearAllBattleLogText();
            yield return new WaitForSeconds(0.5f);
            foreach (Transform child in Director.Instance.TabGrid.transform)
            {
                if (MoveBattleLog)
                    BattleLog.Instance.GetComponent<MoveableObject>().Move(false);

                Destroy(child.gameObject);
            }
        }
        else
            yield break;
       
    }


    public void LevelUp(Unit unit, bool IncreasesLVL = true)
    {
        if (IncreasesLVL)
        {
            LabyrinthLVL += 1;
        }
        unit.attackStat += 1;
        //unit.speedStat += 1;
        unit.defenseStat += 1;
        unit.maxHP += 2;
        unit.currentHP += 2;
    }


}
