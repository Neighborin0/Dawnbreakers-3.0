using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public Image blackScreen;
    public void EnterGame()
    {
        StartCoroutine(LoadGame("MAP2"));
    }

    public IEnumerator LoadGame(string SceneToLoad)
    {
        StartCoroutine(Tools.FadeObject(blackScreen, 0.001f, true));
        yield return new WaitUntil(() => blackScreen.color == new Color(0, 0, 0, 1));
        yield return new WaitForSeconds(1f);
        print("TRANSITIONED");
        SceneManager.LoadScene(SceneToLoad);

    }
}
