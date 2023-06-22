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
        OptionsManager.Instance.Load("Main Menu");
        if(Director.Instance != null)
        {
            SceneManager.MoveGameObjectToScene(Director.Instance.gameObject, SceneManager.GetActiveScene());
        }
        if (MapController.Instance != null)
        {
            SceneManager.MoveGameObjectToScene(MapController.Instance.gameObject, SceneManager.GetActiveScene());
        }
    }

}

