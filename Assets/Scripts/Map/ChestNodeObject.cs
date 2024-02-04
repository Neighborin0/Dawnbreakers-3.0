using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestNodeObject : MonoBehaviour
{
    public List<ItemDisplay> itemDisplays;
    private List<Item> alreadySelected;

    private void Start()
    {
        GetRandomItem();
    }
    public void GetRandomItem()
    {
        alreadySelected.Clear();
        for (int i = 0; i <= 3; i++)
        {
            var chosenItem = SelectRandom();
            while (chosenItem == null)
            {
                chosenItem = SelectRandom();
            }
            itemDisplays[i].item = chosenItem;
            

        }
    }
   
    private Item SelectRandom()
    {
        var chosenItem = Director.Instance.itemDatabase[UnityEngine.Random.Range(0, Director.Instance.itemDatabase.Count)];
        foreach (var item in alreadySelected)
        {
            if (item.itemName == chosenItem.itemName)
            {
                SelectRandom();
                chosenItem = null;
            }
            else
            {
                alreadySelected.Add(chosenItem);
            }
        }
        return chosenItem;
    }
}
