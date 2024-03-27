using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventNode : MapNode
{

    public override void OnInteracted()
    {
        Debug.LogError("It's Eventing Time");
    }

  
    
}
