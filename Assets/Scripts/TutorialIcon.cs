using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TutorialIcon : MonoBehaviour
{
    private bool IsBeingDestroyed = false;
    [SerializeField]
    private TextMeshProUGUI Text1;
    [SerializeField] 
    private TextMeshProUGUI Text2;
    [SerializeField]
    private bool HasAnotherTextBox = false;
    [SerializeField]
    private GameObject BackButton;
    [SerializeField]
    private TextMeshProUGUI NextButtonText;



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsBeingDestroyed || Input.GetKeyDown(KeyCode.Mouse0) && !IsBeingDestroyed)
        {
            if(!HasAnotherTextBox)
            StartCoroutine(StartDestroying());
            else
                SetSecondTextBlockActive();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && !IsBeingDestroyed && BackButton.gameObject.activeSelf || Input.GetKeyDown(KeyCode.Mouse1) && !IsBeingDestroyed && BackButton.gameObject.activeSelf)
        {
            GoBack();
        }
    }

    public void SetSecondTextBlockActive()
    {
        HasAnotherTextBox = false;
        Text1.gameObject.SetActive(false);
        Text2.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
        NextButtonText.text = "Close";
    }

    public void GoBack()
    {
        HasAnotherTextBox = true;
        Text1.gameObject.SetActive(true);
        Text2.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        NextButtonText.text = "Next";
    }
    public void Destroy()
    {
        StartCoroutine(StartDestroying());
    }

    private IEnumerator StartDestroying()
    {
        AudioManager.QuickPlay("button_Hit_002");
        IsBeingDestroyed = true;
        GetComponent<MoveableObject>().Move(false);
        BattleSystem.Instance.playerUnits[0].StartDecision(false);
        Tools.ToggleUiBlocker(true, true);
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
