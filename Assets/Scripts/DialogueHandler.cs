using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueHandler", menuName = "Assets/DialogueHandler")]

public class DialogueHandler : ScriptableObject
{
    [SerializeField] private List<LabLine> labLines;

    public List<LabLine> LabLines => labLines;
}

