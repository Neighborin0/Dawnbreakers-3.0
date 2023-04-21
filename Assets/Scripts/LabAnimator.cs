using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LabAnimator : Animator
{
    public bool Execute = false;

   
    public void PlayAction(string AnimationName, Unit unit)
    {
        var stateId = Animator.StringToHash(AnimationName);
        if (unit.anim != null)
        {
            if (unit.anim.HasState(0, stateId))
            {
                UnityEngine.Debug.Log(AnimationName);
                unit.anim.Play(AnimationName);
            }
            else
            {
                unit.anim.Play("Idle");
            }
        }

    }
       
}
