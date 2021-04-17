using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Utility;
using Random = UnityEngine.Random;

public class ResearchManager : MonoBehaviour
{
    public EventHandler<ResearchType> OnResearchClick;
    
    [SerializeField] private TMP_Text brainText;
    [SerializeField] private RectTransform techTreeCollection;
    [SerializeField] private RectTransform potionRoomCollection;
    [SerializeField] private CanvasGroup partCreationGroup;

    public SerializedDictionary<ResearchType, int> ResearchCostDictionary = new SerializedDictionary<ResearchType, int>();
    
    [SerializeField] private List<int> researchCosts = new List<int>();
    
    //0: Head
    //1: Torso
    //2: Arm
    //3: Leg
    [SerializeField] private List<GameObject> partPreview = new List<GameObject>(4);

    private bool _onPotionRoom;
    private int _numbOBrains;
    private int _activePart;
    
    public void Awake()
    {
        MapData.ResearchManagerRef = this;
        OnResearchClick += OnResearchClickHandler;
    }

    public void Start()
    {
        RefreshPartPreviewList();
    }

    public void FillDictionary()
    {
        ResearchCostDictionary.Clear();
        for (var i = 0; i < 18; i++) ResearchCostDictionary.Add((ResearchType)i, researchCosts[i]);
    }

    public void ToggleView()
    {
        if (_onPotionRoom)
        {
            techTreeCollection.DOAnchorPosX(0f, 1.5f);
            potionRoomCollection.DOAnchorPosX(2500f, 1.5f);
        }
        else
        {
            techTreeCollection.DOAnchorPosX(-2500f, 1.5f);
            potionRoomCollection.DOAnchorPosX(0f, 1.5f);
        }

        _onPotionRoom = !_onPotionRoom;
        DOTween.PlayAll();
    }

    public void CreatePart()
    {
        MapData.BrainAmount -= _numbOBrains;

        var tempPart = (ZombiePart)ScriptableObject.CreateInstance(typeof(ZombiePart));

        //the part preview list is in accord with the PartType enum 
        tempPart.partType = (PartType)_activePart;

        tempPart.attackModifier = MapData.CurrentMaxAttack * Random.Range(0f + _numbOBrains * 0.08f, _numbOBrains * 0.1f);
        
        tempPart.healthModifier = MapData.CurrentMaxHealth * Random.Range(0f + _numbOBrains * 0.08f, _numbOBrains * 0.1f);
        
        tempPart.speedModifier = MapData.CurrentMaxSpeed * Random.Range(0f + _numbOBrains * 0.08f, _numbOBrains * 0.1f);
        
        tempPart.costModifier = MapData.CurrentCostMult * (tempPart.attackModifier + tempPart.healthModifier + tempPart.speedModifier)/25f;

        switch (tempPart.partType)
        {
            case PartType.Head:
                MapData.CreationManagerRef.heads.Add(tempPart);
                break;
            case PartType.Torso:
                MapData.CreationManagerRef.torsos.Add(tempPart);
                break;
            case PartType.Arms:
                MapData.CreationManagerRef.arms.Add(tempPart);
                break;
            case PartType.Leg:
                MapData.CreationManagerRef.legs.Add(tempPart);
                break;
        }

        _numbOBrains = 0;
            
        partCreationGroup.DOFade(0f, 1.5f).Play().OnComplete(() => StartCoroutine(PartCreationSequence()));
    }

    public void SwitchPartLeft()
    {
        if (_activePart > 0)
            _activePart--;
        else
            _activePart = partPreview.Count - 1;
    
        RefreshPartPreviewList();
    }
    
    public void SwitchPartRight()
    {
        if (_activePart < partPreview.Count - 1)
            _activePart++;
        else
            _activePart = 0;
        
        RefreshPartPreviewList();
    }

    //increase the number of brains to be injected into the cauldron, to create a better part
    public void IncreaseBrainAmount()
    {
        if (_numbOBrains < MapData.BrainAmount && _numbOBrains < 10)
            _numbOBrains++;

        brainText.text = _numbOBrains.ToString();
    }

    public void DecreaseBrainAmount()
    {
        if (_numbOBrains > 0)
            _numbOBrains--;
        
        brainText.text = _numbOBrains.ToString();
    }

    private void RefreshPartPreviewList()
    {
        foreach (var gO in partPreview)
        {
            gO.SetActive(false);
        }

        partPreview[_activePart].SetActive(true);
    }

    private IEnumerator PartCreationSequence()
    {
        partPreview[_activePart].transform.DOLocalMoveX(-0.1f, 1.5f);
        partPreview[_activePart].transform.DOLocalMoveY(-0.15f, 1.5f);
        DOTween.PlayAll();

        yield return new WaitForSecondsRealtime(2f);
        
        partPreview[_activePart].transform.DOLocalMoveX(0f, 1.5f);
        partPreview[_activePart].transform.DOLocalMoveY(0f, 1.5f);
        partPreview[_activePart].transform.DOLocalRotate(Vector3.up * 720f, 2f, RotateMode.FastBeyond360);
        DOTween.PlayAll();
    }
    
    private void OnResearchClickHandler(object sender, ResearchType rType)
    {
        if (!ResearchCostDictionary.TryGetValue(rType, out var value)) return;

        if (value <= MapData.BrainAmount)
        {
            MapData.BrainAmount -= value;

            var rButt = (ResearchButton)sender;
            rButt.RState = ResearchState.Researched;

            foreach (var dependentButt in rButt.dependentButts)
            {
                dependentButt.RState = ResearchState.Unlocked;
                dependentButt.RefreshResearchState();
            }
            
            //todo: apply research effects
            
            switch (rType)
            {
                case ResearchType.ATTACK:
                    MapData.CurrentMaxAttack += 2f;
                    break;
                case ResearchType.HEALTH:
                    MapData.CurrentMaxHealth += 10f;
                    break;
                case ResearchType.SPEED:
                    MapData.CurrentMaxSpeed += 5f;
                    break;
                case ResearchType.COST:
                    MapData.CurrentMaxSpeed -= 0.05f;
                    break;
                case ResearchType.ATTACK2:
                    MapData.CurrentMaxAttack += 3f;
                    break;
                case ResearchType.HEALTH2:
                    MapData.CurrentMaxHealth += 10f;
                    break;
                case ResearchType.SPEED2:
                    MapData.CurrentMaxSpeed += 5f;
                    break;
                case ResearchType.COST2:
                    MapData.CurrentMaxSpeed -= 0.1f;
                    break;
                case ResearchType.ATTACK3:
                    MapData.CurrentMaxAttack += 5f;
                    break;
                case ResearchType.HEALTH3:
                    MapData.CurrentMaxAttack += 5f;
                    break;
                case ResearchType.SPEED3:
                    MapData.CurrentMaxSpeed += 5f;
                    break;
                case ResearchType.COST3:
                    MapData.CurrentMaxSpeed -= 0.1f;
                    break;
                case ResearchType.THEALTH:
                    MapData.CurrentTreeHealth += 250f;
                    break;
                case ResearchType.THEALTH2:
                    MapData.CurrentTreeHealth += 250f;
                    break;
                case ResearchType.THEALTH3:
                    MapData.CurrentTreeHealth += 250f;
                    break;
                case ResearchType.TATTACK:
                    MapData.CurrentTreeAttack += 10f;
                    break;
                case ResearchType.TATTACK2:
                    MapData.CurrentTreeAttack += 10f;
                    break;
                case ResearchType.TATTACK3:
                    MapData.CurrentTreeAttack += 10f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rType), rType, null);
            }
        }
    }
}

public enum ResearchType
{
    ATTACK,
    HEALTH,
    SPEED,
    COST,
    ATTACK2,
    HEALTH2,
    SPEED2,
    COST2,
    ATTACK3,
    HEALTH3,
    SPEED3,
    COST3,
    THEALTH,
    THEALTH2,
    THEALTH3,
    TATTACK,
    TATTACK2,
    TATTACK3,
}