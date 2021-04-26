using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeAway : MonoBehaviour
{
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    private void Start()
    {
        _img.DOFade(0f, 0.5f).Play();
    }
}
