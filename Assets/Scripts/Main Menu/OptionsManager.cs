using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }
    public Image SettingsMenu;
    public IEnumerator generalCoruntine;
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionsDropdown;
    public TMPro.TMP_Dropdown vsynDropdown;
    public TMPro.TMP_Dropdown fullScreenDropdown;
    public TMPro.TMP_Dropdown QualityDropdown;
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
        }

    }

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> resoultionparams = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resoultionparams.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(resoultionparams);
        resolutionsDropdown.value = currentResIndex;
        resolutionsDropdown.RefreshShownValue();
        vsynDropdown.value = QualitySettings.vSyncCount;
       if(Screen.fullScreen)
        {
            fullScreenDropdown.value = 0;
        }
       else
            fullScreenDropdown.value = 1;
        QualityDropdown.value = QualitySettings.GetQualityLevel();
    }


    public void SetResolution(int resolutionIndex)
    {
       var resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVsync(int vsyncIndex)
    {
        QualitySettings.vSyncCount = vsyncIndex;
    }

    public void SetQualitySettings(int qualityindex)
    {
        QualitySettings.SetQualityLevel(qualityindex);
        print(QualitySettings.GetQualityLevel());
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
    public void Move(bool moveUp)
    {
        if (moveUp)
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(SettingsMenu.gameObject.GetComponent<RectTransform>(), 0, 0, 0.01f);
            StartCoroutine(generalCoruntine);
        }
        else
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(SettingsMenu.gameObject.GetComponent<RectTransform>(), 0, -1215, 0.01f);
            StartCoroutine(generalCoruntine);
        }

    }
}
