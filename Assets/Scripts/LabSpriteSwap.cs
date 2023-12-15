using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.Rendering;


public class LabSpriteSwap : MonoBehaviour
{
    public Image image;
    public Sprite originalSprite;
    public Sprite highlightedSprite;
 
   public void Revert()
    {
        image.sprite = originalSprite;
    }

    public void Change()
    {
        image.sprite = highlightedSprite;
    }

}
