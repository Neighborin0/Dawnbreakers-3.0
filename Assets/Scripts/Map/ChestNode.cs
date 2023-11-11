using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChestNode : MapNode
{
    public ItemDisplay itemDisplayPrefab;
    public List<ItemDisplay> itemDisplays;
    public List<Item> alreadySelected;
    public override void OnInteracted()
    {
        Tools.ToggleUiBlocker(false, true);
        Director.Instance.CharacterSlotEnable(true);
        BattleLog.Instance.ClearAllBattleLogText();
        for (int i = 0; i <= 2; i++)
        {
            var ID = Instantiate(itemDisplayPrefab, new Vector2(0, -1000), Quaternion.identity, Director.Instance.canvas.transform);
            ID.GetComponent<RectTransform>().anchoredPosition = new Vector2(-550 + (i * 520), -1000);
            ID.GetComponent<MoveableObject>().PositionDownX = -550 + (i * 520);
            ID.GetComponent<MoveableObject>().PositionUpX = -550 + (i * 520);
            itemDisplays.Add(ID);
        }
        GetRandomItem();

    }
    public void GetRandomItem()
    {
        Director.Instance.chooseYourItemText.gameObject.SetActive(true);
        for (int i = 0; i <= 2; i++)
        {
            var chosenItem = SelectRandom();
            while (chosenItem == null)
            {
                chosenItem = SelectRandom();
            }
            itemDisplays[i].item = chosenItem;
            itemDisplays[i].TransitionIn();
            itemDisplays[i].GetComponent<Button>().interactable = false;
        }
        StartCoroutine(MoveItemDisplays());
    }

    private IEnumerator MoveItemDisplays()
    {
        foreach(var id in FindObjectsOfType<ItemDisplay>())
        {
            id.GetComponent<MoveableObject>().Move(true);
            yield return new WaitForSeconds(0.5f);
        }
        foreach (var id in FindObjectsOfType<ItemDisplay>())
        {
            id.GetComponent<Button>().interactable = true;
            id.GetComponent<HighlightedObject>().disabled = false;
        }
    }
    private Item SelectRandom()
    {
        var chosenItem = Director.Instance.itemDatabase[UnityEngine.Random.Range(0, Director.Instance.itemDatabase.Count)];
        if(alreadySelected.Contains(chosenItem) || chosenItem.ExcludedFromLootPools)
        {
            chosenItem = null;
        }
        else
        {
            alreadySelected.Add(chosenItem);
        }
        return chosenItem;
    }
}
