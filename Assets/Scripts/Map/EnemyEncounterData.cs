using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static MapFlow;

[CreateAssetMenu(fileName = "EnemyEncounterData", menuName = "Assets/EnemyEncounterData")]
public class EnemyEncounterData : ScriptableObject
{
    public Floor floorToSpawnOn = Floor.CORONUS;
    public EncounterDifficulty encounterDifficulty;
    public List<string> enemiesToSpawn;

  
}
