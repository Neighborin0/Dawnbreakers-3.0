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

public class AudioManager : MonoBehaviour
{
  
    public static AudioManager Instance { get; private set;  }
    public Sound[] sounds;
     void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
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
        if(s != null)
        {
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s?.source.Play();
        }
       
    }

    public IEnumerator Fade(bool FadeIn, string AudioName, float FadeTime, bool Stop = false)
    {
        var soundSource = Array.Find(sounds, sound => sound.AudioName == AudioName);
        float startVolume = soundSource.source.volume;

        if (!FadeIn)
        {
            Debug.Log("Fading out nooo");
            Debug.Log(soundSource.AudioName);
            while (soundSource.source.volume > 0)
            {
                Debug.Log("Should be lowering volume");
                soundSource.source.volume -= startVolume * Time.deltaTime / FadeTime;
                yield return null;
            }

            if(Stop)
            {
                soundSource.source.Stop();
            }
        }
        else
        {
            soundSource.source.volume = 0;
            Debug.Log("Fading in yessss");
            while (soundSource.source.volume < 1)
            {
                soundSource.source.volume += startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }
        }
    }
        

   
}



[Serializable]
public enum SoundType
{
    SFX,
    MUSIC,
    
};

[Serializable]
public class Sound
{
    public SoundType soundType;

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
