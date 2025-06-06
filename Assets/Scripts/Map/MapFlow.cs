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
    public enum RoomType { COMBAT, CHEST, REST, BOSS, TUTORIAL, EVENT, SHOP, ELITE }


    public static List<LabNode> TutorialFlow = new List<LabNode>
    {
        new LabNode{RoomType = RoomType.TUTORIAL, enemies = new string[]{ "Tutorial_Creature"} },
        new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "DustyEnemy"} },
          new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "Vermin"} },
          new LabNode{RoomType = RoomType.REST },
         new LabNode{RoomType = RoomType.BOSS, enemies = new string[]{ "Matriarch"} },
    };

    public static List<LabNode> DevFlow = new List<LabNode>
    {
        
           new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "TestDummy", "TestDummy", "TestDummy" } },
         new LabNode{RoomType = RoomType.TUTORIAL, enemies = new string[]{ "Tutorial_Creature"} },
        new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "DustyEnemy"} },
          new LabNode{RoomType = RoomType.COMBAT, enemies = new string[]{ "Vermin"} },
            new LabNode{RoomType = RoomType.REST },
         new LabNode{RoomType = RoomType.BOSS, enemies = new string[]{ "Matriarch"} },
    };
}
