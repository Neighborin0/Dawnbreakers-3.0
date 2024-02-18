using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueHandler", menuName = "Assets/DialogueHandler")]

public class DialogueHandler : ScriptableObject
{
    public List<LabLine> labLines;

    public List<LabLine> LabLines => labLines;

   
}

