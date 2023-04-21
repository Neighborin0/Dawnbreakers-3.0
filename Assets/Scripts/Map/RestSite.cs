using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;

public class RestSite : MonoBehaviour
{
    [SerializeField]
    private static Vector3[] restPos =
    {
         new Vector3(-11.7f, 0.64f,-30.3f),
          new Vector3(9f, 0.64f,-30.3f),
           new Vector3(-6f, 0.64f,-23.1f),
            new Vector3(5.7f, 0.64f,-20f),

    };
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }    
    }
    [SerializeField]
    private Image image;
    public List<Button> buttons;
    public static RestSite Instance { get; private set; }

    private void Start()
    {
        for(int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.party[i].gameObject.SetActive(true);
            Director.Instance.party[i].gameObject.transform.localScale = new Vector3(10f, 10f, 1f);
            Director.Instance.party[i].transform.position = restPos[i];
            if (i == 1 || i== 3)
            {
                Director.Instance.party[i].GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, false));

        if(!Director.Instance.DevMode)
        {
            BattleLog.CharacterDialog(ConvserationHandler.DustyAureliaRestMeeting, true, false);
        }
       
    }

    public IEnumerator FadeToBlack(float delay)
    {
        var screenImage = Director.Instance.blackScreen;
        for (int i = 0; i < 101; i++)
        {
            if (screenImage != null)
            {
                screenImage.color = new Color(0, 0, 0, i / 100f);
                print(i / 100f);
                yield return new WaitForSeconds(delay);
            }
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.party[i].GetComponent<SpriteRenderer>().flipX = false;
            Director.Instance.party[i].gameObject.SetActive(false);
          
        }
        StartCoroutine(Director.Instance.DoLoad("MAP2"));
    }

    public void Leave()
    {
        StartCoroutine(FadeToBlack(0.001f));
    }
    public void Train()
    {
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.LevelUp(Director.Instance.party[i], false);
        }
        StartCoroutine(FadeToBlack(0.001f));
    }
    public void Rest()
    {
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.party[i].currentHP = Director.Instance.party[i].maxHP;
        }
        StartCoroutine(FadeToBlack(0.01f));
    }

}
