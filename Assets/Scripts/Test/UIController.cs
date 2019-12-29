using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPonterDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
    }


}
