using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.CanvasScaler;

public class TestDummy : Unit
{
    void Awake()
    {
        unitName = "TestDummy";
        maxHP = 100;
        attackStat = 0;
        defenseStat = 0;
        speedStat = 0;
        currentHP = maxHP;
        IsPlayerControlled = false;
        behavior = this.gameObject.AddComponent<RandomEnemyBehavior>();
    }

}
