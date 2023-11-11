using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class MoveableObject : MonoBehaviour
{
    public float PositionUpX;
    public float PositionUpY;
    public float PositionDownX;
    public float PositionDownY;
    public bool moved = false;
    IEnumerator generalCoruntine;
    public void Move(bool moveUp, float delay = 0.01f, float TimeDivder = 1)
    {

        if (moveUp)
        {
            Stop();

            if (this.gameObject.GetComponent<RectTransform>() != null)
                generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), PositionUpX, PositionUpY, delay);
            else
                generalCoruntine = Tools.SmoothMoveObject(this.gameObject.transform, PositionUpX, PositionUpY, delay, false, 0, 10000, TimeDivder);
            StartCoroutine(generalCoruntine);
            moved = false;
        }
        else
        {
            Stop();

            if (this.gameObject.GetComponent<RectTransform>() != null)
                generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), PositionDownX, PositionDownY, delay);
            else
                generalCoruntine = Tools.SmoothMoveObject(this.gameObject.transform, PositionUpX, PositionUpY, delay, false, 0, 10000, TimeDivder);
            StartCoroutine(generalCoruntine);
            moved = true;
        }

    }

    public void Stop()
    {
        if (generalCoruntine != null)
            StopCoroutine(generalCoruntine);
    }
}
