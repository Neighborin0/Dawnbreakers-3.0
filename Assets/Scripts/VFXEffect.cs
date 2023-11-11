using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public  class VFXEffect : MonoBehaviour
{

    void SetDone()
    {
      GetComponent<Animator>().SetBool("Done", true);
        print("HELLO SET DONE IS BEING RUN!!!!");
    }
  
}
