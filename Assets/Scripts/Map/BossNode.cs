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

    
    public override void OnInteracted()
    {
        Tools.ClearAllCharacterSlots();
        foreach (var unit in Director.Instance.party)
        {
            playerUnits.Add(unit);
           
            print(unit.unitName);
        }
        OptionsManager.Instance.Load("Battle", null, 1000f);
      
        SceneManager.sceneLoaded += OnSceneLoaded;  
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, AudioManager.Instance.currentMusicTrack, 0.1f, false));
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
