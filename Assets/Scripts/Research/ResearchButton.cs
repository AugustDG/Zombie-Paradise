using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Utility;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class ResearchButton : MonoBehaviour
{
    // 0: researched sprite
    // 1: too expensive sprite
    // 2: unlocked sprite
    // 3: hovered/locked sprite
    public Sprite[] researchSprites;

    public ResearchState RState
    {
        get => rState;
        set
        {
            rState = value;
            ResearchStateChanged();
        }
    }
    
    [SerializeField] private ResearchState rState;
    [ReadOnly] [SerializeField] private CustomButton butt;
    [ReadOnly] [SerializeField] private Shadow shadow;

    public List<ResearchButton> dependentButts;

    [SerializeField] private ResearchType rType;

    private void OnValidate()
    {
        AssignReferences();
        
        RState = rState;
    }

    private void Awake()
    {
        AssignReferences();

        butt.OnPointerEnterEvent += OnPointerEnterEvent;
        butt.OnPointerExitEvent += OnPointerExitEvent;
        butt.OnPointerDownEvent += OnPointerDownEvent;
        butt.OnPointerUpEvent += OnPointerUpEvent;

        MapEvents.CurrencyChangedEvent += BrainsAddedHandler;
        
        ResearchStateChanged();
    }
    
    private void AssignReferences()
    {
        GetComponent<Animator>().SetFloat("Offset", Random.value * 2f);
        
        butt = GetComponentInChildren<CustomButton>();
        shadow = butt.GetComponent<Shadow>();
    }

    private void BrainsAddedHandler(object sender, CurrencyType args)
    {
        if (args != CurrencyType.BrainsAdded) return;
        
        RefreshResearchState();
    }

    public void OnButtonClicked()
    {
        if (RState == ResearchState.Unlocked)
            MapData.ResearchManagerRef.OnResearchClick.Invoke(this, rType);
    }

    private void OnPointerEnterEvent(object sender, PointerEventData eventData)
    {
        if (rState == ResearchState.Unlocked)
            butt.image.sprite = researchSprites[3];
    }

    private void OnPointerExitEvent(object sender, PointerEventData eventData)
    {
        ResearchStateChanged();
        //butt.image.sprite = researchSprites[2];
    }

    private void OnPointerDownEvent(object sender, PointerEventData eventData)
    {
        if (rState == ResearchState.Unlocked)
        {
            shadow.enabled = false;
            butt.image.sprite = researchSprites[3];   
        }
    }

    private void OnPointerUpEvent(object sender, PointerEventData eventData)
    {
        if (rState == ResearchState.Unlocked)
        {
            ResearchStateChanged();
            shadow.enabled = true;
        }
    }

    public void RefreshResearchState()
    {
        if(!Application.isPlaying) return;
        
        if (!(RState == ResearchState.Unlocked || RState == ResearchState.TooExpensive)) return;

        if (MapData.ResearchManagerRef.ResearchCostDictionary.TryGetValue(rType, out var cost))
        {
            RState = cost > MapData.BrainAmount ? ResearchState.TooExpensive : ResearchState.Unlocked;
        }
    }
    
    private void ResearchStateChanged()
    {
        switch (RState)
        {
            case ResearchState.Researched:
                butt.image.sprite = researchSprites[0];
                break;
            case ResearchState.TooExpensive:
                butt.image.sprite = researchSprites[1];
                break;
            case ResearchState.Unlocked:
                butt.interactable = true;
                butt.image.color = new Color(1f, 1f, 1, 1f);
                butt.image.sprite = researchSprites[2];
                break;
            case ResearchState.Locked:
                butt.interactable = false;
                butt.image.color = new Color(1f, 1f, 1f, 0.5f);
                butt.image.sprite = researchSprites[3];
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum ResearchState
{
    Researched,
    TooExpensive,
    Unlocked,
    Locked,
}