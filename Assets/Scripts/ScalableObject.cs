using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;


public class ScalableObject : MonoBehaviour
{
    public Vector3 newScaleSize;
    public Vector3 oldScaleSize;
    [NonSerialized]
    private IEnumerator scaler;

    public void Start()
    {
        oldScaleSize = transform.localScale;
        if(GetComponent<Button>() != null )
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(delegate { DisableScale(0.01f); });
        }
    }
    public void EnableScale(float delay = 0.01f)
    {
        if (GetComponent<Button>() != null)
        {
            if(GetComponent<Button>().interactable)
            {
                Scale(true, delay);
            }
        }
        else
        {
            Scale(true, delay);
        }
    }

    void Scale(bool ScaleUp, float delay = 0.01f)
    {
        if (scaler != null)
        {
            StopCoroutine(scaler);   
        }
        if(ScaleUp)
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), newScaleSize, delay);
        else
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), oldScaleSize, delay);


        StartCoroutine(scaler);
    }
    public void DisableScale(float delay = 0.01f)
    {
       Scale(false, delay);
    }
}
