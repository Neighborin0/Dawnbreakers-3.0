using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
    public void Move()
    {
        OptionsManager.Instance.Move(true);
    }
}
