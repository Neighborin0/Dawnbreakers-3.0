using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.Rendering;


public class MiniMapIcon : MonoBehaviour
{
    public Unit unit;
    public SpriteRenderer mapIcon;
    public List<Sprite> mapIcons;
    private IEnumerator mover;
    public int animationStep;
    public float fpsCounter;
    public float fps = 2;
    public enum MapIconState { IDLE, MOVING, JUMPING, STATIC }
    public MapIconState state;
    private void Awake()
    {
        mapIcon = GetComponent<SpriteRenderer>();
        state = MapIconState.IDLE;
    }

    public IEnumerator Move(float transformPointX, float transformPointY, float transformPointZ)
    {
        yield return new WaitUntil(() => state == MapIconState.IDLE);
        state = MapIconState.MOVING;
        if (mover != null)
        {
            StopCoroutine(mover);
        }
        mover = Tools.SmoothMoveObject(gameObject.transform, transformPointX, transformPointY, 0.01f, false, 0, 10);
        StartCoroutine(mover);
        yield return new WaitForSeconds(0.5f);
        state = MapIconState.IDLE;

    }

    private void Update()
    {
        if (mapIcons != null)
        {
            fpsCounter += Time.deltaTime;
            if (state == MapIconState.JUMPING)
            {
                if (mover != null)
                {
                    StopCoroutine(mover);
                }

            }
            if (fpsCounter >= 1f / fps)
            {
                animationStep++;
                if (animationStep == mapIcons.Count)
                {
                    animationStep = 0;
                }
                mapIcon.sprite = mapIcons[animationStep];
                fpsCounter = 0f;

            }
        }

    }

}
