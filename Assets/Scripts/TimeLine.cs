using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TimeLine : MonoBehaviour
{
    public TimeLineChild borderChildprefab;
    public Transform endpoint;
    public Transform startpoint;
    public float minPos;
    public float maxPos;
    public List<TimeLineChild> children;
    public Slider slider;
    public bool Paused = true;
    public bool Resetting = false;

    public void Start()
    {
        slider.value = 0;
    }


    void Update()
    {
        if (!Paused)
        {
            if (slider.value < slider.maxValue && !Resetting)
                slider.value += Time.deltaTime * OptionsManager.Instance.UserTimelineSpeedDelay;
            else if(slider.value > 0 && Resetting)
                slider.value -= Time.deltaTime * OptionsManager.Instance.UserTimelineSpeedDelay * 2f;
        }
    }
    //In Battle
    public IEnumerator ResetTimeline()
    {
        Resetting = true;
        yield return new WaitUntil(() => slider.value <= 0);
        Resetting = false;
        Paused = true;
    }
    //Post Battle
    public void RefreshTimeline()
    {
        Paused = true;
        slider.value = 0;
        foreach (var unit in Tools.GetAllUnits())
        {
            unit.state = PlayerState.IDLE;
        }

    }
    public TimeLineChild SpawnTimelineChild(Unit unit)
    {
        print("Timeline child has been spawned");
        var TL = Instantiate(borderChildprefab, startpoint);
        children.Add(TL);
        TL.portrait.sprite = unit.charPortraits[0];
        TL.unit = unit;
        unit.timelinechild = TL;
        TL.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        TL.value = slider.maxValue;
        return TL;
    }

    public void RemoveTimelineChild(Unit unit)
    {
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child.unit.unitName == unit.unitName)
            {
                Director.Instance.timeline.children.Remove(child);
                Destroy(child.gameObject);
                break;
            }
        }
        foreach (var action in BattleSystem.Instance.ActionsToPerform)
        {
            if (action.unit.unitName == unit.unitName)
            {
                BattleSystem.Instance.ActionsToPerform.Remove(action);
                break;
            }
        }
    }
    public void DoCost(float cost, Unit unit)
    {
        var TL = SpawnTimelineChild(unit);
        TL.value -= cost * unit.actionCostMultiplier < 100 ? cost * unit.actionCostMultiplier : 100;
    }
}
