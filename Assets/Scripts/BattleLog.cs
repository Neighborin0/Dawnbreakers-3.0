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
using UnityEngine.Rendering.PostProcessing;
using static System.Collections.Specialized.BitVector32;

public enum BattleLogStates { IDLE, TALKING }
public class BattleLog : MonoBehaviour
{
    public static BattleLog Instance { get; private set; }
    public TextMeshProUGUI ambientText;
    private Unit displayingEnemyStatsFor;
    public BattleLogStates state;

    int index;
    //character stat text
    public TextMeshProUGUI STATtext;
    public TextMeshProUGUI enemySTATtext;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI enemyIntent;
    public GameObject LineText;


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
            state = BattleLogStates.IDLE;
        }

    }

    public void Start()
    {
        
    }

   
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

    public void CharacterDialog(List<LabLine> dialog, bool PausesBattle = false, bool disableAfter = true, bool ambientText = false, bool EndBattle = false, bool TurnOffUiAfter = true, bool CheckForPause = true, bool ResetBackToIdleState = true)
    {
        ClearAllBattleLogText();
        Portraitparent.gameObject.SetActive(true);
        characterdialog.gameObject.SetActive(true);
        if (!ambientText)
        {
            StartCoroutine(TypeMultiText(dialog, characterdialog, disableAfter, PausesBattle, EndBattle, TurnOffUiAfter, CheckForPause, ResetBackToIdleState));
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
    /*public void CreateRandomAmbientText()
    {
        var text = ambience[UnityEngine.Random.Range(0, ambience.Length)];
        ambientText.text = "";
        StartCoroutine(TypeText(text, 0.03f, ambientText, false));
    }
    */

    public void DisplayPlayerStats(Unit unit)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.ambientText.gameObject.SetActive(false);
        foreach (Transform item in battlelog.inventoryDisplay.transform)
        {
            Destroy(item.gameObject);
        }
        battlelog.STATtext.gameObject.SetActive(true);

        var resistanceText = new List<string>();
        var weaknessText = new List<string>();


        for (int i = 0; i < unit.resistances.Length; i++)
        {
            string stringToAdd = $"<sprite name=\"{unit.resistances[i]}_ALT\">";
            resistanceText.Add(stringToAdd);
        }
        for (int i = 0; i < unit.weaknesses.Length; i++)
        {
            string stringToAdd = $"<sprite name=\"{unit.weaknesses[i]}_ALT\">";
            weaknessText.Add(stringToAdd);
        }


        battlelog.STATtext.text = $"ATK: {unit.attackStat}\nDEF: {unit.defenseStat}\nRES:  {string.Join("", resistanceText.ToArray())}\nWEAK:  {string.Join("", weaknessText.ToArray())}";


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
        battlelog.LineText.SetActive(true);

        /*if(TurnOffItemDisplay)
        {
            battlelog.inventoryDisplay.gameObject.SetActive(false);
        }
        */
        foreach (var item in unit.inventory)
        {
            var x = Instantiate(battlelog.itemImage);
            x.image.sprite = item.sprite;
            x.GetComponent<ItemText>().item = item;
            var rect = x.transform.GetComponent<RectTransform>().anchoredPosition3D;
            x.transform.GetComponent<RectTransform>().SetParent(battlelog.inventoryDisplay.transform.GetComponent<RectTransform>());
            x.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(rect.x, rect.y, rect.z);
            x.transform.rotation = Quaternion.identity;
        }
    }
    public void DisplayCharacterStats(Unit unit)
    {
        if (unit.IsPlayerControlled)
        {
            DisplayPlayerStats(unit);
        }
        else
        {

            if (displayingEnemyStatsFor != unit)
            {
                DisplayEnemyCharacterStats(unit);
                displayingEnemyStatsFor = unit;
            }
        }

    }

    public void DisplayEnemyCharacterStats(Unit unit)
    {
        var resistanceText = new List<string>();
        var weaknessText = new List<string>();


        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.enemySTATtext.gameObject.SetActive(true);
        for (int i = 0; i < unit.resistances.Length; i++)
        {
            string stringToAdd = $"<sprite name=\"{unit.resistances[i]}_ALT\">";
            resistanceText.Add(stringToAdd);
        }
        for (int i = 0; i < unit.weaknesses.Length; i++)
        {
            string stringToAdd = $"<sprite name=\"{unit.weaknesses[i]}_ALT\">";
            weaknessText.Add(stringToAdd);
        }


        battlelog.enemySTATtext.text = $"ATK: {unit.attackStat}\nDEF: {unit.defenseStat}\nRES:  {string.Join("", resistanceText.ToArray())}\nWEAK:  {string.Join("", weaknessText.ToArray())}";
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
        LineText.gameObject.SetActive(false);

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


    //Clears Item Text
    public void ClearBattleText()
    {
        itemText.gameObject.SetActive(false);
    }
    ///<summary>
    ///Used for Clearing all text and creating new ambient text
    ///</summary>
    public void ResetBattleLog()
    {
        DisableCharacterStats();
        ambientText.gameObject.SetActive(false);
        //CreateRandomAmbientText();
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
        var unit = BattleSystem.Instance.playerUnits[0];
        if (unit.levelUpScreenQuotes.Count > 0)
        {
            if (BattleSystem.Instance.TutorialNode)
                CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase(unit.levelUpScreenQuotes[0].name), false, false, true);
            else
                CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase(unit.levelUpScreenQuotes[1].name), false, false, true);
        }

    }
    public void DoPostBattleDialouge(MapController MC)
    {
        CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaPostMeeting"), true, true);
        MapController.Instance.ReEnteredMap -= DoPostBattleDialouge;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            textSpeed = overrideTextSpd;
        }
    }

    private IEnumerator TypeMultiText(List<LabLine> line, TMP_Text x, bool disableAfter, bool Pauses = false, bool EndBattle = false, bool TurnOffUiAfter = true, bool CheckForPause = true, bool ResetBackToIdleState = true)
    {
        BattleStates previousState = BattleStates.IDLE;
        BattleLog.Instance.state = BattleLogStates.TALKING;
        GetComponent<MoveableObject>().Move(true);
        AudioManager.QuickPlay("ui_woosh_001");
        bool WasPaused = false;

        if (Pauses && BattleSystem.Instance != null)
        {
            previousState = BattleSystem.Instance.state;
            foreach (var unit in Tools.GetAllUnits())
            {
                unit.state = PlayerState.DECIDING;
                unit.StaminaHighlightIsDisabled = true;
                unit.ExitDecision();
                if (!unit.IsPlayerControlled)
                {
                    unit.intentUI.gameObject.SetActive(false);
                }
            }
            LabCamera.Instance.uicam.gameObject.SetActive(false);
            Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(false);
            print("BATTLE SHOULD BE PAUSED");
            ClearAllBattleLogText();
            BattleSystem.Instance.state = BattleStates.TALKING;
        }

        foreach (var l in line)
        {
            x.text = "";
            Portraitparent.gameObject.SetActive(true);
            Tools.ToggleUiBlocker(false, true, true);

            if (l.PositionToMoveTo != Vector3.zero)
            {
                LabCamera.Instance.MoveToPosition(l.PositionToMoveTo);
            }

            if (l.CameraRotation != Vector3.zero)
            {
                LabCamera.Instance.Rotate(l.CameraRotation);
            }

            foreach (var unit in Tools.GetAllUnits())
            {
                if (!unit.IsPlayerControlled)
                {
                    unit.intentUI.gameObject.SetActive(false);
                }
            }

            l.OnLineStarted.Invoke();

            if (Director.Instance.Unitdatabase.FirstOrDefault(obj => obj.name == l.unit)?.charPortraits.FirstOrDefault(obj => obj.name == l.expression) != null)
            {
                charPortrait.sprite = Director.Instance.Unitdatabase.FirstOrDefault(obj => obj.name == l.unit)?.charPortraits.FirstOrDefault(obj => obj.name == l.expression);
            }
            else
            {
                charPortrait.sprite = Director.Instance.Unitdatabase.FirstOrDefault(obj => obj.name == l.unit)?.charPortraits.FirstOrDefault(obj => obj.name == "neutral");
            }

            textSpeed = l.textSpeed;
            int characterIndex = 0;
            bool Ignoring = false;
            foreach (char letter in l.text.ToCharArray())
            {
                Portraitparent.gameObject.SetActive(true);
                characterIndex++;
                //So colors don't get formatted
                if(letter.ToString() == "<")
                {
                    Ignoring = true;
                }
                else if(letter.ToString() == ">")
                {
                    Ignoring = false;
                }
               
                if (!Ignoring)
                {
                    string textToWrite = l.text.Substring(0, characterIndex) + "<color=#00000000>" + l.text.Substring(characterIndex) + "</color>";
                    x.text = textToWrite;
                    if (letter.ToString() == ",")
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield return new WaitForSeconds(textSpeed * OptionsManager.Instance.textSpeedMultiplier / 2.5f);
                }
            }

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0));
            AudioManager.QuickPlay("button_Hit_006", true);
            characterdialog.text = "";
            yield return new WaitForSeconds(0.01f);
            l.OnLineEnded.Invoke();
        }

        if (Pauses && BattleSystem.Instance != null)
        {
            DisableCharacterDialog();
            if (!EndBattle)
            {
                BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
                BattleSystem.Instance.state = previousState;
                ResetBattleLog();

                if (ResetBackToIdleState)
                {
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        unit.state = PlayerState.IDLE;

                        if (unit.IsPlayerControlled)
                        {
                            unit.StaminaHighlightIsDisabled = true;
                        }

                        if (unit.health != null)
                        {
                            unit.health.DeathPaused = false;
                        }

                        if (!unit.IsPlayerControlled)
                        {
                            unit.intentUI.gameObject.SetActive(true);
                        }
                    }

                    if (CheckForPause)
                    {
                        if (!BattleSystem.Instance.BattlePhasePause)
                        {
                            WasPaused = true;
                        }

                        yield return new WaitUntil(() => !BattleSystem.Instance.BattlePhasePause);
                        Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
                        BattleLog.Instance.GetComponent<MoveableObject>().Move(true);

                        if (previousState != BattleStates.BATTLE)
                        {
                            BattleSystem.Instance.playerUnits[0].StartDecision();
                        }
                        else if (WasPaused)
                        {
                            BattleSystem.Instance.BattlePhasePause = false;
                        }
                        else
                        {
                            CombatTools.UnpauseStaminaTimer();
                        }
                    }
                    else
                    {
                        BattleLog.Instance.state = BattleLogStates.IDLE;
                    }

                    if (TurnOffUiAfter)
                    {
                        LabCamera.Instance.uicam.gameObject.SetActive(true);
                    }

                    Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
                }
            }
        }

        if (RestSite.Instance != null)
        {
            GetComponent<MoveableObject>().Move(false);
        }

        Tools.ToggleUiBlocker(true, true, true);

        if (disableAfter)
        {
            x.text = "";
            ClearAllBattleLogText();
            characterdialog.gameObject.SetActive(false);
            GetComponent<MoveableObject>().Move(false);
        }

        BattleLog.Instance.state = BattleLogStates.IDLE;
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
