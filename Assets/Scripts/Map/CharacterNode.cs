using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterNode : MapNode
{
    public override void OnInteracted()
    {
        SceneManager.LoadScene(2);
    }

}