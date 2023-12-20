using Autodesk.Fbx;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDrop : MonoBehaviour
{
    public Image LevelDropImage;
    public TextMeshProUGUI SubText;
    public bool Done = false;
    public Image bar;

    public IEnumerator DoOpening()
    {
        if (gameObject != null)
        {
            //Inital Fade In
            LevelDropImage.gameObject.SetActive(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0);
            LevelDropImage.material = Instantiate<Material>(LevelDropImage.material);
            var LevelDropMat = LevelDropImage.material;
            while (LevelDropMat.GetColor("_BaseColor").a < 1)
            {
                LevelDropMat.SetColor("_BaseColor", new Color(LevelDropMat.GetColor("_BaseColor").r, LevelDropMat.GetColor("_BaseColor").g, LevelDropMat.GetColor("_BaseColor").b, LevelDropMat.GetColor("_BaseColor").a + 0.1f));
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitUntil(() => LevelDropMat.GetColor("_BaseColor").a >= 1);
            yield return new WaitForSeconds(0.5f);
            while (SubText.color.a < 1)
            {
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a + 0.1f);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitUntil(() => SubText.color.a >= 1);

            yield return new WaitForSeconds(3f);
            //Fade Out
            while (LevelDropMat.GetColor("_BaseColor").a > 0)
            {
                LevelDropMat.SetColor("_BaseColor", new Color(LevelDropMat.GetColor("_BaseColor").r, LevelDropMat.GetColor("_BaseColor").g, LevelDropMat.GetColor("_BaseColor").b, LevelDropMat.GetColor("_BaseColor").a - 0.1f));
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a - 0.1f);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitUntil(() => LevelDropMat.GetColor("_BaseColor").a <= 1);

            StartCoroutine(Tools.FadeObject(bar, 0.001f, false));
           
            yield return new WaitUntil(() => bar.color.a <= 0);
            Done = true;
        }
    }
}

