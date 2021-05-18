using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Utilities.Extensions;
using Utility;
using Random = UnityEngine.Random;

public class ResearchManager : MonoBehaviour
{
    public EventHandler<ResearchType> OnResearchClick;

    [SerializeField] private TMP_Text brainText;
    [SerializeField] private RectTransform techTreeCollection;
    [SerializeField] private RectTransform potionRoomCollection;
    [SerializeField] private CanvasGroup partCreationGroup;
    [SerializeField] private CanvasGroup statsGroup;

    //0: Head
    //1: Torso
    //2: Arm
    //3: Leg
    [SerializeField] private TMP_Text[] statTexts;

    public SerializedDictionary<ResearchType, int> ResearchCostDictionary = new SerializedDictionary<ResearchType, int>();

    [SerializeField] private List<int> researchCosts = new List<int>();

    //Same as statTexts
    [SerializeField] private GameObject[] partPreview = new GameObject[4];

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
            potionRoomCollection.DOAnchorPosX(2500f, 1.5f).OnComplete(() =>
            {
                partCreationGroup.alpha = 1f;
                statsGroup.alpha = 0f;
                statsGroup.blocksRaycasts = false;
            });
        }
        else
        {
            techTreeCollection.DOAnchorPosX(-2500f, 1.5f);
            potionRoomCollection.DOAnchorPosX(0f, 1.5f);
        }

        _onPotionRoom = !_onPotionRoom;
        DOTween.PlayAll();
    }

    public void CreatePartAgain()
    {
        statsGroup.DOFade(0f, 1f);
        statsGroup.blocksRaycasts = false;
        partCreationGroup.DOFade(1f, 1f);
        DOTween.PlayAll();
    }

    public void CreatePart()
    {
        if (_numbOBrains == 0)
        {
            partCreationGroup.DOFade(0.1f, 0.1f).OnComplete(() =>
            {
                partCreationGroup.DOFade(1f, 0.2f).Play();
            }).Play();
            
            return;
        }
        
        MapData.BrainAmount -= _numbOBrains;

        var tempPart = (ZombiePart)ScriptableObject.CreateInstance(typeof(ZombiePart));

        //the part preview list is in sync with the PartType enum 
        tempPart.partType = (PartType)_activePart;

        var calcAttack = (MapData.CurrentMaxAttack * Random.Range(_numbOBrains * 0.04f, _numbOBrains * 0.1f)).Round(1);
        tempPart.attackModifier = calcAttack;
        statTexts[0].text = calcAttack.ToString(CultureInfo.InvariantCulture);

        var calcHealth = (MapData.CurrentMaxHealth * Random.Range(_numbOBrains * 0.04f, _numbOBrains * 0.1f)).Round(1);
        tempPart.healthModifier = calcHealth;
        statTexts[1].text = calcHealth.ToString(CultureInfo.InvariantCulture);

        var calcSpeed = MapData.CurrentMaxSpeed * Random.Range(_numbOBrains * 0.04f, _numbOBrains * 0.1f).Round(1);
        tempPart.speedModifier = calcSpeed;
        statTexts[2].text = calcSpeed.ToString(CultureInfo.InvariantCulture);

        var calcCost = (MapData.CurrentCostMult * (tempPart.attackModifier + tempPart.healthModifier + tempPart.speedModifier) / 10f).Round(1);
        tempPart.costModifier = calcCost;
        statTexts[3].text = calcCost.ToString(CultureInfo.InvariantCulture);
        
        if (_activePart <= 1)
            tempPart.partObject = partPreview[_activePart].transform.GetChild(0).gameObject;
        else
        {
            var adjPart = tempPart;

            tempPart.partObject = partPreview[_activePart].transform.GetChild(0).gameObject;
            adjPart.partObject = partPreview[_activePart].transform.GetChild(1).gameObject;
            
            tempPart.adjacentPart = adjPart;
        }

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
        brainText.text = _numbOBrains.ToString();

        partCreationGroup.DOFade(0f, 1.5f).Play().OnComplete(() => StartCoroutine(PartCreationSequence()));
    }

    public void SwitchPartLeft()
    {
        if (_activePart > 0)
            _activePart--;
        else
            _activePart = partPreview.Length - 1;

        RefreshPartPreviewList();
    }

    public void SwitchPartRight()
    {
        if (_activePart < partPreview.Length - 1)
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
        partPreview[_activePart].transform.DOLocalRotate(Vector3.up * 720f, 2f, RotateMode.FastBeyond360).OnComplete(() =>
        {
            statsGroup.DOFade(1f, 1f).Play();
            statsGroup.blocksRaycasts = true;
        });
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

            switch (rType)
            {
                case ResearchType.ATTACK:
                    MapData.CurrentMaxAttack += 1f;
                    break;
                case ResearchType.HEALTH:
                    MapData.CurrentMaxHealth += 5f;
                    break;
                case ResearchType.SPEED:
                    MapData.CurrentMaxSpeed += 3f;
                    break;
                case ResearchType.COST:
                    MapData.CurrentMaxSpeed -= 0.05f;
                    break;
                case ResearchType.ATTACK2:
                    MapData.CurrentMaxAttack += 3f;
                    break;
                case ResearchType.HEALTH2:
                    MapData.CurrentMaxHealth += 5f;
                    break;
                case ResearchType.SPEED2:
                    MapData.CurrentMaxSpeed += 3f;
                    break;
                case ResearchType.COST2:
                    MapData.CurrentMaxSpeed -= 0.15f;
                    break;
                case ResearchType.ATTACK3:
                    MapData.CurrentMaxAttack += 5f;
                    break;
                case ResearchType.HEALTH3:
                    MapData.CurrentMaxAttack += 5f;
                    break;
                case ResearchType.SPEED3:
                    MapData.CurrentMaxSpeed += 3f;
                    break;
                case ResearchType.COST3:
                    MapData.CurrentMaxSpeed -= 0.15f;
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