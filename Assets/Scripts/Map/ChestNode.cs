using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestNode : MapNode
{
    public ItemDisplay itemDisplayPrefab;
    public List<ItemDisplay> itemDisplays;
    public List<Item> alreadySelected;
    public override void OnInteracted()
    {
        Tools.ToggleUiBlocker(false, true);
        Director.Instance.CharacterSlotEnable(true);
        for (int i = 0; i <= 2; i++)
        {
            var ID = Instantiate(itemDisplayPrefab, Director.Instance.ItemTabGrid.transform);
            itemDisplays.Add(ID);
        }
        GetRandomItem();
      
    }
    public void GetRandomItem()
    {
        for (int i = 0; i <= 2; i++)
        {
            var chosenItem = SelectRandom();
            while (chosenItem == null)
            {
                chosenItem = SelectRandom();
            }
            itemDisplays[i].item = chosenItem;
            itemDisplays[i].TransitionIn();
        }
    }

    private Item SelectRandom()
    {
        var chosenItem = Director.Instance.itemDatabase[UnityEngine.Random.Range(0, Director.Instance.itemDatabase.Count)];
        if(alreadySelected.Contains(chosenItem))
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
