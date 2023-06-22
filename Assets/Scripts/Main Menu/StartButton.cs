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
        OptionsManager.Instance.Load("MAP2");
    }

}
