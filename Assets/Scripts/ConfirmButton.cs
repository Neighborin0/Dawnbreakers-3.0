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
        BattleLog.Instance.Move(false);
        Director.Instance.LevelUpText.GetComponent<MoveableObject>().Move(true);
        Director.Instance.ConfirmButton.GetComponent<MoveableObject>().Move(true);
        Director.Instance.TabGrid.GetComponent<MoveableObject>().Move(false);  
        yield return new WaitForSeconds(0.5f);
        Director.Instance.StartCoroutine(Director.Instance.DoLoad("MAP2"));
        Director.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitUntil(() => Director.Instance.blackScreen.color == new Color(0, 0, 0, 1));
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