using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SkipButton : MonoBehaviour
{
    public void Start()
    {
        if (Director.Instance.ConfirmButton.GetComponent<Image>().material != null)
        {
            Director.Instance.ConfirmButton.GetComponent<Image>().material = Instantiate<Material>(Director.Instance.ConfirmButton.GetComponent<Image>().material);
            Director.Instance.ConfirmButton.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
        }

    }
    public void SkipRewards()
    {
        StartCoroutine(Skip());
    }
    private IEnumerator Skip()
    {

        ActionRewardManager rewardManager = Director.Instance.actionRewardManager;
        rewardManager.MoveRewards(false);

        yield return new WaitForSeconds(0.6f);
        Director.Instance.DisplayCharacterTab(false, false);


        Director.Instance.backButton.gameObject.SetActive(true);

        MoveableObject backButtonMovement = Director.Instance.backButton.GetComponent<MoveableObject>();

        //button moves in
        if (backButtonMovement != null)
        {
            backButtonMovement.Move(true, setInteractableWhenDone: false);
        }

        MoveableObject levelUpTextMovement = Director.Instance.LevelUpText.GetComponent<MoveableObject>();

        if (levelUpTextMovement != null)
        {
            levelUpTextMovement.Move(true);
        }

        rewardManager.SetConfirmButton(true);
        var CB = Director.Instance.ConfirmButton.GetComponent<ConfirmButton>();
        CB.SetOutline(1, Color.white * 100);
        yield return null;
    }
}
