using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor;
using UnityEngine.Rendering.Universal.Internal;
using System.Diagnostics.Contracts;

public class BattleLog : MonoBehaviour
{
    public static BattleLog Instance { get; private set; }
    public TextMeshProUGUI ambientText;
    private Unit displayingEnemyStatsFor;

    int index;
    //character stat text
    public TextMeshProUGUI STATtext;
    public TextMeshProUGUI enemySTATtext;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI enemyIntent;


    //character dialog
    public Image charPortrait;
    public Image Portraitparent;

    public Image enemycharPortrait;
    public Image enemyPortraitparent;


    public TextMeshProUGUI characterdialog;
    public Image indicator;

    public GridLayoutGroup ActionLayout;
    public ActionContainer genericActionContainer;
    public GridLayoutGroup inventoryDisplay;
    public Button itemImage;
    public TextMeshProUGUI itemText;
    IEnumerator generalCoroutine;
    public float overrideTextSpd = 0f;
    public float textSpeed;
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

    }

    private static string[] ambience = new string[]
        {
            "...",

        };


    public void CreateActionLayout(Unit unit)
    {
        var layout = Instantiate(ActionLayout, transform);
        unit.ActionLayout = layout.gameObject;
        unit.ActionLayout.gameObject.SetActive(true);
        for (int i = 0; i < unit.actionList.Count; i++)
        {
            var container = Instantiate(genericActionContainer, layout.transform) as ActionContainer;
            container.action = unit.actionList[i];
            unit.skillUIs[i] = container.gameObject;
            container.baseUnit = unit;
        }
    }

    public void ClearAmbientText()
    {
        ambientText.text = "";
    }

    public void CharacterDialog(List<LabLine> dialog, bool PausesBattle = false, bool disableAfter = true, bool ambientText = false, bool EndBattle = false)
    {
        ClearAllBattleLogText();
        Portraitparent.gameObject.SetActive(true);
        characterdialog.gameObject.SetActive(true);
        if (!ambientText)
        {
            StartCoroutine(TypeMultiText(dialog, characterdialog, disableAfter, PausesBattle, EndBattle));
        }
        else
            AmbientCharacterText(dialog, characterdialog);
       
    }

    public void DisableCharacterDialog()
    {
        characterdialog.gameObject.SetActive(false);
    }

    public void SetRandomAmbientTextActive()
    {
       ambientText.gameObject.SetActive(true);
    }

    public void SetRandomAmbientTextOff()
    {
        ambientText.gameObject.SetActive(false);
    }
    public void CreateRandomAmbientText()
    {
        var text = ambience[UnityEngine.Random.Range(0, ambience.Length)];
        ambientText.text = "";
        StartCoroutine(TypeText(text, 0.03f, ambientText, false));
    }

    public void DisplayPlayerStats(Unit unit, bool TurnOffItemDisplay = false)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.ambientText.gameObject.SetActive(false);
        foreach (Transform item in battlelog.inventoryDisplay.transform)
        {
            Destroy(item.gameObject);
        }
        battlelog.STATtext.gameObject.SetActive(true);
        battlelog.STATtext.text = $"<sprite name=\"ATK\">ATK: {unit.attackStat}\n<sprite name=\"DEF\">DEF: {unit.defenseStat}\n<sprite name=\"SPD\">SPD: {unit.speedStat}";
        Tools.SetTextColorAlphaToZero(battlelog.STATtext);
        StartCoroutine(Tools.FadeText(battlelog.STATtext, 0.005f, true, false));
        battlelog.charPortrait.sprite = unit.charPortraits.Find(obj => obj.name == "neutral");
        Tools.SetImageColorAlphaToZero(battlelog.charPortrait);
        StartCoroutine(Tools.FadeObject(battlelog.charPortrait, 0.005f, true, false));
        battlelog.Portraitparent.gameObject.SetActive(true);
        Tools.SetImageColorAlphaToZero(battlelog.Portraitparent);
        StartCoroutine(Tools.FadeObject(battlelog.Portraitparent, 0.005f, true, false));
        battlelog.characterName.gameObject.SetActive(true);
        Tools.SetTextColorAlphaToZero(battlelog.characterName);
        StartCoroutine(Tools.FadeText(battlelog.itemText, 0.005f, true, false));
        battlelog.inventoryDisplay.gameObject.SetActive(true);
        battlelog.itemText.gameObject.SetActive(true);
        Tools.SetTextColorAlphaToZero(battlelog.itemText);
        StartCoroutine(Tools.FadeText(battlelog.itemText, 0.005f, true, false));
        battlelog.characterName.text = (unit.unitName);

        if(TurnOffItemDisplay)
        {
            battlelog.inventoryDisplay.gameObject.SetActive(false);
        }
        foreach (var item in unit.inventory)
        {
            var x = Instantiate(battlelog.itemImage);  
            x.image.sprite = item.sprite;
            x.GetComponent<ItemText>().item = item;
            x.transform.SetParent(battlelog.inventoryDisplay.transform);
        }
    }
    public void DisplayCharacterStats(Unit unit, bool TurnOffItemDisplay = false)
    {
        if (unit.IsPlayerControlled)
        {
            DisplayPlayerStats(unit, TurnOffItemDisplay);
        }
        else
        {
           
            if(displayingEnemyStatsFor != unit)
            {
                DisplayEnemyCharacterStats(unit);
                displayingEnemyStatsFor = unit;
            }
        }

    }

    public void DisplayEnemyCharacterStats(Unit unit)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.enemySTATtext.gameObject.SetActive(true);
        battlelog.enemySTATtext.text = $"<sprite name=\"ATK\">ATK: {unit.attackStat}\n<sprite name=\"DEF\">DEF: {unit.defenseStat}\n<sprite name=\"SPD\">SPD: {unit.speedStat}";
        battlelog.enemycharPortrait.sprite = unit.charPortraits.Find(obj => obj.name == "neutral");
        battlelog.enemyPortraitparent.gameObject.SetActive(true);
        battlelog.characterName.gameObject.SetActive(true);
        battlelog.itemText.gameObject.SetActive(true);
        battlelog.characterName.text = (unit.unitName);
        battlelog.ambientText.gameObject.SetActive(false);
        Tools.SetTextColorAlphaToZero(battlelog.enemySTATtext);
        StartCoroutine(Tools.FadeText(battlelog.enemySTATtext, 0.005f, true, false));
        Tools.SetImageColorAlphaToZero(battlelog.enemycharPortrait);
        StartCoroutine(Tools.FadeObject(battlelog.enemycharPortrait, 0.005f, true, false));
        Tools.SetImageColorAlphaToZero(battlelog.enemyPortraitparent);
        StartCoroutine(Tools.FadeObject(battlelog.enemyPortraitparent, 0.005f, true, false));

    }


    public void DisableCharacterStats()
    {
        STATtext.gameObject.SetActive(false);
        enemySTATtext.gameObject.SetActive(false);
        Portraitparent.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        enemyPortraitparent.gameObject.SetActive(false);

        itemText.gameObject.SetActive(false);
        itemText.text = "";
        displayingEnemyStatsFor = null;
        foreach (var item in inventoryDisplay.GetComponentsInChildren<Button>())
        {
            Destroy(item.gameObject);
        }
    }
    public void DisplayEnemyIntentInfo(string description, Unit unit)
    {
        enemyIntent.gameObject.SetActive(true);
        enemyIntent.text = ($"{description}");
        DisplayCharacterStats(unit);
    }

    public void DoBattleText(string text)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.ambientText.gameObject.SetActive(false);
        Tools.SetTextColorAlphaToZero(battlelog.ambientText);
        StartCoroutine(Tools.FadeText(battlelog.ambientText, 0.005f, true, false));
        itemText.gameObject.SetActive(true);
        itemText.text = text;
    }

    public void ClearBattleText()
    {
        itemText.gameObject.SetActive(false);
    }

    //Used for Clearing all text and creating new ambient text
    public void ResetBattleLog()
    {
        DisableCharacterStats();
        SetRandomAmbientTextActive();
        CreateRandomAmbientText();
        ClearBattleText();
        foreach (var z in Tools.GetAllUnits())
        {
            z.IsHighlighted = false;
            z.isDarkened = false;

        }
    }

    //Creates a clean slate for Battle Text
    public void ClearAllBattleLogText()
    {
        itemText.gameObject.SetActive(false);
        DisableCharacterStats();
        ambientText.gameObject.SetActive(false);
        enemyIntent.gameObject.SetActive(false);
    }

    public void DoRandomLevelUpScreenDialogue()
    {
       var unit = BattleSystem.Instance.playerUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.playerUnits.Count)];
            if(unit.levelUpScreenQuotes.Count > 0)
            CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase(unit.levelUpScreenQuotes[UnityEngine.Random.Range(0, unit.levelUpScreenQuotes.Count)].name), false, false, true); 
    }
    public void DoPostBattleDialouge(MapController MC)
    {
        CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaPostMeeting"), true, true);
        MapController.Instance.ReEnteredMap -= DoPostBattleDialouge;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            textSpeed = overrideTextSpd;
        }
    }

    private IEnumerator TypeMultiText(List<LabLine> text, TMP_Text x, bool disableAfter, bool Pauses = false, bool EndBattle = false)
    {
        BattleStates previousState = BattleStates.IDLE;
        GetComponent<MoveableObject>().Move(true);      
        if (Pauses)
        {
            if (BattleSystem.Instance != null)
            {
                previousState = BattleSystem.Instance.state;
                foreach (var unit in Tools.GetAllUnits())
                {
                    unit.state = PlayerState.DECIDING;
                    unit.StaminaHighlightIsDisabled = true;
                    unit.ExitDecision();
                    if(!unit.IsPlayerControlled)
                    {
                        unit.intentUI.gameObject.SetActive(false);
                    }
                }
                LabCamera.Instance.uicam.gameObject.SetActive(false);
                Director.Instance.timeline.GetComponent<MoveableObject>().Move(false);
                Tools.PauseAllStaminaTimers();
                print("BATTLE SHOULD BE PAUSED");
                ClearAllBattleLogText();
                BattleSystem.Instance.state = BattleStates.TALKING;
            }
        }

        for (int i = 0; i < text.Count; i++)
        {
            x.text = "";
            Portraitparent.gameObject.SetActive(true);
            Tools.ToggleUiBlocker(false, true, true);
            if (text[i].PositionToMoveTo != Vector3.zero)
            {
                LabCamera.Instance.MoveToPosition((text[i].PositionToMoveTo));  
            }
            if (text[i].CameraRotation != Vector3.zero)
            {
                LabCamera.Instance.Rotate(text[i].CameraRotation);
            }
            foreach (var unit in Tools.GetAllUnits())
            {
                if(!unit.IsPlayerControlled)
                unit.intentUI.gameObject.SetActive(false);
            }
                text[i].OnLineStarted.Invoke();


            if (Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == text[i].expression) != null)
                 charPortrait.sprite = Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == text[i].expression);
            else
                charPortrait.sprite = Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == "neutral");

            textSpeed = text[i].textSpeed;
            foreach (char letter in text[i].text.ToCharArray())
            {
                 x.text += letter;
                 yield return new WaitForSeconds(textSpeed);
            }
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            characterdialog.text = "";
            yield return new WaitForSeconds(0.01f);
            text[i].OnLineEnded.Invoke();
        }
        if (Pauses)
        {
            if (BattleSystem.Instance != null)
            {
                DisableCharacterDialog();
                if (!EndBattle)
                {
                    BattleSystem.Instance.state = previousState;
                    BattleSystem.Instance.BattlePhasePause = false;
                    Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
                    ResetBattleLog();
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (unit.IsPlayerControlled)
                            unit.state = PlayerState.WAITING;
                        else
                            unit.state = PlayerState.IDLE;

                        unit.StaminaHighlightIsDisabled = false;
                        if (unit.health != null)
                            unit.health.DeathPaused = false;
                        if (!unit.IsPlayerControlled)
                            unit.intentUI.gameObject.SetActive(true);
                        LabCamera.Instance.uicam.gameObject.SetActive(true);
                    }
                }
            }
            if (RestSite.Instance != null)
            {
                GetComponent<MoveableObject>().Move(false);
            }
            Tools.ToggleUiBlocker(true, true, true);
        }
        else
        {
            GetComponent<MoveableObject>().Move(false);
            Tools.ToggleUiBlocker(true, true, true);
        }
        if (disableAfter)
        {
            x.text = "";
            ClearAllBattleLogText();
            characterdialog.gameObject.SetActive(false);
            GetComponent<MoveableObject>().Move(false);
        }
    }

    private void AmbientCharacterText(List<LabLine> text, TMP_Text x)
    {
        GetComponent<MoveableObject>().Move(true);
        ClearAllBattleLogText();
        Portraitparent.gameObject.SetActive(true);
        for (int i = 0; i < text.Count; i++)
        {
            if (Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == text[i].expression) != null)
                charPortrait.sprite = Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == text[i].expression);
            else
                charPortrait.sprite = Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == "neutral");

            x.text = text[i].text;
        }

    }
    private IEnumerator TypeText(string text, float textSpeed, TMP_Text x, bool disableAfter)
    {
        foreach (char letter in text)
        {
            x.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        if (disableAfter)
        {
            x.text = "";
        }

    }


}
