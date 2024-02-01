using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LabExplainer : MonoBehaviour
{
    public string Description;
    [NonSerialized]
    public GameObject currentEffectPopup;
    public Vector3 offset;
    public void SetDescription()
    {
        if (currentEffectPopup == null)
        {
            var EP = Instantiate(Director.Instance.EffectPopUp, this.transform);
            currentEffectPopup = EP;
        }
        else
        {
            currentEffectPopup.SetActive(true);
        }


        var EPtext = currentEffectPopup.GetComponentInChildren<TextMeshProUGUI>();
        EPtext.text = Description;
        currentEffectPopup.transform.localScale = new Vector3(0.015f, 0.015f, 1);
        currentEffectPopup.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 1, 0) + offset;
        Director.Instance.StartCoroutine(Tools.UpdateParentLayoutGroup(EPtext.gameObject));
    }

    public void Remove()
    {
        if (currentEffectPopup != null)
        {
            currentEffectPopup.SetActive(false);
        }
        
    }
}
