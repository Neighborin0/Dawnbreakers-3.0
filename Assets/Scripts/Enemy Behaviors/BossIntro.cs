
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
    public TextMeshProUGUI SubText;
    public bool Done = false;

    public IEnumerator DoIntro()
    {
        if (gameObject != null)
        {
            LabCamera.Instance.uicam.gameObject.SetActive(true);
            Director.Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            LabCamera.Instance.state = LabCamera.CameraState.IDLE;
            Director.Instance.BossCircle.gameObject.SetActive(true);
            Director.Instance.StartCoroutine(Tools.SmoothScale(Director.Instance.BossCircle.transform.GetComponent<RectTransform>(), new Vector3 (10000, 10000, 10000), 0.01f));
            yield return new WaitUntil(() => Director.Instance.BossCircle.transform.localScale.x >= 10000);
            //Inital Fade In
            BossTitleCard.gameObject.SetActive(true);

            Director.Instance.blackScreen.gameObject.SetActive(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 1);
            Director.Instance.BossCircle.gameObject.SetActive(false);
           // SubText.text = "High Priest of Cursed Radiance";

            BossTitleCard.material = Instantiate<Material>(BossTitleCard.material);
            var BossTitleCardMat = BossTitleCard.material;
            while (BossTitleCardMat.GetColor("_BaseColor").a < 1)
            {
                BossTitleCardMat.SetColor("_BaseColor", 10 * new Color(BossTitleCardMat.GetColor("_BaseColor").r, BossTitleCardMat.GetColor("_BaseColor").g, BossTitleCardMat.GetColor("_BaseColor").b, BossTitleCardMat.GetColor("_BaseColor").a + 0.05f));
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => BossTitleCardMat.GetColor("_BaseColor").a >= 1);
            yield return new WaitForSeconds(0.5f);

            while (SubText.color.a < 1)
            {
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a + 0.05f);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => SubText.color.a >= 1);

            yield return new WaitForSeconds(3f);
            //Fade Out
            while (BossTitleCardMat.GetColor("_BaseColor").a > 0)
            {
                BossTitleCardMat.SetColor("_BaseColor", new Color(BossTitleCardMat.GetColor("_BaseColor").r, BossTitleCardMat.GetColor("_BaseColor").g, BossTitleCardMat.GetColor("_BaseColor").b, BossTitleCardMat.GetColor("_BaseColor").a - 0.05f));
                SubText.color = new Color(SubText.color.r, SubText.color.g, SubText.color.b, SubText.color.a - 0.05f);
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
        }
    }
}
