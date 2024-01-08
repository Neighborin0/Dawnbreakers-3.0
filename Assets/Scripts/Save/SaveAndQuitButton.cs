using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveAndQuitButton : MonoBehaviour
{
    public void SaveAndQuit()
    {
        OptionsManager.Instance.Load("Main Menu", "Main Menu Theme");
        if(Director.Instance != null)
        {
            SceneManager.MoveGameObjectToScene(Director.Instance.gameObject, SceneManager.GetActiveScene());
        }
        if (MapController.Instance != null)
        {
            SceneManager.MoveGameObjectToScene(MapController.Instance.gameObject, SceneManager.GetActiveScene());
        }
        /*if(RunTracker.Instance != null)
        {
            Destroy(RunTracker.Instance.gameObject);
        }
        */
        //SaveGame();
    }

    /*public void SaveGame()
    {
        PlayerPrefs.SetString("Level", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }
    */
}

