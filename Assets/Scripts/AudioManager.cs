using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Audio;
using JetBrains.Annotations;

public class AudioManager : MonoBehaviour
{
  
    public static AudioManager Instance { get; private set;  }
    public Sound[] sounds;
     void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach(Sound s in sounds) 
            {     
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
    }

    public void Play(string AudioName)
    {
        var s = Array.Find(sounds, sound => sound.AudioName == AudioName);
        s?.source.Play();
    }

   
}

[Serializable]
public class Sound
{
    public AudioClip clip;
    public string AudioName;
    [Range(0, 1)]
    public float volume;
    [Range(0.1f, 3)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
    public bool loop;
}
