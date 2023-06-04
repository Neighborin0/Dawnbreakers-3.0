using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;


public class BattleLog : MonoBehaviour
{
    public static BattleLog Instance { get; private set; }
    public TextMeshProUGUI ambientText;
    public TextMeshProUGUI battleText;

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

    }

    private static string[] ambience = new string[]
        {
            "...",

        };


    public void CreateActionLayout(Unit unit)
    {
        var layout = Instantiate(ActionLayout, transform);
        unit.ActionLayout = layout.gameObject;
        for (int i = 0; i < unit.actionList.Count; i++)
        {
            var container = Instantiate(genericActionContainer, layout.transform) as ActionContainer;
            container.action = unit.actionList[i];
            unit.skillUIs[i] = container.gameObject;
            container.baseUnit = unit;
        }
    }

    public static void ClearAmbientText()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.ambientText.text = "";
    }

    public static void CharacterDialog(List<LabLine> dialog, bool PausesBattle = false, bool disableAfter = true)
    {

        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        BattleLog.ClearAllBattleLogText();
        battlelog.Portraitparent.gameObject.SetActive(true);
        battlelog.characterdialog.gameObject.SetActive(true);
        battlelog.StartCoroutine(battlelog.TypeMultiText(dialog, battlelog.characterdialog, disableAfter, PausesBattle));
    }

    public static void DisableCharacterDialog()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.characterdialog.gameObject.SetActive(false);
    }


    public static void SetRandomAmbientTextActive()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        BattleLog.ClearAllBattleLogText();
        battlelog.ambientText.gameObject.SetActive(true);
    }

    public static void SetRandomAmbientTextOff()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.ambientText.gameObject.SetActive(false);
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
        foreach (Transform item in battlelog.inventoryDisplay.transform)
        {
            Destroy(item.gameObject);
        }
        battlelog.STATtext.gameObject.SetActive(true);
        battlelog.STATtext.text = $"<sprite name=\"ATK\">ATK: {unit.attackStat}\n<sprite name=\"DEF\">DEF: {unit.defenseStat}\n<sprite name=\"SPD\">SPD: {unit.speedStat}";
        battlelog.charPortrait.sprite = unit.charPortraits.Find(obj => obj.name == "neutral");
        battlelog.Portraitparent.gameObject.SetActive(true);
        battlelog.characterName.gameObject.SetActive(true);
        battlelog.inventoryDisplay.gameObject.SetActive(true);
        battlelog.itemText.gameObject.SetActive(true);
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
    public static void DisplayCharacterStats(Unit unit, bool TurnOffItemDisplay = false)
    {
        var battlelog = BattleLog.Instance;
        if (unit.IsPlayerControlled)
        {
            battlelog.DisplayPlayerStats(unit, TurnOffItemDisplay);
        }
        else
        {
            battlelog.DisplayEnemyCharacterStats(unit);
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
    }


    public static void DisableCharacterStats()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.STATtext.gameObject.SetActive(false);
        battlelog.enemySTATtext.gameObject.SetActive(false);
        battlelog.Portraitparent.gameObject.SetActive(false);
        battlelog.characterName.gameObject.SetActive(false);
        battlelog.enemyPortraitparent.gameObject.SetActive(false);

        battlelog.itemText.gameObject.SetActive(false);
        battlelog.itemText.text = "";
        foreach (var item in battlelog.inventoryDisplay.GetComponentsInChildren<Button>())
        {
            Destroy(item.gameObject);
        }
    }

    public void Move(bool moveUp)
    {

        if (moveUp)
        {
            if (generalCoroutine != null)
                StopCoroutine(generalCoroutine);

            generalCoroutine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), 960, -950, 0.01f);
            StartCoroutine(generalCoroutine);
        }
        else
        {
            if (generalCoroutine != null)
                StopCoroutine(generalCoroutine);

            generalCoroutine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), 960, -1215, 0.01f);
            StartCoroutine(generalCoroutine);
        }

    }
    public static void DisplayEnemyIntentInfo(string description)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.enemyIntent.gameObject.SetActive(true);
        battlelog.enemyIntent.text = ($"{description}");
    }


    public static void SetBattleText(string text)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.DoBattleText(text);
    }

    public void DoBattleText(string text)
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        //BattleLog.ClearAllBattleLogText();
        battlelog.ambientText.gameObject.SetActive(false);
        battleText.gameObject.SetActive(true);
        battleText.text = text;
    }

    public static void ClearBattleText()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.battleText.gameObject.SetActive(false);
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

    public static void ClearAllBattleLogText()
    {
        var battlelog = GameObject.FindObjectOfType<BattleLog>();
        battlelog.battleText.gameObject.SetActive(false);
        BattleLog.DisableCharacterStats();
        battlelog.ambientText.gameObject.SetActive(false);
        battlelog.enemyIntent.gameObject.SetActive(false);
    }

    public void DoPostBattleDialouge(Unit unit)
    {
        BattleLog.CharacterDialog(ConvserationHandler.DustyAureliaPostMeeting, false, false);
        unit.EnteredMap -= DoPostBattleDialouge;
    }

    private IEnumerator TypeMultiText(List<LabLine> text, TMP_Text x, bool disableAfter, bool Pauses = false)
    {
        BattleStates previousState = BattleStates.IDLE;
        Move(true);
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
                }
                LabCamera.Instance.ResetPosition();
                LabCamera.Instance.uicam.gameObject.SetActive(false);
                Tools.PauseAllStaminaTimers();
                print("BATTLE SHOULD BE PAUSED");
                BattleSystem.Instance.state = BattleStates.TALKING;
                LabCamera.Instance.state = LabCamera.CameraState.IDLE;
            }
            else if (RestSite.Instance != null)
            {
                foreach (var button in RestSite.Instance.buttons)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < text.Count; i++)
        {
            charPortrait.sprite = Director.Instance.Unitdatabase.Where(obj => obj.name == text[i].unit).SingleOrDefault().charPortraits.Find(obj => obj.name == text[i].expression);
            foreach (char letter in text[i].text.ToCharArray())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    x.text = text[i].text;
                    print("SKIPPED");
                    break;
                }
                else
                {
                    x.text += letter;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        x.text = text[i].text;
                        print("SKIPPED");
                        break;
                    }
                    yield return new WaitForSeconds(text[i].textSpeed);
                }
            }
            indicator.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.01f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            characterdialog.text = "";
            indicator.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.01f);
        }
        if (disableAfter)
        {
            x.text = "";
        }
        if (Pauses)
        {
            if (BattleSystem.Instance != null)
            {
                Tools.UnpauseAllStaminaTimers();
                BattleLog.SetRandomAmbientTextActive();
                BattleSystem.Instance.state = previousState;
                LabCamera.Instance.state = LabCamera.CameraState.SWAY;
                BattleLog.DisableCharacterDialog();

                foreach (var unit in Tools.GetAllUnits())
                {
                    unit.state = PlayerState.IDLE;
                    unit.StaminaHighlightIsDisabled = false;
                    LabCamera.Instance.state = LabCamera.CameraState.SWAY;
                    LabCamera.Instance.uicam.gameObject.SetActive(true);
                }
            }
            if (RestSite.Instance != null)
            {
                foreach (var button in RestSite.Instance.buttons)
                {
                    button.gameObject.SetActive(true);
                }
                Move(false);
            }
        }
        else
        {
            Move(false);
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
