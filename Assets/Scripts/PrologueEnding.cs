using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;

public class PrologueEnding : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> text;

    void Awake()
    {
        if(Director.Instance != null)
        {
            Destroy(Director.Instance.gameObject);
        }
        if (OptionsManager.Instance != null)
        {
            if (OptionsManager.Instance.blackScreen.color.a > 0)
                StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
        }
        gameObject.transform.localScale = new Vector3(0, 0, 0);
    }

    void Start()
    {
        StartCoroutine(EndConversation());
    }

    private IEnumerator EndConversation()
    {
        AudioManager.Instance.Play("Ending");
        AudioManager.Instance.StartCoroutine(AudioManager.Instance.Fade(0.5f, "Ending", 0.5f, false));
        yield return new WaitForSeconds(2);
        for (int i = 0; i < text.Count; i++)
        {
            StartCoroutine(Tools.FadeText(text[i], 0.01f, true, false));
            AudioManager.QuickPlay("low_hum_001", true);
            yield return new WaitForSeconds(1.2f);
            if (i == 0)
            {
                AudioManager.QuickPlay("ending_rumble_001");
            }
            yield return new WaitForSeconds(0.3f);
            if (i == 0)
            {
                transform.GetComponent<Animator>().Play("MoteEnding");
            }
            yield return new WaitForSeconds(3f);
            AudioManager.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "low_hum_001", 2f, true));
            StartCoroutine(Tools.FadeText(text[i], 0.01f, false, true));
            yield return new WaitForSeconds(7.5f);
            if(i == 5)
            {
                AudioManager.Instance.StartCoroutine(AudioManager.Instance.Fade(0, "ending_ambience_001", 1f, true));
                OptionsManager.Instance.StartCoroutine(OptionsManager.Instance.DoLoad("Main Menu", "Main Menu Theme"));
            }

        }
    }
}
