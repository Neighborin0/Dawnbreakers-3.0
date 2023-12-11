using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PipCounter : MonoBehaviour
{

    public int pipCount = 0;
    public int maxPips = 3;
    public GridLayoutGroup pipGrid;
    public GameObject pipPrefab;

    public void AddPip()
    {
        if (pipCount < maxPips)
        {
            pipCount++;
            Instantiate(pipPrefab, pipGrid.transform);
        }

    }

    public void TakePip()
    {
        if (pipCount > 0)
        {
            pipCount--;
            foreach (Transform pip in pipGrid.transform)
            {
                Destroy(pip.gameObject);
                break;
            }
        }

    }

    public void ResetPips()
    {
        pipCount = 1;
        foreach (Transform pip in pipGrid.transform)
        {
            Destroy(pip.gameObject);
        }
        Instantiate(pipPrefab, pipGrid.transform);
    }


}
