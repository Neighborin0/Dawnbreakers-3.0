using System.Collections;
using UnityEditor;
using UnityEngine;

public class TutorialIcon : MonoBehaviour
{
    private bool IsBeingDestroyed = false;



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsBeingDestroyed)
        {
            StartCoroutine(StartDestroying());
        }
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
