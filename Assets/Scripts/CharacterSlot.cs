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
    public TextMeshProUGUI ATKtext;
    public TextMeshProUGUI DEFtext;
    public Slider slider;
    public Unit unit;
   
    public void ResetStats()
    {
        ATKtext.text = $"<sprite name=\"ATK\">:{unit.attackStat}";
        DEFtext.text = $"<sprite name=\"DEF\">:{unit.defenseStat}";
        //SPDtext.text = $"<sprite name=\"SPD\">:{unit.speedStat}";
        healthNumbers.text = $"{unit.currentHP} / {unit.maxHP}";
    }

}
