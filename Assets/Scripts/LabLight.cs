using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LabLight : MonoBehaviour
{
    [NonSerialized]
    public float startIntensity;
    [NonSerialized]
    public float currentIntensity;
    [NonSerialized]
    public Light lightComponent;

    public void Start()
    {      
        lightComponent = GetComponent<Light>();
        startIntensity = lightComponent.intensity;
    }
}
