using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;


public class Director : MonoBehaviour
{
    public List<Unit> party;
    public List<Unit> characterdatabase;
    public List<Unit> Unitdatabase;
    public List<Item> itemDatabase;
    public List<GameObject> VFXList;
    public List<Action> actionDatabase;
    public List<GameObject> iconDatabase;
    public static List<DialogueHandler> dialogues;



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
    public TimeLine timeline;
    public float timelinespeedDelay;
    public float staminaSPDDivider;
    public GameObject LevelUpText;
    public TextMeshProUGUI chooseYourItemText;
    public GameObject EffectPopUp;
    public GameObject ConfirmButton;
    public GameObject CharacterSlotButtonprefab;
    public Image LevelDropText;
    //public TextMeshProUGUI LevelDropSubText;

    public LabCamera.CameraState previousCameraState;
    public static Director Instance { get; private set;  }
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
             DevMode = false;
#endif
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dialogues = Resources.LoadAll<DialogueHandler>("Dialogue").ToList();
            foreach (var unit in party.ToList())
            {
                var startingUnit = Instantiate(unit);
                party.Remove(unit);
                DontDestroyOnLoad(startingUnit);
                party.Add(startingUnit);
                //RunTracker.Instance.partyMembersCollected.Add(startingUnit);
                startingUnit.gameObject.SetActive(false);
                if(!DevMode)
                startingUnit.currentHP -= 15;
            }   
        }

    }

   
    void Update()
    {
        
        //Speed Up
        if(Director.Instance.DevMode && SceneManager.GetActiveScene().name != "Main Menu")
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
        //Put Character Slots Away
        if (Input.GetKeyDown(KeyCode.E) && BattleSystem.Instance == null && !OptionsManager.Instance.blackScreen.gameObject.activeSelf && SceneManager.GetActiveScene().name != "Main Menu")
        {
            if(CharacterSlotsDisplayed)
            DisplayCharacterTab(false);
            else if(ItemTabGrid.transform.childCount == 0)
                DisableCharacterTab();
            print("MOVING CHARACTER SLOTS");
        }
        
    }

    public void CreateCharacterSlots(List<Unit> units)
    {
        int i = 0;
        //Instantiate(CharacterSlotButtonprefab, canvas.transform);
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
                    //CharacterSlotButtonprefab.GetComponent<RectTransform>().anchoredPosition = newcharacterSlot.GetComponent<RectTransform>().anchoredPosition + new Vector2(100, 0);
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
            if (MapController.Instance.mapControlBar != null)
                MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(true);
        }
        else
        {
            if (CharacterSlotsDisplayed)
            {
                characterSlotpos.GetComponent<MoveableObject>().Move(true);
                CharacterSlotsDisplayed = false;
                if (MapController.Instance.mapControlBar != null)
                    MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(false);
            }
            else
            {
                characterSlotpos.GetComponent<MoveableObject>().Move(false);
                CharacterSlotsDisplayed = true;
                if (MapController.Instance.mapControlBar != null)
                    MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(true);
            }
        }
    }

 
    public static void AddUnitToParty(string unitName)
    {
        var unitToAdd = Instantiate(Director.Instance.characterdatabase.Where(obj => obj.name == unitName).SingleOrDefault());
        DontDestroyOnLoad(unitToAdd);
        unitToAdd.gameObject.SetActive(false);
        Director.Instance.party.Add(unitToAdd);
        //RunTracker.Instance.partyMembersCollected.Add(unitToAdd);
    }

    public void DisplayCharacterTab(bool LevelUp = true, bool Interactable = false)
    {
        CharacterSlotEnable();
        if (generalCoruntine != null)
            StopCoroutine(generalCoruntine);
        BattleLog.Instance.ClearAllBattleLogText();
        Tools.ToggleUiBlocker(false, true);
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(true);
        if (Director.Instance.TabGrid.transform.childCount > 0)
        {
            foreach (Transform child in Director.Instance.TabGrid.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var unit in party)
            {
                if (!unit.IsSummon)
                {
                    CharacterTab CT = Instantiate(characterTab);
                    if(Interactable)
                    { 
                        CT.GetComponent<Button>().interactable = true;
                    }
                    CT.gameObject.transform.SetParent(TabGrid.transform, false);
                    if (LevelUp)
                    {
                        Tools.ToggleUiBlocker(true, true);
                        CT.detailedDisplay.SetActive(false);
                        CT.levelupDisplay.SetActive(true);
                        foreach (var x in characterTab.LevelUpButtons)
                        {
                            x.interactable = true;
                        }
                        CT.unit = unit;
                        CT.DEFText.text = $"DEF: {unit.defenseStat}";
                        CT.ATKtext.text = $"ATK: {unit.attackStat}";
                        CT.HPtext.text = $"HP: {unit.maxHP}";
                        CT.SPDText.text = $"SPD: {unit.speedStat}";
                        CT.actionDisplay.gameObject.SetActive(true);
                        SetUpActionList(unit, CT);
                        CT.actionDisplay.gameObject.SetActive(false);
                        LevelUpText.GetComponent<MoveableObject>().Move(false);
                        ConfirmButton.GetComponent<MoveableObject>().Move(false);
                        ConfirmButton.GetComponent<Button>().interactable = false;

                    }
                    else
                    {
                        CT.levelupDisplay.SetActive(false);
                        CT.detailedDisplay.SetActive(true);
                        SetUpActionList(unit, CT);
                        CT.unit = unit;
                        CT.DEFText.text = $"DEF: {unit.defenseStat}";
                        CT.ATKtext.text = $"ATK: {unit.attackStat}";
                        CT.HPtext.text = $"HP: {unit.maxHP}";
                        CT.SPDText.text = $"SPD: {unit.speedStat}";
                        CT.inventoryDisplay.gameObject.SetActive(true);
                        CT.actionDisplay.gameObject.SetActive(true);

                        
                        foreach (var item in unit.inventory)
                        {
                            var x = Instantiate(BattleLog.Instance.itemImage);
                            x.image.sprite = item.sprite;
                            x.GetComponent<ItemText>().item = item;
                            x.GetComponent<ItemText>().unit = unit;
                            x.transform.SetParent(CT.inventoryDisplay.transform);
                            x.transform.localScale = new Vector3(1, 1, 1);

                        }
                        CT.characterTransfer.interactable = Interactable;
                        CT.inventoryDisplay.gameObject.SetActive(false);
                    }

                }
            }
        }

    }
    public List<LabLine> FindObjectFromDialogueDatabase(string dialogueName)
    {
        var dialogueToReturn = dialogues.Where(obj => obj.name == dialogueName).SingleOrDefault();
        return dialogueToReturn.LabLines;
    }
    private void SetUpActionList(Unit unit, CharacterTab CT)
    {
        foreach (var action in unit.actionList)
        {
            var actionContainer = Instantiate(CT.detailedAction);
            actionContainer.transform.SetParent(CT.actionDisplay.transform);
            actionContainer.transform.localScale = new Vector3(1, 1, 1);
            var assignedAction = actionContainer.GetComponent<ActionContainer>();
            assignedAction.targetting = false;
            assignedAction.baseUnit = unit;
            assignedAction.button.interactable = true;
            assignedAction.button.enabled = true;
            assignedAction.action = action;
            assignedAction.damageNums.text = "<sprite name=\"ATK\">" + (action.damage + unit.attackStat).ToString();
            assignedAction.durationNums.text = "<sprite name=\"Duration\">" + (action.duration).ToString();
            assignedAction.costNums.text = action.cost * unit.actionCostMultiplier < 100 ? $"{action.cost * unit.actionCostMultiplier}%" : $"100%";
            assignedAction.costNums.color = Color.yellow;
            assignedAction.textMesh.text = action.ActionName;
            if(assignedAction.action.New)
            {
                assignedAction.GetComponent<Image>().material = Instantiate<Material>(assignedAction.GetComponent<Image>().material);
                assignedAction.GetComponent<Image>().material.SetFloat("OutlineThickness", 2);
                assignedAction.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                assignedAction.transform.SetAsFirstSibling();
            }
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
        }
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


    public void DisableCharacterTab()
    {
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(false);
        foreach (var CT in FindObjectsOfType<CharacterTab>())
        {
            foreach (var actionDisplays in CT.actionDisplay.transform.GetComponentsInChildren<Button>())
            {
                actionDisplays.interactable = false;
            }
        }
        if (generalCoruntine != null)
            StopCoroutine(generalCoruntine);

        generalCoruntine = DisablingCharacterTab();
        StartCoroutine(generalCoruntine);
    }

    public IEnumerator DisablingCharacterTab()
    {
        characterSlotpos.GetComponent<MoveableObject>().Move(false);
        CharacterSlotsDisplayed = true;
        Tools.ToggleUiBlocker(true, true);
        BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
        BattleLog.Instance.ClearAllBattleLogText();
        yield return new WaitForSeconds(0.5f);
        foreach (Transform child in Director.Instance.TabGrid.transform)
        {
            BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
            Destroy(child.gameObject);
        }
    }
    

        public void LevelUp(Unit unit, bool IncreasesLVL = true)
        {
            if (IncreasesLVL)
            {
              LabyrinthLVL += 1;
            }
             unit.attackStat += 1;
             unit.speedStat += 1;
             unit.defenseStat += 1;
             unit.maxHP += 2;
             unit.currentHP += 2;
        }

  
}
