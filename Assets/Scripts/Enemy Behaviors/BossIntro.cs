
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossIntro : MonoBehaviour
{
    public Image BossTitleCard;
    public bool Done = false;
    public TextMeshProUGUI SubText;


    public IEnumerator DoIntro()
    {
        if (gameObject != null)
        {
            SubText.fontSharedMaterial = Instantiate<Material>(SubText.fontSharedMaterial);
            SubText.fontSharedMaterial.SetColor("_FaceColor", new Color(191, 167, 83, 0));
            AudioManager.QuickPlay("map_opening_001");
            AudioManager.Instance.Play("Coronus_Boss", 0);
            Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0.8f, "Coronus_Boss", 0.5f, false));
            LabCamera.Instance.uicam.gameObject.SetActive(true);
            Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            LabCamera.Instance.state = LabCamera.CameraState.IDLE;

            Director.Instance.BossCircle.gameObject.SetActive(true);
            OptionsManager.Instance.StartCoroutine(Tools.SmoothScale(Director.Instance.BossCircle.transform.GetComponent<RectTransform>(), new Vector3 (100, 100, 100), 0.1f));
            yield return new WaitUntil(() => Director.Instance.BossCircle.transform.localScale.x >= 100);
            //Inital Fade In
            BossTitleCard.gameObject.SetActive(true);

            Director.Instance.blackScreen.gameObject.SetActive(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 1);
            Director.Instance.BossCircle.gameObject.SetActive(false);
            SubText.text = "High Priest of Cursed Radiance";

            BossTitleCard.material = Instantiate<Material>(BossTitleCard.material);


            var BossTitleCardMat = BossTitleCard.material;

            Color TitleColor = new Color32(255, 255, 255, 0);
            BossTitleCardMat.SetColor("_BaseColor", new Color(TitleColor.r, TitleColor.g, TitleColor.b, 0));

            while (BossTitleCardMat.GetColor("_BaseColor").a < 1)
            {
                BossTitleCardMat.SetColor("_BaseColor", new Color(TitleColor.r, TitleColor.g, TitleColor.b, BossTitleCardMat.GetColor("_BaseColor").a + 0.05f));
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => BossTitleCardMat.GetColor("_BaseColor").a >= 1);
            yield return new WaitForSeconds(1f);
            SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, 0);
            SubText.gameObject.SetActive(true);
            Color color = new Color32(255, 222, 180, 0);
            SubText.fontSharedMaterial.SetColor("_FaceColor", new Color(color.r, color.g, color.b, 255) * 1.00001f);
            SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, 0);
            SubText.ForceMeshUpdate();

            while (SubText.color.a < 1)
            {
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a + 0.05f);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => SubText.color.a >= 1);
            yield return new WaitForSeconds(7f);
            //Fade Out
            while (BossTitleCardMat.GetColor("_BaseColor").a > 0)
            {
                BossTitleCardMat.SetColor("_BaseColor", new Color(BossTitleCardMat.GetColor("_BaseColor").r, BossTitleCardMat.GetColor("_BaseColor").g, BossTitleCardMat.GetColor("_BaseColor").b, BossTitleCardMat.GetColor("_BaseColor").a - 0.05f));
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a - 0.05f);
                SubText.fontSharedMaterial.SetColor("_FaceColor", new Color(color.r, color.g, color.b, SubText.fontSharedMaterial.GetColor("_FaceColor").a - 0.05f));
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => BossTitleCardMat.GetColor("_BaseColor").a <= 1);

            StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, false));
           
            yield return new WaitUntil(() => Director.Instance.blackScreen.color.a <= 0);
            yield return new WaitForSeconds(0.01f);

            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
            BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
            BattleSystem.Instance.playerUnits[0].StartDecision();
            LabCamera.Instance.uicam.gameObject.SetActive(true);
            Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            foreach(var unit in Tools.GetAllUnits())
            {
                if(!unit.IsPlayerControlled)
                {
                    unit.intentUI.gameObject.SetActive(true);
                }
            }
        }
    }
}

