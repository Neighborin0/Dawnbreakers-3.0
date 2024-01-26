
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDrop : MonoBehaviour
{
   // public Image LevelDropImage;
    public TextMeshProUGUI MainText;
    public TextMeshProUGUI SubText;
    public bool Done = false;
    public Image bar;

    public void Awake()
    {
        if (Director.Instance.DevMode)
            Destroy(this.gameObject);
    }
    public IEnumerator DoOpening()
    {
        if (gameObject != null)
        {
            //Inital Fade In
            AudioManager.QuickPlay("map_opening_001");


            MainText.gameObject.SetActive(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0);
            // LevelDropImage.material = Instantiate<Material>(LevelDropImage.material);
            //var LevelDropMat = LevelDropImage.material;
            while (MainText.color.a < 1)
            {
                MainText.color = new Color(MainText.color.r, MainText.color.g, MainText.color.b, MainText.color.a + 0.05f);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => MainText.color.a >= 1);
            yield return new WaitForSeconds(0.5f);
            while (SubText.color.a < 1)
            {
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a + 0.05f);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => SubText.color.a >= 1);

            yield return new WaitForSeconds(3f);
            //Fade Out
            while (MainText.color.a > 0)
            {
                MainText.color = new Color(MainText.color.r, MainText.color.g, MainText.color.b, MainText.color.a - 0.05f);
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a - 0.05f);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => MainText.color.a <= 1);

            StartCoroutine(Tools.FadeObject(bar, 0.001f, false));
           
            yield return new WaitUntil(() => bar.color.a <= 0);
            Done = true;
        }
    }
}

