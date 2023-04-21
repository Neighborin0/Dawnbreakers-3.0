using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TargetLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Texture[] textures;
    public int animationStep;
    public float fpsCounter;
    public float fps;
     void Awake()
    {
        lineRenderer= GetComponent<LineRenderer>();

    }

    private void Update()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps && this != null)
        {
            animationStep++;
            if(animationStep == textures.Length)
            {
                animationStep = 0;
            }
            lineRenderer.material.SetTexture("_BaseMap", textures[animationStep]);
            fpsCounter = 0f;

        }

    }
}
