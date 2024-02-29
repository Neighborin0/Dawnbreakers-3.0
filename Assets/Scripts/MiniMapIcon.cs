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
    public float speed = 1;
    public enum MapIconState { IDLE, MOVING, JUMPING, STATIC }
    public MapIconState state;
    private void Awake()
    {
        mapIcon = GetComponent<SpriteRenderer>();
        state = MapIconState.IDLE;
        fps = 0;
    }



    public IEnumerator Move(float transformPointX, float transformPointY, float transformPointZ)
    {
        yield return new WaitUntil(() => state == MapIconState.IDLE);
        fps = 3;
       // GetComponent<Animator>().Play("Walk");
        state = MapIconState.MOVING;
        if (mover != null)
        {
            StopCoroutine(mover);
        }
        mover = Tools.SmoothMoveObject(gameObject.transform, transformPointX, gameObject.transform.position.y, 0.01f, false, 0, 10, 5);
        StartCoroutine(mover);
        yield return new WaitForSeconds(1.5f);
        state = MapIconState.IDLE;
        fps = 0;
        // GetComponent<Animator>().Play("Walk");

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
            /*if(state == MapIconState.MOVING)
            {
                float y = Mathf.PingPong(Time.time * speed, 0.5f);
                transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            }
            */
        }

    }

}
