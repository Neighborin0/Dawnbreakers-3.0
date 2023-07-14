using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class CharacterJoinSceneScript : MonoBehaviour
{
    private Unit chosenUnit;
    void Start()
    {
        chosenUnit = Director.Instance.characterdatabase[UnityEngine.Random.Range(0, Director.Instance.characterdatabase.Count)];
        chosenUnit.Intro();
        //print(unit.introText[0]);
       StartCoroutine(Tools.SmoothMove(BattleLog.Instance.gameObject, 0f, 60, 0, 3.4f));
        StartIntro(chosenUnit);

    }

    private void StartIntro(Unit unit)
    {
        //BattleLog.CharacterDialog(unit.introText);
    }

    public void AddToParty()
    {

        StartCoroutine(Tools.SmoothMove(BattleLog.Instance.gameObject, 0f, 60, 0, -3.4f));
        Director.AddUnitToParty(chosenUnit.name);
        SceneManager.LoadScene("MAP2");
    }



}
