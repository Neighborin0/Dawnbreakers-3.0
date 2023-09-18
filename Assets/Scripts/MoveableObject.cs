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
    IEnumerator generalCoruntine;
    public void Move(bool moveUp, bool retainXpos = false, bool retainYpos = false)
    {
        /*if(retainXpos)
        {
            PositionUpX = this.gameObject.transform.position.x;
            PositionDownX = this.gameObject.transform.position.x;
        }
        if (retainYpos)
        {
            PositionUpY = this.gameObject.GetComponent<RectTransform>().position.y;
            PositionDownY = this.gameObject.GetComponent<RectTransform>().position.y;
        }
        */

            if (moveUp)
            {
                if (generalCoruntine != null)
                    StopCoroutine(generalCoruntine);

                generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), PositionUpX, PositionUpY, 0.01f);
                StartCoroutine(generalCoruntine);
            }
            else
            {
                if (generalCoruntine != null)
                    StopCoroutine(generalCoruntine);

                generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), PositionDownX, PositionDownY, 0.01f);
                StartCoroutine(generalCoruntine);
            }

    }
}
