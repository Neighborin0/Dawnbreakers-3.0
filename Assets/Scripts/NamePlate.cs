using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NamePlate : MonoBehaviour
{
    //public TextMeshProUGUI nameText;
    public GridLayoutGroup IconGrid;
    public GameObject DEF_icon;
    public TextMeshProUGUI defText;
    public Unit unit;

    public void Start()
    {
        if(unit.IsPlayerControlled)
        {
            DEF_icon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(67.1f, DEF_icon.transform.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
    public void UpdateArmor()
    {
        if(!DEF_icon.activeSelf)
        {
            Director.Instance.StartCoroutine(Fade(true));
        }
        defText.text = unit.armor.ToString();
        if(unit.armor <= 0)
        {
            Director.Instance.StartCoroutine(Fade(false));
        }
    }

    public IEnumerator Fade(bool FadeIn)
    {
        var DEfImage = DEF_icon.GetComponent<Image>();
        if (!FadeIn)
        {
            if (gameObject != null)
            {
                while (DEfImage.color.a > 0 && DEF_icon != null)
                {
                    DEfImage.color = new Color(DEfImage.color.r, DEfImage.color.g, DEfImage.color.b, DEfImage.color.a - 0.1f);
                    defText.color = new Color(defText.color.r, defText.color.g, defText.color.b, defText.color.a - 0.1f);
                    yield return new WaitForSeconds(0.001f);
                }
                yield return new WaitUntil(() => DEfImage.color.a <= 0);
                DEF_icon.SetActive(false);
            }
        } 
        else 
        {
            if (gameObject != null)
            {
                DEF_icon.SetActive(true);
                while (DEfImage.color.a < 1 && DEF_icon != null)
                {
                    DEfImage.color = new Color(DEfImage.color.r, DEfImage.color.g, DEfImage.color.b, DEfImage.color.a + 0.1f);
                    defText.color = new Color(defText.color.r, defText.color.g, defText.color.b, defText.color.a + 0.1f);
                    yield return new WaitForSeconds(0.001f);
                }
                yield return new WaitUntil(() => DEfImage.color.a >= 1);   
            }
        }
    }
   
     
}
