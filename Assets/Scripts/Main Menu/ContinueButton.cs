using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{

    public void LoadGame()
    {
        OptionsManager.Instance.Load(PlayerPrefs.GetString("Level"));
    }

}
