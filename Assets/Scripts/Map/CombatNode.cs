using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatNode : MapNode
{
    public List<Unit> enemies;
    public List<Unit> playerUnits;
    public override void OnInteracted()
    {
        Tools.ClearAllCharacterSlots();
        foreach (var unit in Director.Instance.party)
        {
            playerUnits.Add(unit);
           
        }
        AudioManager.QuickPlay("button_Hit_003", true);
        OptionsManager.Instance.Load("Battle", "Coronus_Battle", 0.5f, 0.25f);
        SceneManager.sceneLoaded += OnSceneLoaded;  
        RetrieveEnemyData();
    }

     private void RetrieveEnemyData()
    {
        var enemyEncounter = NodeController.Instance.viableEnemyEncounters[UnityEngine.Random.Range(0, NodeController.Instance.viableEnemyEncounters.Count)];
        foreach (var enemyNames in enemyEncounter.enemiesToSpawn)
        {
            enemies.Add(Director.Instance.Unitdatabase.Where(obj => obj.name == enemyNames).FirstOrDefault());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var battlesystem = BattleSystem.Instance;
        battlesystem.playerUnits = playerUnits;
        battlesystem.enemiesToLoad = enemies;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
