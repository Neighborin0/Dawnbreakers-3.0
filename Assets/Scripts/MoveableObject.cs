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
    IEnumerator generalCoroutine;
    public bool scaleWithScreenSize = false;
    public void Move(bool moveUp, float delay = 0.01f, float timeDivider = 1)
    {
        Stop();

        float targetX, targetY;

        if (scaleWithScreenSize)
        {
            Vector2 referenceResolution = new Vector2(1920f, 1080f); // Reference resolution
            float aspectRatio = referenceResolution.x / referenceResolution.y;
            float currentAspectRatio = Screen.width / (float)Screen.height;

            if (currentAspectRatio > aspectRatio) // Screen is wider than reference
            {
                float scale = Screen.height / referenceResolution.y;
                targetY = moveUp ? PositionUpY * scale : PositionDownY * scale;
                targetX = (moveUp ? PositionUpX : PositionDownX) * scale;
            }
            else // Screen is narrower or equal to reference
            {
                float scale = Screen.width / referenceResolution.x;
                targetX = (moveUp ? PositionUpX : PositionDownX) * scale;
                targetY = moveUp ? PositionUpY * scale : PositionDownY * scale;
            }
        }
        else
        {
            targetX = moveUp ? PositionUpX : PositionDownX;
            targetY = moveUp ? PositionUpY : PositionDownY;
        }

        if (gameObject.GetComponent<RectTransform>() != null)
            generalCoroutine = Tools.SmoothMoveUI(gameObject.GetComponent<RectTransform>(), targetX, targetY, delay);
        else
            generalCoroutine = Tools.SmoothMoveObject(gameObject.transform, targetX, targetY, delay, false, 0, 10000, timeDivider);

        StartCoroutine(generalCoroutine);
        moved = !moveUp;
    }

    public void Stop()
    {
        if (generalCoroutine != null)
            StopCoroutine(generalCoroutine);
    }
}
