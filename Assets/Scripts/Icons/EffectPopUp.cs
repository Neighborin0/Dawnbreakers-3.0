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

public class EffectPopUp : MonoBehaviour
{
    public void Start()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Camera.main.transform.rotation;
    }
    /* private TextMeshProUGUI text;
     public GameObject childeffectPopUp;
     bool ISchildEP = false;
     public bool AlreadyAssignedPosition = false;

     private void Awake()
     {
         text = GetComponentInChildren<TextMeshProUGUI>();
     }

      void OnEnable()
     {
         CheckForSpecialText(); 


     }

     private void OnDisable()
     {
         if (childeffectPopUp != null)
             childeffectPopUp.SetActive(false);
     }
     public void CheckForSpecialText()
     {
         var effectText = text.text;
         if(!ISchildEP)
         {
             switch (effectText)
             {
                 case var s when effectText.Contains("<sprite name=\"BLOCK\">"):
                     {
                         if (childeffectPopUp == null)
                         {
                             var EP = Instantiate(Director.Instance.EffectPopUp, Director.Instance.canvas.transform);
                             EP.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                             childeffectPopUp = EP;
                             var rectTrans = transform.GetComponent<RectTransform>();
                             childeffectPopUp.transform.GetComponent<RectTransform>().localPosition = new Vector3(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y, 0);
                             var EPtext = childeffectPopUp.GetComponentInChildren<TextMeshProUGUI>();
                             EPtext.text = "<sprite name=\"BLOCK\">: Reduces the DMG of the\n next hit by 50%.";
                         }
                         else
                         {
                             childeffectPopUp.SetActive(true);
                         }
                         childeffectPopUp.GetComponent<EffectPopUp>().ISchildEP = true;
                     }
                     break;

                 case var s when effectText.Contains("<sprite name=\"FORTIFY\">"):
                     {
                         if (childeffectPopUp == null)
                         {
                             var EP = Instantiate(Director.Instance.EffectPopUp, Director.Instance.canvas.transform);
                             EP.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                             childeffectPopUp = EP;
                             var rectTrans = transform.GetComponent<RectTransform>();
                             childeffectPopUp.transform.GetComponent<RectTransform>().localPosition = new Vector3(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y, 0);
                             var EPtext = childeffectPopUp.GetComponentInChildren<TextMeshProUGUI>();
                             EPtext.text = "<sprite name=\"FORTIFY\">: Reduces damage taken until \nthe start of the next round.";
                         }
                         else
                         {
                             childeffectPopUp.SetActive(true);
                         }
                         childeffectPopUp.GetComponent<EffectPopUp>().ISchildEP = true;
                     }
                     break;

                 case var s when effectText.Contains("<sprite name=\"VIGOR\">"):
                     {
                         if (childeffectPopUp == null)
                         {
                             var EP = Instantiate(Director.Instance.EffectPopUp, Director.Instance.canvas.transform);
                             EP.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                             childeffectPopUp = EP;
                             var rectTrans = transform.GetComponent<RectTransform>();
                             childeffectPopUp.transform.GetComponent<RectTransform>().localPosition = new Vector3(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y, 0);
                             var EPtext = childeffectPopUp.GetComponentInChildren<TextMeshProUGUI>();
                             EPtext.text = "<sprite name=\"VIGOR\": Increases damage.";
                         }
                         else
                         {
                             childeffectPopUp.SetActive(true);
                         }
                         childeffectPopUp.GetComponent<EffectPopUp>().ISchildEP = true;
                     }
                     break;
             }

         }
     }
    */
}
