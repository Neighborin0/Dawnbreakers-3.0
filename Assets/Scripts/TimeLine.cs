using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeLine : MonoBehaviour
{
    public TimeLineChild borderChildprefab;
    public Transform endpoint;
    public Transform startpoint;
    public float minPos;
    public float maxPos;
    public List<TimeLineChild> children;
    IEnumerator generalCoruntine;
    public void Move(bool moveDown)
    {

        if (moveDown)
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), 0, maxPos, 0.01f);
            StartCoroutine(generalCoruntine);
        }
        else
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(this.gameObject.GetComponent<RectTransform>(), 0, minPos, 0.01f);
            StartCoroutine(generalCoruntine);
        }

    }
}
