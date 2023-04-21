using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class CharacterSlot : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI healthNumbers;
    public TextMeshProUGUI stats;
    public Slider slider;
    public Unit unit;
   
    public void DisplayDetailedCharacterTab()
    {
        Director.Instance.DisplayCharacterTab(false);
    }

}
