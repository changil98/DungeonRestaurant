using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.parent.GetComponentInParent<ScrollRect>().OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.parent.GetComponentInParent<ScrollRect>().OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.parent.GetComponentInParent<ScrollRect>().OnEndDrag(eventData);
    }
}
