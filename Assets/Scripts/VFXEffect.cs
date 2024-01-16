using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public  class VFXEffect : MonoBehaviour
{

    void SetDone()
    {
      GetComponent<Animator>().SetBool("Done", true);
    }

    public void PlayAudio(string AudioName)
    {
        AudioManager.QuickPlay(AudioName);
    }
}
