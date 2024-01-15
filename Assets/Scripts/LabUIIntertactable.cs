using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;
using System.Reflection;

public class LabUIInteractable : MonoBehaviour
{
    private EventTrigger eventTrigger;

    public UnityEvent Entered;
    public UnityEvent Exited;
    public UnityEvent Clicked;

    public bool HasClicked = true;
    public void Start()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();

            Entry enterEvent = new Entry()
            {
                eventID = EventTriggerType.PointerEnter,
            };
            enterEvent.callback.AddListener(TriggerEnter);
            eventTrigger.triggers.Add(enterEvent);


            Entry exitEvent = new Entry()
            {
                eventID = EventTriggerType.PointerExit,
            };
            exitEvent.callback.AddListener(TriggerExit);
            eventTrigger.triggers.Add(exitEvent);

            if (HasClicked)
            {
                Entry clickEvent = new Entry()
                {
                    eventID = EventTriggerType.PointerClick,
                };
                clickEvent.callback.AddListener(TriggerClick);
                eventTrigger.triggers.Add(clickEvent);
            }

        }
    }


    public void TriggerEnter(BaseEventData baseEventData)
    {
        if (Entered.GetPersistentEventCount() == 0)
        {
            if (GetComponent<Button>() != null && GetComponent<Button>().interactable || GetComponent<TMP_Dropdown>() != null && GetComponent<TMP_Dropdown>().interactable || GetComponent<Slider>() != null && GetComponent<Slider>().interactable)
            {
                DoEnter(string.Empty);
            }
            else
            {
                DoEnter(string.Empty);
            }
        }
        else
        {
            print("finding persistent events");
        }
    }

    public void DoEnter(string AudioToReturn)
    {
        if (AudioToReturn == string.Empty)
        {
            AudioManager.QuickPlay("button_hover");
        }
        else
            AudioManager.QuickPlay(AudioToReturn);
    }
    public void TriggerExit(BaseEventData baseEventData)
    {
        if (Exited.GetPersistentEventCount() == 0)
        {
            DoExit(string.Empty);
        }
    }

    public void DoExit(string AudioToReturn)
    {

    }
    public void TriggerClick(BaseEventData baseEventData)
    {
        if (Clicked.GetPersistentEventCount() == 0)
        {
            DoClick(string.Empty);
        }
    }

    public void DoClick(string AudioToReturn)
    {
        if (AudioToReturn == string.Empty)
        {
            AudioManager.QuickPlay("button_Hit_001");
        }
        else
            AudioManager.QuickPlay(AudioToReturn);
    }



}
