using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LabBillboard : MonoBehaviour
{
    public Vector3 rotation = new Vector3 (0f, 180, 0f);
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(rotation);
    }
}
