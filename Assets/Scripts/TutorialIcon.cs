using System.Collections;
using UnityEngine;

public class TutorialIcon : MonoBehaviour
{
    [SerializeField]
    private bool PauseStaminaTimers = false;

    private void Start()
    {
        if (PauseStaminaTimers)
            Tools.PauseAllStaminaTimers();
    }
    public void Destroy()
    {
        StartCoroutine(StartDestroying());
    }

    private IEnumerator StartDestroying()
    {
        StartCoroutine(Tools.SmoothMoveUI(this.GetComponent<RectTransform>(), 5000, 140, 0.01f));
        if (PauseStaminaTimers)
            Tools.UnpauseAllStaminaTimers();
        yield return new WaitUntil(() => this.GetComponent<RectTransform>().position.x == 5000);
        Destroy(this.gameObject);
    }
}
