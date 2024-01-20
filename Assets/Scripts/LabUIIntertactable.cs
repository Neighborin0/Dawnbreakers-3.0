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

    [SerializeField]
    private UltEvents.UltEvent Entered;
    [SerializeField]
    private UltEvents.UltEvent Exited;
    [SerializeField]
    private UltEvents.UltEvent Clicked;


    public bool CanClick = true;
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

            if (CanClick)
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
        if (Entered.PersistentCallsList.Count == 0)
        {
            if (GetComponent<Button>() != null && GetComponent<Button>().interactable || GetComponent<TMP_Dropdown>() != null && GetComponent<TMP_Dropdown>().interactable || GetComponent<Slider>() != null && GetComponent<Slider>().interactable)
            {
                DoEnter("button_hover");
            }
            else
            {
                DoEnter("button_hover");
            }
        }
        else
        {
            Entered.Invoke();
        }
    }

    public void DoEnter(string AudioToReturn)
    {
        PlayAudio(AudioToReturn);
    }
    public void TriggerExit(BaseEventData baseEventData)
    {
        if (Exited.PersistentCallsList.Count == 0)
        {
            DoExit(string.Empty);
        }
        else
        {
            Exited.Invoke();
        }
    }

    public void DoExit(string AudioToReturn)
    {

    }
    public void TriggerClick(BaseEventData baseEventData)
    {
        if (Clicked.PersistentCallsList.Count == 0)
        {
            if (GetComponent<Button>() != null && GetComponent<Button>().IsInteractable())
            {
                DoClick("button_Hit_001");
            }
            else if (GetComponent<Button>() == null)
            {
                DoClick("button_Hit_001");
            }
        }
        else
        {
            Clicked.Invoke();
        }

    }

    public void DoClick(string AudioToReturn)
    {
        PlayAudio(AudioToReturn);
    }


    private void PlayAudio(string AudioToReturn)
    {
        AudioManager.QuickPlay(AudioToReturn);
    }
   


}
