using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    public void EnterGame()
    {
        /* if (PlayerPrefs.GetString("Level") != null)
             PlayerPrefs.DeleteKey("Level");
        */
        //OptionsManager.Instance.Load("Battle");
        MapController.Instance.currentNodes[0].DisableNode();

    }

}
