using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class HighlightedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsHighlighted = false;
    public float StartingThickness;
    public bool DoesntHighlightInBattle = false;
    public bool disabled = false;
    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", StartingThickness);
        gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
    }

     void Update()
    {
        if(GetComponent<Button>() != null)
        {
            if (!GetComponent<Button>().interactable)
            {
                this.disabled = true;
            }
            
        }

        if (mouse_over)
        {
            EnableHighlight();
        }
    }

    public void EnableHighlight()
    {
        if (!disabled)
        {
            if (DoesntHighlightInBattle)
            {
                if (BattleSystem.Instance.state != BattleStates.BATTLE)
                {
                    gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
                    gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                    IsHighlighted = true;
                }
                else
                {
                    IsHighlighted = false;
                }
            }
            else
            {
                gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
                gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                IsHighlighted = true;
            }
        }
    }

    public void DisableHighlight()
    {
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", StartingThickness);
        gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
        IsHighlighted = false;
    }
   
     private bool mouse_over = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
}
