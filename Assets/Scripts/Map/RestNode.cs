using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestNode : MapNode
{
    public void Transition()
    {
        DisableNode();
        Director.Instance.CharacterSlotEnable();
        StartCoroutine(Director.Instance.DoLoad("Rest"));
    }


}
