using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;

public class DefendEF : EffectIcon
{
    public override string GetDescription()
    {
        description = $"+{storedValue}  <sprite name=\"DEF BLUE\">";
        return description;
    }
}
