using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterNode : MapNode
{
    public void Transition()
    {
        SceneManager.LoadScene(2);
    }

}
