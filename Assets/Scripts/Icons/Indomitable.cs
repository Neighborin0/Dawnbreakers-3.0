﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class Indomitable : EffectIcon
{
   
    void Start()
    {
        iconName = "INDOMITABLE";
        TimedEffect = false;

    }
    public override string GetDescription()
    {
        description = $"Prevents staggers and breaks.";
        return description;
    }

}
