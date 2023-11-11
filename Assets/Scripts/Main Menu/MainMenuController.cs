using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    void Start()
    {

#if UNITY_EDITOR
           
#else
            OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
            OptionsManager.Instance.blackScreen.color = new Color(0, 0, 0, 1);
#endif
            StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
    }

    

}
