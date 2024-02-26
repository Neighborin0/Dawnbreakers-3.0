using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LabShaker : MonoBehaviour
{
    public bool IsShaking = false;
    public float shakeAmount = 0.5f;
    public Vector3 originalLocalPosition;
    public float increaseFactor = 1.8f;
    public void Shake()
    {
        originalLocalPosition = transform.localPosition;
        IsShaking = true;
    }

    public void LateUpdate()
    {
        if (IsShaking)
        {
            gameObject.transform.localPosition = originalLocalPosition + Random.insideUnitSphere * shakeAmount;
            shakeAmount += Time.deltaTime * increaseFactor;
        }
      
    }
}
