using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeLineChild : MonoBehaviour
{
    public Image portrait;
    public Unit unit;
    public bool CanMove = true;
    public RectTransform rectTransform;
    public bool CanClear = false;
    public TextMeshProUGUI num;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        if(CanMove)
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, new Vector3(unit.stamina.slider.value * -15.74f, rectTransform.anchoredPosition.y), Director.Instance.timelinespeedDelay);
    }
}
