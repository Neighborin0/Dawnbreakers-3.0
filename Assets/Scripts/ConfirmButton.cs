using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConfirmButton : MonoBehaviour
{
    public void Start()
    {
        if(Director.Instance.ConfirmButton.GetComponent<Image>().material != null)
        {
            Director.Instance.ConfirmButton.GetComponent<Image>().material = Instantiate<Material>(Director.Instance.ConfirmButton.GetComponent<Image>().material);
            Director.Instance.ConfirmButton.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
        }
      
    }
    public void StartDestroy()
    {
        StartCoroutine(DelayedDestroy());
    }
    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.3f);
        BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
        if (Director.Instance.LevelUpText != null)
            Director.Instance.LevelUpText.GetComponent<MoveableObject>().Move(true);

        Director.Instance.actionRewardManager.ClearRewards();

        Director.Instance.ConfirmButton.GetComponent<MoveableObject>().Move(false);
        Director.Instance.backButton.GetComponent<MoveableObject>().Move(false);
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(false);  
        yield return new WaitForSeconds(0.5f);
        OptionsManager.Instance.Load("MAP", "Coronus_Map", 1, 0.5f);
        OptionsManager.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        NodeController.Instance.parentCanvas.gameObject.SetActive(true);
        Director.Instance.timeline.RefreshTimeline();
        CombatTools.ReturnPipCounter().ResetPips();
        BattleLog.Instance.characterdialog.gameObject.SetActive(false);
        Director.Instance.blackScreen.gameObject.SetActive(false);
        Tools.ClearAllEffectPopup();
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
            unit.gameObject.SetActive(false);
        }
        foreach (var CT in (Director.Instance.TabGrid.GetComponentsInChildren<CharacterTab>()))
        {
            Destroy(CT.gameObject);
        }
        print("SHOULD BE TRANSITIONING");
        this.GetComponent<Button>().interactable = false;
        yield break;
    }

    public void SetOutline(float thickness, Color color)
    {
        var confirmImage = Director.Instance.ConfirmButton.GetComponent<Image>();
        if (confirmImage != null)
        {

            confirmImage.material.SetFloat(
                "OutlineThickness",
                thickness
            );

            confirmImage.material.SetColor(
                "OutlineColor",
                color
            );
        }
    }
}
