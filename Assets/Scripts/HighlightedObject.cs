using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class HighlightedObject : MonoBehaviour
{
    public bool IsHighlighted = false;
    public float StartingThickness;
    public bool DoesntHighlightInBattle = false;
    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", StartingThickness);
        gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
    }

    public void ToggleHighlight()
    {
        if (!IsHighlighted)
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
        else
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", StartingThickness);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
            IsHighlighted = false;
        }
    }
}
