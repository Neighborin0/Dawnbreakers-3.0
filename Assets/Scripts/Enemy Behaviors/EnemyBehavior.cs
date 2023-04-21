using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    public virtual IEnumerator DoBehavior(Unit baseUnit) { yield break; }
}
