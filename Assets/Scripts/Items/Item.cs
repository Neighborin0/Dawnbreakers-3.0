using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting.FullSerializer;

public class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite sprite;

    public bool CanBeTransfered = true;


    public event Action<Item> OnPickedUp;
    public event Action<Item> Removed;

    public virtual void OnPickup(Unit unit) 
    {
        OnPickedUp?.Invoke(this);  
    }

    public void RemoveFromInventory(Unit unit)
    {
        unit.inventory.Remove(this);
    }
    public virtual void OnRemoved(Unit unit)
    {
        Removed?.Invoke(this);
    }
}
