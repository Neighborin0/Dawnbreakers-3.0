﻿using System.Collections;
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
    public List<Button> buttons;
    public Unit unit;
    public Image portrait;

    //detailed display
    public GameObject detailedDisplay;
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
    public bool IsHighlighted = true;
    public Vector3 oldScaleSize;
    public Vector3 newScaleSize = new Vector3(1000, 1000, 1000);
    private IEnumerator scaler;

    public event Action<CharacterTab> OnInteracted;



    [SerializeField] public TextMeshProUGUI statDisplay;
    void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        oldScaleSize = transform.localScale;
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
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
                if(action.New)
                {
                    notification.gameObject.SetActive(true);
                }
            }
            levelupDisplay.SetActive(true);
        }
    }

  

     void Update()
    {
        if(detailedDisplay != null && Input.GetKeyDown(KeyCode.E))
        {
            Director.Instance.DisableCharacterTab();
        }

    }



    public IEnumerator LevelUpText()
    {
        statDisplay.text = $"HP: {unit.maxHP} +2\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
        yield return new WaitForSeconds(0.5f);
        statDisplay.text = $"HP: {unit.maxHP} +2\nATK:{unit.attackStat} +1\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
        yield return new WaitForSeconds(0.5f);
        statDisplay.text = $"HP: {unit.maxHP} +2\nATK:{unit.attackStat} +1\nDEF:{unit.defenseStat} +1\nSPD:{unit.speedStat}";
        yield return new WaitForSeconds(0.5f);
        statDisplay.text = $"HP: {unit.maxHP} +2\nATK:{unit.attackStat} +1\nDEF:{unit.defenseStat} +1\nSPD:{unit.speedStat} +1";
        yield return new WaitForSeconds(1.5f);
        statDisplay.text = $"HP: {unit.maxHP}\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
        yield break;
    }
    public void DoOnInteracted()
    {
        OnInteracted?.Invoke(this);
    }
    public void TransferItem(ItemText item)
    {
        var dragObj = item.gameObject.GetComponent<DraggableObject>();
        dragObj.originalParent = this.inventoryDisplay.transform;
        //var ID = this.inventoryDisplay.GetComponent<InventoryDisplay>();
        item.item.RemoveFromInventory(item.unit);
        item.item.OnRemoved(item.unit);
        item.item.OnPickup(this.unit);
        this.unit.inventory.Add(item.item);
        item.unit = this.unit;
        foreach (var CT in Director.Instance.TabGrid.transform.GetComponentsInChildren<CharacterTab>())
        {
            CT.DEFText.text = $"DEF: {CT.unit.defenseStat}";
            CT.ATKtext.text = $"ATK: {CT.unit.attackStat}";
            CT.HPtext.text = $"HP: {CT.unit.maxHP}";
            CT.SPDText.text = $"SPD: {CT.unit.speedStat}";
        }
        foreach (var CS in Director.Instance.characterSlotpos.transform.GetComponentsInChildren<CharacterSlot>())
        {
            CS.stats.text = $":{CS.unit.attackStat}\n:{CS.unit.defenseStat}\n:{CS.unit.speedStat}";
            CS.healthNumbers.text = $"{CS.unit.currentHP} / {CS.unit.maxHP}";
        }

    }

    public void ToggleHighlight()
    {
        if (IsHighlighted && GetComponent<Button>().interactable)
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 3f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
            if (scaler != null)
            {
                StopCoroutine(scaler);
            }
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), newScaleSize, 0.01f);
            StartCoroutine(scaler);
            IsHighlighted = false;
        }
        else if(GetComponent<Button>().interactable)
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
            if (scaler != null)
            {
                StopCoroutine(scaler);
            }
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), oldScaleSize, 0.01f);
            StartCoroutine(scaler);
            IsHighlighted = true;
        }
    }
    public void IncreaseStat()
    {
        Director.LabyrinthLVL += 1;
        Director.Instance.characterTab.transform.transform.SetAsFirstSibling();
        if (EventSystem.current.currentSelectedGameObject == buttons[0].gameObject)
        {
            unit.maxHP += 3;
            unit.currentHP += 3;
            statDisplay.text = $"HP: {unit.maxHP} +3\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
        }
        else if (EventSystem.current.currentSelectedGameObject == buttons[1].gameObject)
        {
            unit.attackStat += 1;
            statDisplay.text = $"HP: {unit.maxHP}\nATK:{unit.attackStat} +1\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat}";
        }
        else if (EventSystem.current.currentSelectedGameObject == buttons[2].gameObject)
        {
            unit.defenseStat += 1;
            statDisplay.text = $"HP: {unit.maxHP}\nATK:{unit.attackStat} +1\nDEF:{unit.defenseStat} +1\nSPD:{unit.speedStat}";
        }
        else if (EventSystem.current.currentSelectedGameObject == buttons[3].gameObject)
        {
            unit.speedStat += 1;
            statDisplay.text = $"HP: {unit.maxHP}\nATK:{unit.attackStat}\nDEF:{unit.defenseStat}\nSPD:{unit.speedStat} +1";
        }
        foreach(var x in buttons)
        {
            x.interactable = false;
        }

        StartCoroutine(DelayedDestroy());
       
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
            notification.gameObject.SetActive(false);
            if (levelupDisplay.activeInHierarchy)
            {
                DisplaySwitcher.image.sprite = LevelUpIcon;
                levelupDisplay.SetActive(false);
                actionDisplay.gameObject.SetActive(true);        
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

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
        if (Director.Instance.TabGrid.transform.childCount <= 1)
        {
            BattleLog.Instance.Move(false);
            Director.Instance.StartCoroutine(Director.Instance.DoLoad("MAP2"));
            yield return new WaitUntil(() => Director.Instance.blackScreen.color == new Color(0, 0, 0, 1));
            foreach (var unit in Tools.GetAllUnits())
            {
                unit.StaminaHighlightIsDisabled = true;
                unit.gameObject.SetActive(false);
            }
            print("SHOULD BE TRANSITIONING");
        }
        Destroy(this.gameObject);
        yield break;
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