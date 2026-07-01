using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class VectorTest : UnityEvent<Vector3> { }

[System.Serializable]
public struct LabLine
{
    public string text;
    public string expression;
    public string unit;
    public float textSpeed;
    public UnityEvent OnLineStarted;
    public UnityEvent OnLineEnded;
    public Vector3 PositionToMoveTo;
    public Vector3 CameraRotation;

}

