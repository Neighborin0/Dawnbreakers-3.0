using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    public TimeLineChild borderChildprefab;
    public Transform endpoint;
    public Transform startpoint;
    public List<TimeLineChild> children;
    IEnumerator generalCoruntine;
    public void Move(bool moveDown)
    {

        if (moveDown)
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), 0, 425, 0.01f);
            StartCoroutine(generalCoruntine);
        }
        else
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), 0, 692, 0.01f);
            StartCoroutine(generalCoruntine);
        }

    }
}
