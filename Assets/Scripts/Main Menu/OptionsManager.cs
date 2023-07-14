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
    public FPSCounter fPSCounter;
    public bool SettingsMenuDisabled;
    public Image blackScreen;
    public Button quitButton;
    public Button Continue;
    public Canvas canvas;
    public int IntensityLevel = 0; 
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
            StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
            if(PlayerPrefs.GetString("Level") != null)
            {
                Continue.gameObject.SetActive(true);
            }
#endif
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
            string option = resolutions[i].width + "x" + resolutions[i].height;
            resoultionparams.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResIndex = i;
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
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && fPSCounter != null)
        {
            FPSCounterEnable();
        }
        if (Input.GetKeyDown(KeyCode.Tab) && SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (BattleSystem.Instance != null && Director.Instance != null && BattleSystem.Instance.CheckPlayableState())
            {
                if (SettingsMenuDisabled)
                {
                    Director.Instance.previousCameraState = LabCamera.Instance.state;
                    LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                    Tools.PauseAllStaminaTimers();
                    //Time.timeScale = 0;
                }
                else
                {
                    //Time.timeScale = 1;
                    LabCamera.Instance.state = Director.Instance.previousCameraState;
                    if(!Tools.CheckIfAnyUnitIsDeciding())
                    {
                        Tools.UnpauseAllStaminaTimers();
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
            blackScreen.transform.SetAsFirstSibling();
            Tools.ToggleUiBlocker(false);
            SettingsMenuDisabled = false;
            canvas.sortingOrder = 10;
        }
        else
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(SettingsMenu.gameObject.GetComponent<RectTransform>(), 0, -1215, 0.01f);
            StartCoroutine(generalCoruntine);
            Tools.ToggleUiBlocker(true);
            SettingsMenuDisabled = true;
            canvas.sortingOrder = 1;
        }

    }

    public void Load(string SceneToLoad)
    {
        StartCoroutine(DoLoad(SceneToLoad));
    }

    public IEnumerator DoLoad(string SceneToLoad)
    {
        Move(false);
        blackScreen.gameObject.SetActive(true);
        canvas.sortingOrder = 100;
        StartCoroutine(Tools.FadeObject(blackScreen, 0.001f, true));
        if (SceneToLoad != "Main Menu")
        {
            quitButton.gameObject.SetActive(true);
        }
        else
        {
            quitButton.gameObject.SetActive(false);
        }
        yield return new WaitUntil(() => blackScreen.color == new Color(0, 0, 0, 1));
        yield return new WaitForSeconds(1f);
        print("TRANSITIONED");
        SceneManager.LoadScene(SceneToLoad);
        canvas.sortingOrder = 1;
    }
}
