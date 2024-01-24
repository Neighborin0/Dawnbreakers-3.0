using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Audio;
using JetBrains.Annotations;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering;
using static System.Collections.Specialized.BitVector32;

public class AmbienceDoer : MonoBehaviour
{

    public AudioSource[] ambiences;

 
    
    public void Start()
    {
        SceneManager.sceneUnloaded += FadeAmbience;

        foreach (var ambience in ambiences)
        {
            print(ambience.clip.name);
            AudioManager.Instance.StartCoroutine(AudioManager.Instance.Fade(0.4f, ambience.clip.name, 1f, false));
        }
    }

    private void FadeAmbience(Scene scene)
    {
        foreach (var ambience in ambiences)
        {
            if(ambience != null)
            {
                AudioManager.Instance.StartCoroutine(AudioManager.Instance.Fade(0f, ambience.clip.name, 1f, false));
            }
            
        }
        SceneManager.sceneUnloaded -= FadeAmbience;
    }

}

