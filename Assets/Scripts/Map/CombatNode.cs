using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatNode : MapNode
{
    public List<Unit> enemies;
    public List<Unit> playerUnits;
    public void StartBattle()
    {
        DisableNode();
        Tools.ClearAllCharacterSlots();
        foreach (var unit in Director.Instance.party)
        {
            playerUnits.Add(unit);
           
            print(unit.unitName);
        }
        //SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
        StartCoroutine(Director.Instance.DoLoad("Battle"));
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var battlesystem = BattleSystem.Instance;
        battlesystem.playerUnits = playerUnits;
        battlesystem.enemiesToLoad = enemies;
        print(battlesystem.enemyUnits.Count);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}