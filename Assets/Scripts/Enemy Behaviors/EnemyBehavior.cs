using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    public virtual void DoBehavior(Unit baseUnit) {}
}
