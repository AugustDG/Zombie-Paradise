using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utility;

public class SetCurrencyValue : MonoBehaviour
{
    [SerializeField] private CurrencyType type;
    private TMP_Text _label;
    
    private void Awake()
    {
        _label = GetComponent<TMP_Text>();
        MapEvents.CurrencyChangedEvent += CurrencyChangedEvent;
    }
    private void CurrencyChangedEvent(object sender, EventArgs e)
    {
        _label.text = type == CurrencyType.Brains ? MapData.BrainAmount.ToString() : MapData.FingerAmount.ToString();
    }
}

public enum CurrencyType
{
    Fingers,
    Brains
}
