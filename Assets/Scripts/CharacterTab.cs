using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;
using System.Linq;

public class CharacterTab : MonoBehaviour, IDropHandler
{
    //level up
    public GameObject levelupDisplay;
    public List<Button> LevelUpButtons;
    public List<Button> LevelDownbuttons;
    public Unit unit;
    public Image portrait;
    public int skillPoints = 1;
    public TextMeshProUGUI skillPointText;

    //detailed display
    public GameObject detailedDisplay;
    public GameObject statText;
    public TextMeshProUGUI HPtext;
    public TextMeshProUGUI ATKtext;
    public TextMeshProUGUI DEFText;
    public TextMeshProUGUI SPDText;
    public Button DisplaySwitcher;
    public GameObject detailedAction;
    public GridLayoutGroup detailedstatDisplay;
    public GridLayoutGroup inventoryDisplay;
    public GridLayoutGroup actionDisplay;
    public Button characterTransfer;
    public Sprite actionIcon;
    public Image notification;
    public Sprite LevelUpIcon;
    public Sprite itemIcon;
    public CharacterTabPopup popup;

    public event Action<CharacterTab> OnInteracted;



    [SerializeField] public TextMeshProUGUI statDisplay;
    void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        DisplaySwitcher.GetComponent<Image>().material = Instantiate<Material>(DisplaySwitcher.GetComponent<Image>().material);
        DisplaySwitcher.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
        if (unit != null)
        {
            if (unit.charPortraits != null)
            {
                portrait.sprite = unit.charPortraits.Find(obj => obj.name == "neutral");
            }
            this.inventoryDisplay.GetComponent<InventoryDisplay>().unit = unit;
        }
        if (BattleSystem.Instance != null)
        {
            DisplaySwitcher.image.sprite = actionIcon;
            foreach (var action in unit.actionList)
            {
                if(action.New == true)
                {
                    DisplaySwitcher.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
                    DisplaySwitcher.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                }
            }
            levelupDisplay.SetActive(true);
        }
        if (OptionsManager.Instance != null)
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        Tools.ToggleUiBlocker(false, true);
    }

    public void DoOnInteracted()
    {
        OnInteracted?.Invoke(this);
    }
    public void TransferItem(ItemText item)
    {
        var dragObj = item.gameObject.GetComponent<DraggableObject>();
        dragObj.originalParent = this.inventoryDisplay.transform;
        item.item.RemoveFromInventory(item.unit);
        item.item.OnRemoved(item.unit);
        item.item.OnPickup(this.unit);
        this.unit.inventory.Add(item.item);
        item.unit = this.unit;
        foreach (var CT in Director.Instance.TabGrid.transform.GetComponentsInChildren<CharacterTab>())
        {
            CT.DEFText.text = $"DEF:{CT.unit.defenseStat}";
            CT.ATKtext.text = $"ATK: {CT.unit.attackStat}";
            CT.HPtext.text = $"HP: {CT.unit.maxHP}";
            CT.SPDText.text = $"SPD: {CT.unit.speedStat}";
            GetComponent<HighlightedObject>().disabled = false;
        }
        foreach (var CS in Director.Instance.characterSlotpos.transform.GetComponentsInChildren<CharacterSlot>())
        {
            CS.ResetStats();
        }

    }

    public void IncreaseStat()
    {
        Director.Instance.characterTab.transform.transform.SetAsFirstSibling();
        skillPoints -= 1;
        if (EventSystem.current.currentSelectedGameObject == LevelUpButtons[0].gameObject)
        {
            unit.maxHP += 2;
            unit.currentHP += 2;
            statDisplay.text = $"HP:<color=#00FF00>{unit.maxHP}</color> +2\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
            LevelDownbuttons[0].gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == LevelUpButtons[1].gameObject)
        {
            unit.attackStat += 1;
            statDisplay.text = $"HP:{unit.maxHP}\nATK:<color=#00FF00>{unit.attackStat}</color> +1\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
            LevelDownbuttons[1].gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == LevelUpButtons[2].gameObject)
        {
            unit.defenseStat += 1;
            statDisplay.text = $"HP:{unit.maxHP}\nATK:{unit.attackStat}\nDEF:<color=#00FF00>{unit.defenseStat}</color> +1\nSPD:{unit.speedStat}";
            LevelDownbuttons[2].gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == LevelUpButtons[3].gameObject)
        {
            unit.speedStat += 1;
            statDisplay.text = $"HP:{unit.maxHP}\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:<color=#00FF00>{unit.speedStat}</color> +1";
            LevelDownbuttons[3].gameObject.SetActive(true);
        }
        foreach(var x in LevelUpButtons)
        {
            x.gameObject.SetActive(false);
        }
        skillPointText.text = $"Stat Points:{skillPoints}";
        CheckNumberOfSkillPointsInAllTabs();
    }

    private void CheckNumberOfSkillPointsInAllTabs()
    {
       bool allSame = Director.Instance.TabGrid.GetComponentsInChildren<CharacterTab>().All(item => item.skillPoints == 0);
        if(allSame)
        {
            Director.Instance.ConfirmButton.GetComponent<Button>().interactable = true;
            Director.Instance.ConfirmButton.GetComponent<Image>().material = Instantiate<Material>(Director.Instance.ConfirmButton.GetComponent<Image>().material);
            Director.Instance.ConfirmButton.GetComponent<Image>().material.SetFloat("OutlineThickness", 1);
            Director.Instance.ConfirmButton.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);

        }
        else
        {
            Director.Instance.ConfirmButton.GetComponent<Button>().interactable = false;
            Director.Instance.ConfirmButton.GetComponent<Image>().material = Instantiate<Material>(Director.Instance.ConfirmButton.GetComponent<Image>().material);
            Director.Instance.ConfirmButton.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
            Director.Instance.ConfirmButton.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
        }
    }

    public void DecreaseStat()
    {
        Director.Instance.characterTab.transform.transform.SetAsFirstSibling();
        skillPoints += 1;
        if (EventSystem.current.currentSelectedGameObject == LevelDownbuttons[0].gameObject)
        {
            unit.maxHP -= 2;
            unit.currentHP -= 2;
        }
        else if (EventSystem.current.currentSelectedGameObject == LevelDownbuttons[1].gameObject)
        {
            unit.attackStat -= 1;
        }
        else if (EventSystem.current.currentSelectedGameObject == LevelDownbuttons[2].gameObject)
        {
            unit.defenseStat -= 1;
        }
        else if (EventSystem.current.currentSelectedGameObject == LevelDownbuttons[3].gameObject)
        {
            unit.speedStat -= 1;
        }
        statDisplay.text = $"HP:{unit.maxHP}\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
        skillPointText.text = $"Stat Points:{skillPoints}";
        foreach (var x in LevelUpButtons)
        {
            x.gameObject.SetActive(true);
        }
        foreach (var x in LevelDownbuttons)
        {
            x.gameObject.SetActive(false);
        }
        CheckNumberOfSkillPointsInAllTabs();
    }
     public void SwitchDetailedStates()
    {

        if (BattleSystem.Instance == null)
        {
            if (inventoryDisplay.isActiveAndEnabled)
            {
                DisplaySwitcher.image.sprite = itemIcon;
                inventoryDisplay.gameObject.SetActive(false);
                actionDisplay.gameObject.SetActive(true);
                statText.gameObject.SetActive(true);
            }
            else
            {
                DisplaySwitcher.image.sprite = actionIcon;
                inventoryDisplay.gameObject.SetActive(true);
                actionDisplay.gameObject.SetActive(false);
            }
        }
        else
        {
            if (DisplaySwitcher.GetComponent<Image>().material.GetFloat("OutlineThickness") != 0)
            {
                DisplaySwitcher.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
            }
            if (levelupDisplay.activeInHierarchy)
            {
                DisplaySwitcher.image.sprite = LevelUpIcon;
                levelupDisplay.SetActive(false);
                actionDisplay.gameObject.SetActive(true);
                statText.gameObject.SetActive(true);
            }
            else
            {
                DisplaySwitcher.image.sprite = actionIcon;
                levelupDisplay.SetActive(true);
                actionDisplay.gameObject.SetActive(false);
                foreach(var action in unit.actionList)
                {
                    action.New = false;
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        if (obj.GetComponent<ItemText>() != null)
        {
            var item = obj.GetComponent<ItemText>();
            if(item.item.CanBeTransfered) 
            {
                print(item.item.itemName + " owner:" + item.unit.unitName);
                this.TransferItem(item);
            }
          
        }
    }
}
