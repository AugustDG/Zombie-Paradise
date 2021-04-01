using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Utility.Events;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioObject windObj;
    [SerializeField] private AudioObject graveyardObj;
    [SerializeField] private AudioObject mainAmbienceObj;
    [SerializeField] private AudioObject endAmbienceObj;
    [SerializeField] private AudioObject footstepObj;
    [SerializeField] private AudioObject brainObj;
    [SerializeField] private AudioObject fingerObj;

    private AudioObject _graveyardInstance;
    private AudioObject _mainInstance;
    private AudioObject _endInstance;
    private AudioObject _windInstance;

    private void Awake()
    {
        MapEvents.ChangedToGraveyardEvent += ChangedToGraveyardHandler;
        MapEvents.ChangedToTopViewEvent += ChangedToTopViewHandler;
        MapEvents.BrainAddedEvent += BrainAddedHandler;
        MapEvents.FingerAddedEvent += FingerAddedHandler;
        MapEvents.HumanKilledEvent += HumanKilledHandler;
        MapEvents.StepTakenEvent += StepTakenHandler;
    }
    private void StepTakenHandler(object sender, AudioEventArgs obj)
    {
        obj.AudioObject = Instantiate(footstepObj, (sender as MonoBehaviour)?.transform);
    }

    private void Start()
    {
        _mainInstance = Instantiate(mainAmbienceObj, transform);
        _graveyardInstance = Instantiate(graveyardObj, transform);
        _graveyardInstance.source.volume = 0;
        _windInstance = Instantiate(windObj, transform);
        _windInstance.source.volume = 0;
        _mainInstance.FadeIn(0.5f);
    }

    private void HumanKilledHandler(object sender, EventArgs e)
    {
        if (MapData.HumanList.Count <= 10 && _endInstance != null)
        {
            _mainInstance.FadeOut(2.5f);

            _endInstance = Instantiate(endAmbienceObj, transform);
            _endInstance.FadeIn(2f);
        } 
    }

    private void BrainAddedHandler(object sender, EventArgs e)
    {
        Instantiate(brainObj);
    }
    
    private void FingerAddedHandler(object sender, EventArgs e)
    {
        Instantiate(fingerObj);
    }
    
    private void ChangedToTopViewHandler(object sender, bool goingToTop)
    {
        if (goingToTop)
        {
            _mainInstance.FadeTo(_mainInstance.source.volume/2f, 1.5f);
            _windInstance.FadeIn(2f);
        }
        else
        {
            _mainInstance.FadeIn(2f);
            _windInstance.FadeOut(1.5f);
        }
    }

    
    private void ChangedToGraveyardHandler(object sender, bool goingToGrave)
    {
        if (goingToGrave)
        {
            _mainInstance.FadeOut(2f);
            _graveyardInstance.FadeIn(2.5f);
        }
        else
        {
            _mainInstance.FadeIn(2.5f);
            _graveyardInstance.FadeOut(2f);
        }
    }
}