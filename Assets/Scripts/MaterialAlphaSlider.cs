using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MaterialAlphaSlider : MonoBehaviour
{
    public Material material;

    public void AlphaSlider(float value)
    {
        Color color = material.color;
        color.a = value;
    }

}
