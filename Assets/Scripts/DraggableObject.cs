using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Image image;
    public bool CanBeDragged = true;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(CanBeDragged)
        {
            originalParent = transform.parent;
            transform.SetParent(Director.Instance.canvas.transform, true);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CanBeDragged)
        {
            this.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CanBeDragged)
        {
            transform.SetParent(originalParent, true);
            image.raycastTarget = true;
        }
    }
}
