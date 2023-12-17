using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class Stalwart : EffectIcon
{
   
    public void Start()
    {
        iconName = "STALWART";
        TimedEffect = false;
    }
    public override string GetDescription()
    {
        description = $"Prevents next stun.";
        return description;
    }

}
