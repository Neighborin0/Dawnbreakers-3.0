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
        attackStat = 100;
        defenseStat = 0;
        speedStat = 0;
        currentHP = maxHP;
        IsPlayerControlled = false;
        StartingStamina = 1;
        behavior = this.gameObject.AddComponent<RandomEnemyBehavior>();
        Tools.AddItemToInventory(this, "Iron Shield");
    }

}
