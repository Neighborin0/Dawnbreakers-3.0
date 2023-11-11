using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;


public class FloatingObject : MonoBehaviour
{
    [SerializeField]
    private bool DoneMovingUp = false;
    [SerializeField]
    private float amountToSway = 0.5f;
    [SerializeField]
    private float TimeDivider = 100f;
    public float smoothingTime = 0.5f;

    [SerializeField]
    private Vector3 RotationVectors;
    [SerializeField]
    private float RotationSpeed = 0f;

    Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.position;
    }
    private void LateUpdate()
    {
        smoothingTime += Time.deltaTime;
        if (!DoneMovingUp)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, originalPos.y + amountToSway, originalPos.z), smoothingTime / TimeDivider);
            if (transform.position.y > originalPos.y + amountToSway - .1f)
            {
                DoneMovingUp = true;
                smoothingTime = 0f;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, originalPos.y - amountToSway * 2, originalPos.z), smoothingTime / TimeDivider);
            if (transform.position.y < originalPos.y - amountToSway * 2 + .1f)
            {
                DoneMovingUp = false;
                smoothingTime = 0f;
            }
        }
        this.transform.Rotate(RotationVectors * (RotationSpeed * Time.deltaTime));
    }



}
