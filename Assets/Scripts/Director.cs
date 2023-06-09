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
    


    public BattleLog BL;
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
    public FPSCounter fPSCounter;
    public bool CharacterSlotsDisplayed;
    IEnumerator generalCoruntine;
    bool buttonsAreDisabled = false;
    public bool DevMode;
    public Image blackScreen;
    public Button backButton;
    public TimeLine timeline;
    public float timelinespeedDelay;
    public float UserTimelineSpeedDelay = 2f;
    public float staminaSPDDivider;
    public GameObject LevelUpText;
    public GameObject EffectPopUp;
    public GameObject ConfirmButton;

    LabCamera.CameraState previousCameraState;
    public static Director Instance { get; private set;  }
     void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
#if UNITY_EDITOR 
            Debug.unityLogger.logEnabled = true;
            
#else
             Debug.unityLogger.logEnabled = false;
             DevMode = true;
#endif
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach(var unit in party.ToList())
            {
                var startingUnit = Instantiate(unit);
                party.Remove(unit);
                DontDestroyOnLoad(startingUnit);
                party.Add(startingUnit);
                startingUnit.gameObject.SetActive(false);
            }   
        }

    }

   
    void Update()
    {
        
        //Quit 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        //Pause
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
                previousCameraState = LabCamera.Instance.state;
                LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                Tools.PauseAllStaminaTimers();
            }
            else
            {
                Time.timeScale = 1;
                LabCamera.Instance.state = previousCameraState;
                Tools.UnpauseAllStaminaTimers();
            }
        }
        //Speed Up
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
        //Put Character Slots Away
        if (Input.GetKeyDown(KeyCode.E) && characterSlotpos != null)
        {
            CharacterSlotEnable();
            print("MOVING CHARACTER SLOTS");
        }
        //Disables and Enables FPS Counter
        if (Input.GetKeyDown(KeyCode.F) && fPSCounter != null)
        {
            FPSCounterEnable();
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
                    newcharacterSlot.healthNumbers.text = $"{healthbar.value} / {x.maxHP}";
                    newcharacterSlot.transform.SetParent(characterSlotpos.transform, false);
                    if (x.charPortraits != null)
                    {
                        newcharacterSlot.portrait.sprite = x.charPortraits.Find(obj => obj.name == "neutral");
                    }
                    newcharacterSlot.stats.text = $"<sprite name=\"ATK\">:{x.attackStat}  <sprite name=\"DEF\">:{x.defenseStat}  <sprite name=\"SPD\">:{x.speedStat}";
                    i++;
                }
            }
            
        }
    }   

    public void CharacterSlotEnable(bool forceDisable = false)
    {
        if (forceDisable)
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(characterSlotpos.GetComponent<RectTransform>(), -1500f, characterSlotpos.GetComponent<RectTransform>().anchoredPosition.y, 0.01f);
            StartCoroutine(generalCoruntine);
            CharacterSlotsDisplayed = false;
        }
        else
        {
            if (CharacterSlotsDisplayed)
            {
                if (generalCoruntine != null)
                    StopCoroutine(generalCoruntine);

                generalCoruntine = Tools.SmoothMoveUI(characterSlotpos.GetComponent<RectTransform>(), -1500f, characterSlotpos.GetComponent<RectTransform>().anchoredPosition.y, 0.01f);
                StartCoroutine(generalCoruntine);
                CharacterSlotsDisplayed = false;
            }
            else
            {
                if (generalCoruntine != null)
                    StopCoroutine(generalCoruntine);

                generalCoruntine = Tools.SmoothMoveUI(characterSlotpos.GetComponent<RectTransform>(), -825f, characterSlotpos.GetComponent<RectTransform>().anchoredPosition.y, 0.01f);
                StartCoroutine(generalCoruntine);
                CharacterSlotsDisplayed = true;
            }
        }
    }

    public void FPSCounterEnable()
    {
        if (fPSCounter.gameObject.activeSelf)
        {
          fPSCounter.gameObject.SetActive(false);
        }
        else
        {
            fPSCounter.gameObject.SetActive(true);
        }
    }
    public static void AddUnitToParty(string unitName)
    {
        var unitToAdd = Instantiate(Director.Instance.characterdatabase.Where(obj => obj.name == unitName).SingleOrDefault());
        DontDestroyOnLoad(unitToAdd);
        unitToAdd.gameObject.SetActive(false);
        Director.Instance.party.Add(unitToAdd);
    }

    public void DisplayCharacterTab(bool LevelUp = true, bool Interactable = false)
    {
        CharacterSlotEnable();
        Tools.ToggleUiBlocker(false);
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(true);
        if (Director.Instance.TabGrid.transform.childCount > 0)
        {
            foreach (Transform child in Director.Instance.TabGrid.transform)
            {
                child.gameObject.SetActive(true);
            }
            BattleLog.Instance.Move(true);
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
                        Tools.ToggleUiBlocker(true);
                        CT.detailedDisplay.SetActive(false);
                        CT.levelupDisplay.SetActive(true);
                        foreach (var x in characterTab.LevelUpButtons)
                        {
                            x.interactable = true;
                        }
                        CT.unit = unit;
                        CT.statDisplay.text = $"HP:{unit.maxHP}\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
                        CT.actionDisplay.gameObject.SetActive(true);
                        SetUpActionList(unit, CT);
                        CT.actionDisplay.gameObject.SetActive(false);
                        LevelUpText.GetComponent<MoveableObject>().Move(false);
                        ConfirmButton.GetComponent<MoveableObject>().Move(false);
                        ConfirmButton.GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        BL.Move(true);
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
                            var x = Instantiate(Director.Instance.BL.itemImage);
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

    /*public void MoveCharacterTabGrid(bool MoveUp)
    {
        if(MoveUp)
        StartCoroutine(Tools.SmoothMoveUI(TabGrid.GetComponent<RectTransform>(), TabGrid.GetComponent<RectTransform>().anchoredPosition.x, 61, 0.01f));
        else
        {
            StartCoroutine(Tools.SmoothMoveUI(TabGrid.GetComponent<RectTransform>(), TabGrid.GetComponent<RectTransform>().anchoredPosition.x, -1020, 0.01f));
        }
    }
    */
    private void SetUpActionList(Unit unit, CharacterTab CT)
    {
        foreach (var action in unit.actionList)
        {
            var actionContainer = Instantiate(CT.detailedAction);
            actionContainer.transform.SetParent(CT.actionDisplay.transform);
            actionContainer.transform.localScale = new Vector3(1, 1, 1);
            var assignedAction = actionContainer.GetComponent<ActionContainer>();
            assignedAction.targetting = false;
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
        //ToggleEveryButton();
        if (generalCoruntine != null)
            StopCoroutine(generalCoruntine);

        generalCoruntine = Tools.SmoothMoveUI(characterSlotpos.GetComponent<RectTransform>(), -825f, characterSlotpos.GetComponent<RectTransform>().anchoredPosition.y, 0.01f);
        StartCoroutine(generalCoruntine);
        CharacterSlotsDisplayed = true;
        Tools.ToggleUiBlocker(true);
        BL.Move(false);
        BattleLog.ClearAllBattleLogText();
        foreach (Transform child in Director.Instance.TabGrid.transform)
        {
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
    public IEnumerator DoLoad(string SceneToLoad)
    {
        blackScreen.gameObject.SetActive(true);
        Director.Instance.StartCoroutine(Tools.FadeObject(blackScreen, 0.001f, true));
        yield return new WaitUntil(() => blackScreen.color == new Color(0, 0, 0, 1));
        yield return new WaitForSeconds(1f);
        print("TRANSITIONED");
        SceneManager.LoadScene(SceneToLoad);
    }
}
