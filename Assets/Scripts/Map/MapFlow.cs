using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static MapFlow;

public struct LabNode
{
    public RoomType RoomType;
    public string[] enemies;
}
public class MapFlow : MonoBehaviour
{
    public enum RoomType { COMBAT, CHEST, REST }


    public static List<LabNode> TestFlow = new List<LabNode>
    {
        new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "Tutorial_Creature"} },
        new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "DustyEnemy"} },
        new LabNode{RoomType = RoomType.REST },
         new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "Vermin", "Husk"} },
        new LabNode{RoomType = RoomType.CHEST },
         new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "Vermin", "Vermin", "Vermin" } },
         new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "Matriarch", "Husk", "Husk", } },
    };
}
