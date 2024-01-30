using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using System;
using static BattleSystem;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }
    public Image SettingsMenu;
    public IEnumerator generalCoruntine;
    public struct FilteredResolutions
    {
        public int width;
        public int height;
        public int refreshRate;
    }

    //Graphics
    public List<FilteredResolutions> filteredResolutions;
    public TMPro.TMP_Dropdown resolutionsDropdown;
    public TMPro.TMP_Dropdown vsynDropdown;
    public TMPro.TMP_Dropdown fullScreenDropdown;
    public TMPro.TMP_Dropdown QualityDropdown;
    public TMPro.TMP_Dropdown FPSLimit;
    //public TMPro.TMP_Dropdown BloomDropDown;
    //public Slider BloomSlider;
    //public TextMeshProUGUI bloomValue;

    //Gameplay
    public Slider MapSensitivitySlider;
    public TextMeshProUGUI mapSensValue;
    public Slider WaitTimeMultiplier;
    public TextMeshProUGUI waitTimeValue;
    public Slider TextSpeedslider;
    public TextMeshProUGUI TextSpeedvalue;

    //Audio
    public Slider MasterVolumeSlider;
    public TextMeshProUGUI MasterVolumeSliderValue;
    public Slider SFXSlider;
    public TextMeshProUGUI SFXSliderValue;
    public Slider MusicSlider;
    public TextMeshProUGUI MusicSliderValue;



    public FPSCounter fPSCounter;
    public bool SettingsMenuDisabled = true;
    public Image blackScreen;
    public Button quitButton;
    public Button Continue;
    public Canvas canvas;
    public int IntensityLevel = 0;
    //public int bloomMultiplier;
    public float UserTimelineSpeedDelay = 100f;
    public float mapSensitivityMultiplier = 1;
    public float textSpeedMultiplier = 1;
    public bool CanPause = true;
    
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
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
            
#else
            Debug.unityLogger.logEnabled = false;
            OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
            OptionsManager.Instance.blackScreen.color = new Color(0, 0, 0, 1);
            StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.01f, false));

#endif
        }

    }

    void Start()
    {
        filteredResolutions = new List<FilteredResolutions>();
        resolutionsDropdown.ClearOptions();
        List<string> resoultionparams = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            var fr = new FilteredResolutions
            {
                width = Screen.resolutions[i].width,
                height = Screen.resolutions[i].height,
                refreshRate = Screen.resolutions[i].refreshRate
            };
            if (fr.refreshRate == Screen.currentResolution.refreshRate)
            {
                string option = fr.width + " x " + fr.height;
                filteredResolutions.Add(fr);
                resoultionparams.Add(option);
                if (fr.width == Screen.width && fr.refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResIndex = i;
                }
            }

        }

        resolutionsDropdown.AddOptions(resoultionparams);
        resolutionsDropdown.value = currentResIndex;
        resolutionsDropdown.RefreshShownValue();
        vsynDropdown.value = QualitySettings.vSyncCount;
        if (Screen.fullScreen)
        {
            fullScreenDropdown.value = 0;
        }
        else
            fullScreenDropdown.value = 1;
        QualityDropdown.value = QualitySettings.GetQualityLevel();
        FPSLimit.value = Application.targetFrameRate;


        MapSensitivitySlider.value = Tools.CheckPlayerPrefsFloat("MapSensitivity", 1f);
        TextSpeedslider.value = Tools.CheckPlayerPrefsFloat("TextSpeed", 1f);
        WaitTimeMultiplier.value = Tools.CheckPlayerPrefsFloat("WaitTime", 1f);

        SetWaitTime();
        SetTextSpeed();
        SetMapSensitivity();


        mapSensValue.text = Math.Round(MapSensitivitySlider.value, 1).ToString();
        TextSpeedvalue.text = Math.Round(TextSpeedslider.value, 1).ToString();
        waitTimeValue.text = Math.Round(WaitTimeMultiplier.value, 1).ToString();

        MasterVolumeSlider.value = Tools.CheckPlayerPrefsFloat("MasterVolume", 0.5f);
        SFXSlider.value = Tools.CheckPlayerPrefsFloat("SFXVolume", 0.5f);
        MusicSlider.value = Tools.CheckPlayerPrefsFloat("MusicVolume", 0.5f);

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

        MasterVolumeSliderValue.text = Math.Round(MasterVolumeSlider.value * 100, 0).ToString();
        SFXSliderValue.text = Math.Round(SFXSlider.value * 100, 0).ToString();
        MusicSliderValue.text = Math.Round(MusicSlider.value * 100, 0).ToString();


    }


    void Update()
    {


        if (Input.GetKeyDown(KeyCode.F) && fPSCounter != null)
        {
            FPSCounterEnable();
        }
       
        if (Input.GetKeyDown(KeyCode.Tab) && CanPause)
        {
            if (BattleSystem.Instance != null && Director.Instance != null && BattleSystem.Instance.CheckPlayableState())
            {
                if (SettingsMenuDisabled && !BattleSystem.Instance.Paused)
                {
                    Director.Instance.previousCameraState = LabCamera.Instance.state;
                    LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                    CombatTools.PauseStaminaTimer();
                }
                else
                {

                    LabCamera.Instance.state = Director.Instance.previousCameraState;
                    if(!CombatTools.CheckIfAnyUnitIsDeciding() && !BattleSystem.Instance.Paused)
                    {
                        CombatTools.UnpauseStaminaTimer();
                    }
                   
                }
            }
           
              
            Move(SettingsMenuDisabled);

        }
    }

    public void FPSCounterEnable()
    {
        if (fPSCounter.gameObject.activeSelf)
        {
            fPSCounter.gameObject.SetActive(false);
        }
        else
        {
            fPSCounter.gameObject.SetActive(true);
        }
    }

  

    public void SetResolution(int resolutionIndex)
    {
       var resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVsync(int vsyncIndex)
    {
        QualitySettings.vSyncCount = vsyncIndex;
    }

    public void SetTargetFrameRate(int fpsLimitIndex)
    {
        QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = fpsLimitIndex;
        if (fpsLimitIndex == 0)
        {
            Application.targetFrameRate = -1;
            print("Unlimited");
        }
        else if(fpsLimitIndex == 1)
        {
            Application.targetFrameRate = 30;
            print(Application.targetFrameRate);
        }
        else if (fpsLimitIndex == 2)
        {
            Application.targetFrameRate = 60;
            print(Application.targetFrameRate);
        }
        else if (fpsLimitIndex == 3)
        {
            Application.targetFrameRate = 120;
            print(Application.targetFrameRate);
        }
        else if (fpsLimitIndex == 4)
        {
            Application.targetFrameRate = 144;
            print(Application.targetFrameRate);
        }
        else if (fpsLimitIndex == 5)
        {
            Application.targetFrameRate = 240;
            print(Application.targetFrameRate);
        }
    }

    public void SetQualitySettings(int qualityindex)
    {
        QualitySettings.SetQualityLevel(qualityindex);
        print(QualitySettings.GetQualityLevel().ToString());
    }
    public void SetFullScreen(int fsIndex)
    {
        if (fsIndex == 0)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    public void SetBloom(int bloomVal)
    {
        /* bloomValue.text = Math.Round(BloomSlider.value, 1).ToString() + "x";
         foreach (var volume in FindObjectsOfType<Volume>())
         {
             if (volume.sharedProfile.TryGet<UnityEngine.Rendering.Universal.Bloom>(out var bloom))
             {
                 bloom.intensity.value = (float)Math.Round(BloomSlider.value, 1);
             }
         }
        */
     
    }

    public void SetMapSensitivity()
    {
        mapSensValue.text = Math.Round(MapSensitivitySlider.value, 1).ToString();
        mapSensitivityMultiplier = (float)Math.Round(MapSensitivitySlider.value, 1);
        PlayerPrefs.SetFloat("MapSensitivity", MapSensitivitySlider.value);
    }

    public void SetWaitTime()
    {
        waitTimeValue.text = Math.Round(WaitTimeMultiplier.value, 1).ToString();
        UserTimelineSpeedDelay = 100 * (float)Math.Round(WaitTimeMultiplier.value, 1);
        PlayerPrefs.SetFloat("WaitTime", WaitTimeMultiplier.value);
    }

    public void SetTextSpeed()
    {
        TextSpeedvalue.text = Math.Round(TextSpeedslider.value, 1).ToString();
        textSpeedMultiplier = (float)Math.Round(TextSpeedslider.value, 1);
        PlayerPrefs.SetFloat("TextSpeed", TextSpeedslider.value);
    }

    public void SetMasterVolume()
    {
        float volume = MasterVolumeSlider.value;
        MasterVolumeSliderValue.text = Math.Round(MasterVolumeSlider.value * 100, 0).ToString();
        AudioManager.Instance.mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        SFXSliderValue.text = Math.Round(SFXSlider.value * 100, 0).ToString();
        AudioManager.Instance.mixer.SetFloat("SFXVolume", Mathf.Log10(SFXSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = MusicSlider.value;
        MusicSliderValue.text = Math.Round(MusicSlider.value * 100, 0).ToString();
        AudioManager.Instance.mixer.SetFloat("MusicVolume", Mathf.Log10(MusicSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void Move(bool moveUp)
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            quitButton.gameObject.SetActive(true);
        }
        else
        {
            quitButton.gameObject.SetActive(false);
        }

        if (moveUp)
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(SettingsMenu.gameObject.GetComponent<RectTransform>(), 0, 0, 0.01f);
            StartCoroutine(generalCoruntine);
            blackScreen.transform.SetAsFirstSibling();
            Tools.ToggleUiBlocker(false);
            SettingsMenuDisabled = false;
            canvas.sortingOrder = 10;
            AudioManager.QuickPlay("ui_woosh_001");
            if (BattleSystem.Instance != null && BattleSystem.Instance.state == BattleStates.BATTLE)
            {
                CombatTools.PauseStaminaTimer();
            }
        }
        else
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(SettingsMenu.gameObject.GetComponent<RectTransform>(), 0, -2000, 0.01f);
            StartCoroutine(generalCoruntine);
            Tools.ToggleUiBlocker(true);
            SettingsMenuDisabled = true;
            canvas.sortingOrder = 10;
            AudioManager.QuickPlay("ui_woosh_001");
            if (BattleSystem.Instance != null && BattleSystem.Instance.state == BattleStates.BATTLE)
            {
                CombatTools.UnpauseStaminaTimer();
            }
        }

    }

    public void Load(string SceneToLoad, string MusicToPlay, float MusicFadeInTime = 1, float TargetVolume = 1)
    {
        StartCoroutine(DoLoad(SceneToLoad, MusicToPlay, MusicFadeInTime, TargetVolume));
    }

    public IEnumerator DoLoad(string SceneToLoad, string MusicToPlay, float MusicFadeTime = 1, float TargetVolume = 1)
    {
        OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
        OptionsManager.Instance.CanPause = false;
        Move(false);
        blackScreen.gameObject.SetActive(true);
        canvas.sortingOrder = 100;
        StartCoroutine(Tools.FadeObject(blackScreen, 0.001f, true));

        foreach(var musicTrack in AudioManager.Instance.sounds)
        {
            if(musicTrack.type.name == "Music")
            {
                StartCoroutine(AudioManager.Instance.Fade(0, musicTrack.AudioName, 1f, true));
            }
        }
       
        yield return new WaitUntil(() => blackScreen.color == new Color(0, 0, 0, 1));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneToLoad);
        AudioManager.Instance.Play(MusicToPlay);
        StartCoroutine(AudioManager.Instance.Fade(TargetVolume, MusicToPlay, MusicFadeTime, false));
        canvas.sortingOrder = 2;
    }
}
