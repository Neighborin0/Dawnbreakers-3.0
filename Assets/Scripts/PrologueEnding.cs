using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PrologueEnding : MonoBehaviour
{
    private Unit chosenUnit;

    void Awake()
    {
        Destroy(Director.Instance.timeline.gameObject);
    }

    void Start()
    {
        StartCoroutine(EndConversation());
    }

    private IEnumerator EndConversation()
    {
        BattleLog.Instance.CharacterDialog(TutorialConversationHandler.PrologueEnding, false, true);
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        OptionsManager.Instance.StartCoroutine(OptionsManager.Instance.DoLoad("Main Menu"));
    }
}
