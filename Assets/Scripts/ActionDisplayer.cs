using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;
using UnityEditor.Build;

public class ActionDisplayer : MonoBehaviour
{
    public TextMeshProUGUI baseText;

    public void Start()
    {
        baseText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public IEnumerator Fade(bool FadeIn)
    {
        var img = GetComponent<Image>();
        if (!FadeIn)
        {
            if (gameObject != null)
            {
                while (img.color.a > 0 && this != null)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - 0.1f);
                    baseText.color = new Color(baseText.color.r, baseText.color.g, baseText.color.b, baseText.color.a - 0.1f);
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitUntil(() => img.color.a <= 0);
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (gameObject != null)
            {
                gameObject.SetActive(true);
                while (img.color.a < 1 && this != null)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.1f);
                    baseText.color = new Color(baseText.color.r, baseText.color.g, baseText.color.b, baseText.color.a + 0.1f);
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitUntil(() => img.color.a >= 1);
            }
        }
    }

}
