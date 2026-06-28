using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CharacterTab : MonoBehaviour, IDropHandler
{
    //level up
    //public GameObject levelupDisplay;
    //public List<Button> LevelUpButtons;
    //public List<Button> LevelDownbuttons;
    public Unit unit;
    public Image portrait;
    public int skillPoints = 1;
    public TextMeshProUGUI skillPointText;

    public TextMeshProUGUI skillWEAKText;
    public TextMeshProUGUI skillATKText;
    public TextMeshProUGUI skillDEFText;
    public TextMeshProUGUI skillRESText;

    //detailed display
    public GameObject detailedDisplay;
    public GameObject statText;
    public TextMeshProUGUI REStext;
    public TextMeshProUGUI ATKtext;
    public TextMeshProUGUI DEFText;
    public TextMeshProUGUI WEAKText;
    public Button DisplaySwitcher;
    public GameObject detailedAction;
    public GridLayoutGroup detailedstatDisplay;
    public GridLayoutGroup inventoryDisplay;
    public GridLayoutGroup actionDisplay;
    public Button characterTransfer;
    public Sprite actionIcon;
    public Image notification;
    //public Sprite LevelUpIcon;
    public Sprite itemIcon;
    public CharacterTabPopup popup;

    public List<string> resistanceText;
    public List<string> weaknessText;


    public event Action<CharacterTab> OnInteracted;

    private void Start()
    {
        Image image = GetComponent<Image>();

        if (image != null && image.material != null)
        {
            image.material = Instantiate(image.material);
        }

        if (DisplaySwitcher != null &&
            DisplaySwitcher.image != null &&
            DisplaySwitcher.image.material != null)
        {
            DisplaySwitcher.image.material =
                Instantiate(DisplaySwitcher.image.material);

            DisplaySwitcher.image.material.SetFloat(
                "OutlineThickness",
                0f
            );

            DisplaySwitcher.image.sprite = itemIcon;
        }
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
            CT.REStext.text = $"RES:  {string.Join("", CT.resistanceText.ToArray())}";
            CT.WEAKText.text = $"WEAK: {string.Join("", CT.weaknessText.ToArray())}";
            GetComponent<HighlightedObject>().disabled = false;
        }
        foreach (var CS in Director.Instance.characterSlotpos.transform.GetComponentsInChildren<CharacterSlot>())
        {
            CS.ResetStats();
        }
        skillRESText.text = $"RES:  {string.Join("", resistanceText.ToArray())}";
        skillATKText.text = $"ATK:{unit.attackStat}";
        skillDEFText.text = $"DEF:{unit.defenseStat}";
        skillWEAKText.text = $"WEAK:   {string.Join("", weaknessText.ToArray())}";

    }

    public void SwitchDetailedStates()
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

    public void Init(
    Unit targetUnit,
    bool levelUp,
    bool interactable = false)
    {
        if (targetUnit == null)
        {
            Debug.LogError(
                "CharacterTab initialized with null unit.",
                this
            );

            return;
        }

        unit = targetUnit;

        resistanceText.Clear();
        weaknessText.Clear();

        foreach (Transform child in actionDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in inventoryDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        if (unit.resistances != null)
        {
            for (int i = 0; i < unit.resistances.Length; i++)
            {
                resistanceText.Add(
                    $"<sprite name=\"{unit.resistances[i]}\">"
                );
            }
        }

        if (unit.weaknesses != null)
        {
            for (int i = 0; i < unit.weaknesses.Length; i++)
            {
                weaknessText.Add(
                    $"<sprite name=\"{unit.weaknesses[i]}\">"
                );
            }
        }

        string resistanceString =
            string.Join("", resistanceText.ToArray());

        string weaknessString =
            string.Join("", weaknessText.ToArray());

        DEFText.text = $"DEF: {unit.defenseStat}";
        ATKtext.text = $"ATK: {unit.attackStat}";
        REStext.text = $"RES:  {resistanceString}";
        WEAKText.text = $"WEAK:   {weaknessString}";

        skillDEFText.text = $"DEF: {unit.defenseStat}";
        skillATKText.text = $"ATK: {unit.attackStat}";
        skillRESText.text = $"RES:  {resistanceString}";
        skillWEAKText.text = $"WEAK:   {weaknessString}";

        portrait.sprite = unit.charPortraits[0];

        actionDisplay.gameObject.SetActive(true);
        SetUpActionList(unit);

        inventoryDisplay.gameObject.SetActive(true);

        foreach (Item item in unit.inventory)
        {
            if (item == null)
                continue;

            
            Button itemButton = Instantiate(BattleLog.Instance.itemImage, inventoryDisplay.transform, false);
            ItemText itemText = itemButton.GetComponent<ItemText>();
            itemButton.image.sprite = item.sprite;
            itemText.item = item;
            itemText.unit = unit;

            RectTransform itemRect =
                itemText.GetComponent<RectTransform>();

            if (itemRect != null)
            {
                itemRect.localScale = Vector3.one;
            }
        }

        detailedDisplay.SetActive(!levelUp);

        if (levelUp)
        {
            actionDisplay.gameObject.SetActive(false);
            inventoryDisplay.gameObject.SetActive(false);
        }
        else
        {
            actionDisplay.gameObject.SetActive(true);
            inventoryDisplay.gameObject.SetActive(false);
        }

        if (characterTransfer != null)
        {
            characterTransfer.interactable = interactable;
        }
    }

    public void SetUpActionList(Unit unit)
    {
        foreach (var action in unit.actionList)
        {
            var actionContainer = Instantiate(detailedAction);
            actionContainer.transform.SetParent(actionDisplay.transform);
            actionContainer.transform.localScale = new Vector3(1, 1, 1);

            if (BattleSystem.Instance != null)
            {
                //Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                actionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(actionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D.x, actionContainer.transform.GetComponent<RectTransform>().anchoredPosition3D.y, 1);
                actionDisplay.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(actionDisplay.transform.GetComponent<RectTransform>().anchoredPosition3D.x, actionDisplay.transform.GetComponent<RectTransform>().anchoredPosition3D.y, 1);
            }


            var assignedAction = actionContainer.GetComponent<ActionContainer>();
            assignedAction.targetting = false;
            assignedAction.baseUnit = unit;
            assignedAction.button.interactable = true;
            assignedAction.button.enabled = true;
            assignedAction.action = action;
            assignedAction.damageNums.text = $"<sprite name=\"{action.damageType}\">" + (CombatTools.DetermineTrueActionValue(action) + unit.attackStat).ToString();
            assignedAction.durationNums.text = "<sprite name=\"Duration\">" + (action.duration).ToString();
            assignedAction.costNums.text = CombatTools.DetermineTrueCost(action) < 100 ? $"{CombatTools.DetermineTrueCost(action)}%" : $"100%";
            assignedAction.costNums.color = Color.yellow;
            assignedAction.textMesh.text = action.ActionName;
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
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        if (obj.GetComponent<ItemText>() != null)
        {
            var item = obj.GetComponent<ItemText>();
            if (item.item.CanBeTransfered)
            {
                print(item.item.itemName + " owner:" + item.unit.unitName);
                this.TransferItem(item);
            }

        }
    }
}
