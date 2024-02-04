using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class FeedbackButton : MonoBehaviour
{
  public void OpenWebsite(string url = "https://forms.gle/M6t7p1Cw5vD2kH1P6")
    {
        Application.OpenURL("https://forms.gle/M6t7p1Cw5vD2kH1P6");
    }
}
