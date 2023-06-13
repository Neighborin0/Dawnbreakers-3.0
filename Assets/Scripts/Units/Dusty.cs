using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dusty : Unit
{

    private List<string> intro = new List<string>
   {
        "yo.",
   };

    public override void Intro()
    {
        introText = intro;
    }
    void Awake()
    {
        unitName = "Dusty";
        maxHP = 40;
        attackStat = 5;
        defenseStat = 8;
        speedStat = 7;
        StartingStamina = UnityEngine.Random.Range(80, 90);
        currentHP = maxHP;
        IsPlayerControlled = true;
    }

  


}
