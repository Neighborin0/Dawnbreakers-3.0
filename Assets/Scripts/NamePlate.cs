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
    public void UpdateArmor(float ArmorAdded, bool ForceClose = false)
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
                

                fadeCoroutine = Fade(true, this);

                Director.Instance.StartCoroutine(fadeCoroutine);
            }
         
        
        defText.text = unit.armor.ToString();

        if(unit.armor <= 0 || ForceClose)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
                
            fadeCoroutine = Fade(false, this);
            unit.armor = 0;
            Director.Instance.StartCoroutine(fadeCoroutine);
        }
    }

    public static IEnumerator Fade(bool FadeIn, NamePlate namePlate)
    {
        var DEfImage = namePlate.DEF_icon.GetComponent<Image>();
        if (!FadeIn)
        {
            if (namePlate.gameObject != null)
            {
                while (DEfImage.color.a > 0 && namePlate.DEF_icon != null)
                {
                    DEfImage.color = new Color(DEfImage.color.r, DEfImage.color.g, DEfImage.color.b, DEfImage.color.a - 0.1f);
                    namePlate.defText.color = new Color(namePlate.defText.color.r, namePlate.defText.color.g, namePlate.defText.color.b, namePlate.defText.color.a - 0.1f);
                    yield return new WaitForSeconds(0.05f);
                }
                namePlate.unit.armor = 0;
                yield return new WaitUntil(() => DEfImage.color.a <= 0);
               if (namePlate != null)
                namePlate.DEF_icon.SetActive(false);
            }
        } 
        else 
        {
            if (namePlate.gameObject != null)
            {
                namePlate.DEF_icon.SetActive(true);
                while (DEfImage.color.a < 1 && namePlate.DEF_icon != null)
                {
                    DEfImage.color = new Color(DEfImage.color.r, DEfImage.color.g, DEfImage.color.b, DEfImage.color.a + 0.1f);
                    namePlate.defText.color = new Color(namePlate.defText.color.r, namePlate.defText.color.g, namePlate.defText.color.b, namePlate.defText.color.a + 0.1f);
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitUntil(() => DEfImage.color.a >= 1);   
            }
        }
    }
   
     
}
