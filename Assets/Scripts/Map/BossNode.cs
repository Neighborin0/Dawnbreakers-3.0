using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossNode : MapNode
{
    public List<Unit> enemies;
    public List<Unit> playerUnits;
    public Vector3 customSize;

    public void Start()
    {
        oldScaleSize = new Vector3(5, 5, 5);
        newScaleSize = new Vector3(6, 6, 6);
    }
    public override void OnInteracted()
    {
        Tools.ClearAllCharacterSlots();
        foreach (var unit in Director.Instance.party)
        {
            playerUnits.Add(unit);
           
            print(unit.unitName);
        }
        OptionsManager.Instance.Load("Battle");
        SceneManager.sceneLoaded += OnSceneLoaded;  
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var battlesystem = BattleSystem.Instance;
        battlesystem.BossNode = true;
        battlesystem.playerUnits = playerUnits;
        battlesystem.enemiesToLoad = enemies;
        print(battlesystem.enemyUnits.Count);
        if(customSize != Vector3.zero)
        {
            battlesystem.cameraPos3Units = customSize;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}