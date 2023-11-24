using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConfirmButton : MonoBehaviour
{

    public void StartDestroy()
    {
        StartCoroutine(DelayedDestroy());
    }
    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.3f);
        BattleLog.Instance.GetComponent<MoveableObject>().Move(false);
        Director.Instance.LevelUpText.GetComponent<MoveableObject>().Move(true);
        Director.Instance.ConfirmButton.GetComponent<MoveableObject>().Move(true);
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(false);  
        yield return new WaitForSeconds(0.5f);
        OptionsManager.Instance.Load("MAP2");
        OptionsManager.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        Director.Instance.timeline.RefreshTimeline();
        BattleLog.Instance.characterdialog.gameObject.SetActive(false);
        Director.Instance.blackScreen.gameObject.SetActive(false);
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
}
