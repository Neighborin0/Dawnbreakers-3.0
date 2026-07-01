using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum Rarity { LVL_1, LVL_2, LVL_3 };

[System.Serializable]
public struct ActionPoolEntry
{
    public Action action;
    public Rarity rarity;

}

