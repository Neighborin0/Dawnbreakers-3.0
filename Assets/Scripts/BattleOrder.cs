using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleOrder : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public IEnumerator TransitionUIOut(float DistanceToMove)
    {
        for (int i = 0; i < 60; i++)
        {
            BattleOrder[] battleorder = GameObject.FindObjectsOfType<BattleOrder>();
            foreach (var order in battleorder)
            {
                order.transform.position = new Vector3(order.transform.position.x, order.transform.position.y + DistanceToMove, order.transform.position.z);
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator TransitionIn(float DistanceToMove)
    {
        for (int i = 0; i < 60; i++)
        {
            BattleOrder[] battleorder = GameObject.FindObjectsOfType<BattleOrder>();
            foreach (var order in battleorder)
            {
                order.transform.position = new Vector3(order.transform.position.x, order.transform.position.y - DistanceToMove, order.transform.position.z);
            }
            yield return new WaitForSeconds(0.001f);
        }
    }
}
