using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using System.Linq;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;

public class EffectPopUp : MonoBehaviour
{
    public void Start()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Camera.main.transform.rotation;
    }
   
}
