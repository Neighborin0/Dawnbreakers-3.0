using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TimeLine : MonoBehaviour
{
    public TimeLineChild borderChildprefab;
    public Transform endpoint;
    public Transform startpoint;
    public float minPos;
    public float maxPos;
    public List<TimeLineChild> children;
    public Slider slider;
    public bool Paused = false;

    void Update()
    {
        if (slider.value < slider.maxValue && !Paused)
        {
            slider.value += Time.deltaTime / OptionsManager.Instance.UserTimelineSpeedDelay;
        }
    }


}
