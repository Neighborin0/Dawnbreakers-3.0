using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static MapFlow;

[CreateAssetMenu(fileName = "MapTemplate", menuName = "Assets/MapTemplate")]
public class MapTemplate : ScriptableObject
{
    public Floor floorToSpawnOn = Floor.CORONUS;
    public List<RoomType> roomsToSpawn;

  
}
