using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    public EventHandler<PointerEventData> OnPointerEnterEvent;
    public EventHandler<PointerEventData> OnPointerExitEvent;
    public EventHandler<PointerEventData> OnPointerDownEvent;
    public EventHandler<PointerEventData> OnPointerUpEvent;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(this, eventData);

        base.OnPointerEnter(eventData);
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(this, eventData);

        base.OnPointerEnter(eventData);
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke(this, eventData);

        base.OnPointerEnter(eventData);
    }
    
    public override void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke(this, eventData);

        base.OnPointerEnter(eventData);
    }
}
