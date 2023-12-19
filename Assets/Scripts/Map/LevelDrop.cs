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
    public List<GameObject> bars;

    public IEnumerator DoOpening()
    {
        if (gameObject != null)
        {
            //Inital Fade In
            LevelDropImage.gameObject.SetActive(true);
            LevelDropImage.material = Tools.ReturnMaterialCopy(LevelDropImage.gameObject);
            var LevelDropMat = LevelDropImage.material;
            while (LevelDropMat.GetColor("_BaseColor").a < 1)
            {
                LevelDropMat.SetColor("_BaseColor", new Color(LevelDropMat.GetColor("_BaseColor").r, LevelDropMat.GetColor("_BaseColor").g, LevelDropMat.GetColor("_BaseColor").b, LevelDropMat.GetColor("_BaseColor").a + 0.1f));
            }
            yield return new WaitUntil(() => LevelDropMat.GetColor("_BaseColor").a >= 1);

            while (SubText.color.a < 1)
            {
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a + 0.1f);
            }
            yield return new WaitUntil(() => SubText.color.a >= 1);

            yield return new WaitForSeconds(2f);
            //Fade Out
            while (LevelDropMat.GetColor("_BaseColor").a > 0)
            {
                LevelDropMat.SetColor("_BaseColor", new Color(LevelDropMat.GetColor("_BaseColor").r, LevelDropMat.GetColor("_BaseColor").g, LevelDropMat.GetColor("_BaseColor").b, LevelDropMat.GetColor("_BaseColor").a - 0.1f));
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a - 0.1f);
            }
            foreach(var b in bars)
            {
                b.GetComponent<MoveableObject>().Move(true);
            }
            yield return new WaitUntil(() => LevelDropMat.GetColor("_BaseColor").a <= 1);
            Done = true;
        }
    }
}

