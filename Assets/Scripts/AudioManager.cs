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
    public AudioMixer mixer;
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
                Play("Main Menu Theme", 0.5f);
            else if(SceneManager.GetActiveScene().name == "MAP2")
                Play("Coronus_Map", 0.5f);
           /* else if(SceneManager.GetActiveScene().name == "Prologue Ending")
                Play("Ending", 1f);
           */
        }
    }


    public void Play(string AudioName, float volume = 0, bool ApplyPitchVariations = false, float pitch = 0)
    {
        var s = Array.Find(sounds, sound => sound.AudioName == AudioName);
        if(s != null)
        {
            float pitchVarition = 0;
            if(s.type.name == "Music")
            {
                currentMusicTrack = s.AudioName;   
            }

            s.source.outputAudioMixerGroup = s.type;

            if (ApplyPitchVariations)
            {
                pitchVarition = UnityEngine.Random.Range(-0.1f, 0.1f);

                if(AudioName == "button_hover")
                    pitchVarition = UnityEngine.Random.Range(-0.05f, 0.05f);
            }
          
            if(volume == 0)
                s.source.volume = s.volume;
            else
                 s.source.volume = volume;

            if (pitch == 0)
                s.source.pitch = s.pitch + pitchVarition;
            else
                s.source.pitch = pitch;

            s?.source.Play();
        }
       
    }


    public static void QuickPlay(string AudioName, bool ApplyPitchVariations = false)//For controlling audio through editor
    {
        var s = Array.Find(AudioManager.Instance.sounds, sound => sound.AudioName == AudioName);
        float volume = 1;
        if (s != null)
        {
            volume = s.volume;
        }       
        AudioManager.Instance.Play(AudioName, s.volume, ApplyPitchVariations);
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
                float currentTime = 0;
                while (soundSource.source.volume < TargetVolume && soundSource.Fading)
                {
                   
                    currentTime += Time.deltaTime * FadeTime;
                    soundSource.source.volume = Mathf.Lerp(startVolume, TargetVolume, currentTime);
                    yield return null;
                }
            }

        }
       
    }

    public void Pause(string AudioName)
    {
        var soundSource = Array.Find(sounds, sound => sound.AudioName == AudioName);
        soundSource.source.Pause();

    }

    public void UnPause(string AudioName)
    {
        var soundSource = Array.Find(sounds, sound => sound.AudioName == AudioName);
        soundSource.source.UnPause();
        soundSource.source.Play();

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
    public bool Fading = false;
    public AudioMixerGroup type;

}
