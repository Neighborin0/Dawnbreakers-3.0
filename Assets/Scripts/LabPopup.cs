using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class LabPopup : MonoBehaviour
{
    public IEnumerator scaler;
    public IEnumerator dropper;
    public IEnumerator Pop()
    {
        scaler = Tools.SmoothScaleObj(transform, new Vector3(2, 2, 0), 0.001f);
        StartCoroutine(scaler);
        yield return new WaitForSeconds(0.2f);
        StopCoroutine(scaler);
        scaler = Tools.SmoothScaleObj(transform, new Vector3(1, 1, 0), 0.001f);
        StartCoroutine(scaler);
    }

    public IEnumerator Rise()
    {
        transform.localScale = Vector3.one;
        dropper = Tools.SmoothMoveObject(transform, transform.position.x, transform.position.y + 1, 0.001f);
        StartCoroutine(dropper);
        yield break;
    }
    public IEnumerator RiseAndDrop()
    {
        transform.localScale = Vector3.one;
        dropper = Tools.SmoothMoveObject(transform, transform.position.x, transform.position.y + 2, 0.001f);
        StartCoroutine(dropper);
        yield return new WaitForSeconds(0.3f);
        StopCoroutine(dropper);
        dropper = Tools.SmoothMoveObject(transform, transform.position.x, transform.position.y - 2, 0.001f);
        StartCoroutine(dropper);
    }
    public IEnumerator DestroyPopUp(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        var number = this.GetComponentInChildren<TextMeshProUGUI>();
        if(scaler != null)
        StopCoroutine(scaler);
        if (dropper != null)
            StopCoroutine(dropper);
        while (number.color.a > 0)
        {
            if (number != null)
            {
                number.color -= new Color(0, 0, 0, 0.1f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        Destroy(this.gameObject);
        yield break;
    }
}
