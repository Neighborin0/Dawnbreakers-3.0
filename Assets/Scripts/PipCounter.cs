using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PipCounter : MonoBehaviour
{

    public int pipCount = 0;
    public int maxPips = 3;
    public TextMeshProUGUI pipText;

    public void AddPip()
    {
        if (pipCount < maxPips)
            pipCount++;

        pipText.text = pipCount.ToString();
    }

    public void TakePip()
    {
        if (pipCount > 0)
            pipCount--;


        pipText.text = pipCount.ToString();
    }

    public void ResetPips()
    {
        pipCount = 1;
        pipText.text = pipCount.ToString();
    }


}
