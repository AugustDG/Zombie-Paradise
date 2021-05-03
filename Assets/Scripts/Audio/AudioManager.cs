using System;
using Characters;
using UnityEngine;
using Utility;
using Utility.Events;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioObject windObj;
    [SerializeField] private AudioObject graveyardObj;
    [SerializeField] private AudioObject mainAmbienceObj;
    [SerializeField] private AudioObject endAmbienceObj;
    [SerializeField] private AudioObject footstepObj;
    [SerializeField] private AudioObject robotStepObj;
    [SerializeField] private AudioObject robotAttackObj;
    [SerializeField] private AudioObject zombieAttackObj;
    [SerializeField] private AudioObject lifeLostObj;
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
        MapEvents.CurrencyChangedEvent += CurrencyChangedHandler;
        MapEvents.HumanKilledEvent += HumanKilledHandler;
        MapEvents.StepTakenEvent += StepTakenHandler;
        MapEvents.AttackEvent += AttackHandler;
        MapEvents.TreeLifeLostEvent += TreeLifeLostHandler;
    }
    private void TreeLifeLostHandler(object sender, EventArgs e)
    {
        Instantiate(lifeLostObj, transform);
    }
    
    private void StepTakenHandler(object sender, AudioEventArgs obj)
    {
        var behaviour = sender as HumanBehaviour;
        
        if (behaviour == null) return;

        obj.AudioObject = Instantiate(behaviour.isRobotSoldier ? robotStepObj : footstepObj, behaviour.transform);
    }
    
    private void AttackHandler(object sender, AudioEventArgs obj)
    {
        var behaviour = sender as CharacterBehaviour;
        
        if (behaviour == null) return;

        if (behaviour is HumanBehaviour hBehaviour) obj.AudioObject = Instantiate(hBehaviour.isRobotSoldier ? robotAttackObj : null, hBehaviour.transform);

        if (behaviour is ZombieBehaviour zBehaviour) obj.AudioObject = Instantiate(zombieAttackObj, zBehaviour.transform);
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

    private void CurrencyChangedHandler(object sender, CurrencyType e)
    {
        if (e == CurrencyType.BrainsAdded)
            Instantiate(brainObj, transform);
        else if (e == CurrencyType.FingersAdded)
            Instantiate(fingerObj, transform);
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