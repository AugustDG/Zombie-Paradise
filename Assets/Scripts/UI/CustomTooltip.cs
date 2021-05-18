using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CustomTooltip : MonoBehaviour
{
    public static CustomTooltip Instance;
    
    private CanvasGroup _backGround;
    private TMP_Text _text;
    private RectTransform _rectTransform;
    
    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        _backGround = GetComponentInChildren<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();

        _backGround.alpha = 0f;

        Instance = this;
    }

    public void ShowTooltip(Vector2 anchoredPos, string text = "")
    {
        if (text != "")
            _text.text = text;

        _rectTransform.anchoredPosition = anchoredPos;
        
        _backGround.DOFade(1f, 0.2f).Play();
    }
    
    public void HideTooltip()
    {
        _backGround.DOFade(0f, 0.2f).Play();
    }
}
