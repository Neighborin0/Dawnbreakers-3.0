using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestNode : MapNode
{
    public override void OnInteracted()
    {
        Director.Instance.CharacterSlotEnable();
        OptionsManager.Instance.Load("Rest", "Rest_Coronus", 1, 0.5f);
        AudioManager.QuickPlay("button_Hit_003", true);
    }


}
