using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent upEvent, downEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        downEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        upEvent?.Invoke();
    }

    
}
