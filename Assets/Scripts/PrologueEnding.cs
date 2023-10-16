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
            Destroy(Director.Instance.timeline.gameObject);
        }      
        gameObject.transform.localScale = new Vector3(0, 0, 0);
    }

    void Start()
    {
        StartCoroutine(EndConversation());
    }

    private IEnumerator EndConversation()
    {
      
        yield return new WaitForSeconds(1);
        for (int i = 0; i < text.Count; i++)
        {
            StartCoroutine(Tools.FadeText(text[i], 0.01f, true, false));
            yield return new WaitForSeconds(1.5f);
            if (i == 0)
            {
                StartCoroutine(Tools.SmoothScaleObj(transform, new Vector3(1f, 1f, 1f), 0.01f));
            }
            yield return new WaitForSeconds(3f);
            StartCoroutine(Tools.FadeText(text[i], 0.01f, false, true));
            yield return new WaitForSeconds(2f);
            if(i == 4)
            {
                OptionsManager.Instance.StartCoroutine(OptionsManager.Instance.DoLoad("Main Menu"));
            }

        }
    }
}
