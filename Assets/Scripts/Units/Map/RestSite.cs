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
    public List<GameObject> restPoints;


    public Vector3 camPos;
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
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.party[i].gameObject.SetActive(true);
            Director.Instance.party[i].gameObject.transform.localScale = new Vector3(10f, 10f, 1f);
            Director.Instance.party[i].transform.position = restPoints[i].transform.position;
            if (i == 1 || i == 3)
            {
                Director.Instance.party[i].GetComponent<SpriteRenderer>().flipX = true;
            }
            Director.Instance.party[i].gameObject.transform.localScale = new Vector3(10f, 10f, 1f);
            var stateId = Animator.StringToHash("Resting");
            if (Director.Instance.party[i].anim.HasState(0, stateId))
            {
                print("CHARACTER RESTING SHOULD BE PLAYING");
                Director.Instance.party[i].anim.Play("Resting");
            }
        }
        Director.Instance.StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        Debug.LogWarning("Where rest site");
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        LabCamera.Instance.ReadjustCam();
        yield return new WaitForSeconds(2f);
        OptionsManager.Instance.blackScreen.gameObject.SetActive(false);
        Tools.ToggleUiBlocker(true, true, true);
        LabCamera.Instance.TimeDivider = 500;
        LabCamera.Instance.state = LabCamera.CameraState.SWAY;
        Director.Instance.CharacterSlotEnable();
        foreach (var button in buttons)
        {
            button.GetComponent<MoveableObject>().Move(true);
            button.interactable = true;
        }
        OptionsManager.Instance.CanPause = true;

    }
    public void Train()
    {
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.LevelUp(Director.Instance.party[i], false);
            DontDestroyOnLoad(Director.Instance.party[i].gameObject);

        }
        StartCoroutine(TransitionToMap());
    }
    public void Rest()
    {
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            Director.Instance.party[i].currentHP = Director.Instance.party[i].maxHP;
        }
        foreach (var button in buttons)
        {
            button.GetComponent<MoveableObject>().Move(false);
            button.interactable = false;
        }
        StartCoroutine(TransitionToMap());
    }

    public IEnumerator TransitionToMap()
    {
        Director.Instance.CharacterSlotEnable(true);
        if (!Director.Instance.DevMode)
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("DustyAureliaRestMeeting"), true, true);
        }
        yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
        for (int i = 0; i <= Director.Instance.party.Count - 1; i++)
        {
            DontDestroyOnLoad(Director.Instance.party[i].gameObject);
        }
        OptionsManager.Instance.Load("MAP2", "Coronus_Map", 1, 0.5f);
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.StaminaHighlightIsDisabled = true;
            unit.gameObject.SetActive(false);
            unit.anim.Play("Idle");
        }
    }

}
