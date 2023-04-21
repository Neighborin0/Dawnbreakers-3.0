using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class HighlightedObject : MonoBehaviour
{
    public bool IsHighlighted = true;
    public float StartingThickness;
    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", StartingThickness);
        gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
    }

    public void ToggleHighlight()
    {
        if (IsHighlighted && GetComponent<Button>().interactable)
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
            IsHighlighted = false;
        }
        else
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", StartingThickness);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
            IsHighlighted = true;
        }
    }
}
