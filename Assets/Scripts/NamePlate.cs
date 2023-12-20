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

    public IEnumerator fadeCoroutine;

    public void Start()
    {
        if(unit.IsPlayerControlled)
        {
            DEF_icon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(67.1f, DEF_icon.transform.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
    public void UpdateArmor(float ArmorAdded)
    {
        Debug.LogWarning($"{ArmorAdded}");
        Debug.LogWarning($"{ArmorAdded > 0}");

            if(ArmorAdded > 0)
            {
                Debug.LogWarning($"{ArmorAdded > 0}");
                var DEfImage = DEF_icon.GetComponent<Image>();
                DEfImage.color = new Color(DEfImage.color.r, DEfImage.color.g, DEfImage.color.b, 0);
                defText.color = new Color(defText.color.r, defText.color.g, defText.color.b, 0);
               

                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                

                fadeCoroutine = Fade(true);

                Director.Instance.StartCoroutine(fadeCoroutine);
            }
         
        
        defText.text = unit.armor.ToString();
        if(unit.armor <= 0)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
                
            fadeCoroutine = Fade(false);

            Director.Instance.StartCoroutine(fadeCoroutine);
        }
    }

    public IEnumerator Fade(bool FadeIn)
    {
        var DEfImage = DEF_icon.GetComponent<Image>();
        Debug.LogWarning("IS THIS SHIT EVEN WORKING????????");
        if (!FadeIn)
        {
            if (gameObject != null)
            {
                while (DEfImage.color.a > 0 && DEF_icon != null)
                {
                    DEfImage.color = new Color(DEfImage.color.r, DEfImage.color.g, DEfImage.color.b, DEfImage.color.a - 0.1f);
                    defText.color = new Color(defText.color.r, defText.color.g, defText.color.b, defText.color.a - 0.1f);
                    yield return new WaitForSeconds(0.05f);
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
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitUntil(() => DEfImage.color.a >= 1);   
            }
        }
    }
   
     
}
