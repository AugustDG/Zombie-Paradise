using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Multiline] public string msg;

    [SerializeField] private Vector2 offset = new Vector2(35, 75);

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CustomTooltip.Instance.ShowTooltip(_rectTransform.anchoredPosition + offset, msg);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        CustomTooltip.Instance.HideTooltip();
    }
}
