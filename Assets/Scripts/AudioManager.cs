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

public class AudioManager : MonoBehaviour
{
  
    public static AudioManager Instance { get; private set;  }
    public Sound[] sounds;
    public string currentMusicTrack;
    private bool Stopped = false;
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

            if (SceneManager.GetActiveScene().name == "Main Menu")
                Play("Main Menu Theme", 1);
            else
                Play("Coronus_Map", 1);
        }
    }


    public void Play(string AudioName, float volume = 0)
    {
        var s = Array.Find(sounds, sound => sound.AudioName == AudioName);
        if(s != null)
        {
            if(s.soundType == SoundType.MUSIC)
            {
                currentMusicTrack = s.AudioName;
            }

            s.source.volume = volume;
            s.source.pitch = s.pitch;
            s?.source.Play();
        }
       
    }

    public static void QuickPlay(string AudioName)//For controlling audio through editor
    {
        var s = Array.Find(AudioManager.Instance.sounds, sound => sound.AudioName == AudioName);
        float volume = 1;
        if (s != null)
        {
            volume = s.volume;
        }
        AudioManager.Instance.Play(AudioName, s.volume);
    }

    public void Stop(string AudioName)
    {
        var s = Array.Find(sounds, sound => sound.AudioName == AudioName);
        if (s != null)
        {
            s?.source.Stop();
        }
    }

    public Sound ReturnSound(string AudioName)
    {
        var s = Array.Find(sounds, sound => sound.AudioName == AudioName);
        return s;
    }

    public IEnumerator Fade(float TargetVolume, string AudioName, float FadeTime, bool Stop = false)
    {
        var soundSource = Array.Find(sounds, sound => sound.AudioName == AudioName);
      
        if (soundSource != null)
        {
            soundSource.Fading = false;
            float startVolume = soundSource.source.volume;

            if (soundSource.source.volume > TargetVolume) //Fade Out
            {
                Debug.Log("Fading out nooo");
                Debug.Log(soundSource.AudioName);
                soundSource.Fading = true;
                while (soundSource.source.volume > TargetVolume && soundSource.Fading)
                {
                    soundSource.source.volume -= startVolume * Time.deltaTime / FadeTime;
                    yield return null;
                }

                if (Stop)
                {
                    soundSource.source.Stop();
                }
            }
            else //Fade In
            {
                Debug.Log("Fading in: " + soundSource.AudioName);
                soundSource.Fading = true;
                while (soundSource.source.volume < TargetVolume && soundSource.Fading)
                {
                    soundSource.source.volume += Time.deltaTime * FadeTime;

                    yield return null;
                }
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
    [Range(0, 2)]
    public float volume;
    [Range(0.1f, 3)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
    public bool loop;
    public bool Fading = false;
}
