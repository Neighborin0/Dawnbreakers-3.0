using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class Vigor : EffectIcon
{
    public override string GetDescription()
    {
        description = $"+{storedValue} <sprite name=\"ATK RED2\">";
        return description;
    }
}
