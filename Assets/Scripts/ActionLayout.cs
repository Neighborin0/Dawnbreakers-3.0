using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class ActionLayout : MonoBehaviour
{
    private IEnumerator generalCoruntine;
    private IEnumerator fadeCoruntine;
    public Vector3 origPos;
    public Color groupColor;


    public void Move(bool moveIn)
    {
        if (moveIn)
        {
         

            if (fadeCoruntine != null)
                StopCoroutine(fadeCoruntine);

            fadeCoruntine = Fade(moveIn);
            StartCoroutine(fadeCoruntine);

        }
        else
        {
            if (fadeCoruntine != null)
                StopCoroutine(fadeCoruntine);

            fadeCoruntine = Fade(moveIn);
            StartCoroutine(fadeCoruntine);
        }

    }

    public IEnumerator Fade(bool moveIn)
    {
        if (moveIn)
        {
            while (groupColor.a != 1 && moveIn)
            {
                groupColor += new Color(0, 0, 0, 0.1f);
                yield return new WaitForSeconds(0.01f);
            }
            yield break;
        }
        else
        {
            while (groupColor.a != 0 && !moveIn)
            {
                groupColor -= new Color(0, 0, 0, 0.1f);
                yield return new WaitForSeconds(0.01f);

            }
            yield break;
        }

    }

}
