using System;
using System.Collections.Generic;
using System.Globalization;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities.Extensions;
using Utility;
public class CreationManager : MonoBehaviour
{
    public List<ZombiePart> heads;
    public List<ZombiePart> torsos;
    public List<ZombiePart> arms;
    public List<ZombiePart> legs;
    
    [SerializeField] private Transform headPosition;
    [SerializeField] private Transform torsoPosition;
    [SerializeField] private Transform armLPosition;
    [SerializeField] private Transform armRPosition;
    [SerializeField] private Transform legLPosition;
    [SerializeField] private Transform legRPosition;

    [SerializeField] private TMP_Text attackLabel;
    [SerializeField] private TMP_Text healthLabel;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text costLabel;

    private GameObject _spawnedHead;
    private ZombiePart _spawnedHeadPart;
    private int _headIndex;
    private GameObject _spawnedTorso;
    private ZombiePart _spawnedTorsoPart;
    private int _torsoIndex;
    private GameObject[] _spawnedArms = new GameObject[2];
    private ZombiePart[] _spawnedArmsPart = new ZombiePart[2];
    private int _armsIndex;
    private GameObject[] _spawnedLegs = new GameObject[2];
    private ZombiePart[] _spawnedLegsPart = new ZombiePart[2];
    private int _legsIndex;

    public void Awake()
    {
        MapData.CreationManagerRef = this;
    }

    public void Start()
    {
        DisplayZombie();
    }

    /// <summary>
    /// Adds supplied part to the corresponding list!
    ///
    /// Caution: when giving arms or legs, only give the left one with the right being adjacent ;)
    /// Also, it doesn't save, so beware shutting the game down!
    /// </summary>
    /// <param name="part"> Part to be given and added to part list!</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void AddPart(ZombiePart part)
    {
        switch (part.partType)
        {
            case PartType.Head:
                heads.Add(part);
                break;
            case PartType.Torso:
                torsos.Add(part);
                break;
            case PartType.Arms:
                arms.Add(part);
                break;
            case PartType.Leg:
                legs.Add(part);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SwapHead(int side)
    {
        if ((Side)side == Side.Right)
        {
            if (_headIndex < heads.Count - 1)
                _headIndex++;
            else
                _headIndex = 0;
        }
        else
        {
            if (_headIndex > 0)
                _headIndex--;
            else
                _headIndex = heads.Count - 1;
        }
        
        _spawnedHead.Destroy();

        _spawnedHeadPart = heads[_headIndex];
        
        _spawnedHead = Instantiate(_spawnedHeadPart.partObject, headPosition.position, headPosition.rotation, headPosition);
        
        _spawnedHead.GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        
        CalculateCurrentStats();
    }
    
    public void SwapTorso(int side)
    {
        if ((Side)side == Side.Right)
        {
            if (_torsoIndex < torsos.Count - 1)
                _torsoIndex++;
            else
                _torsoIndex = 0;
        }
        else
        {
            if (_torsoIndex > 0)
                _torsoIndex--;
            else
                _torsoIndex = torsos.Count - 1;
        }
        
        _spawnedTorso.Destroy();

        _spawnedTorsoPart = torsos[_torsoIndex];
        
        _spawnedTorso = Instantiate(_spawnedTorsoPart.partObject, torsoPosition.position, torsoPosition.rotation, torsoPosition);
        
        _spawnedTorso.GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        
        CalculateCurrentStats();
    }
    
    public void SwapArms(int side)
    {
        if ((Side)side == Side.Right)
        {
            if (_armsIndex < arms.Count - 1)
                _armsIndex++;
            else
                _armsIndex = 0;
        }
        else
        {
            if (_armsIndex > 0)
                _armsIndex--;
            else
                _armsIndex = arms.Count - 1;
        }
        
        _spawnedArms[0].Destroy();
        _spawnedArms[1].Destroy();

        _spawnedArmsPart[0] = arms[_armsIndex];
        _spawnedArmsPart[1] = arms[_armsIndex].adjacentPart;
        
        _spawnedArms[0] = Instantiate(_spawnedArmsPart[0].partObject, armLPosition.position, armLPosition.rotation, armLPosition);
        _spawnedArms[1] = Instantiate(_spawnedArmsPart[1].partObject, armRPosition.position, armRPosition.rotation, armRPosition);
        
        _spawnedArms[0].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedArms[1].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        
        CalculateCurrentStats();
    }
    
    public void SwapLegs(int side)
    {
        if ((Side)side == Side.Right)
        {
            if (_legsIndex < legs.Count - 1)
                _legsIndex++;
            else
                _legsIndex = 0;
        }
        else
        {
            if (_legsIndex > 0)
                _legsIndex--;
            else
                _legsIndex = legs.Count - 1;
        }
        
        _spawnedLegs[0].Destroy();
        _spawnedLegs[1].Destroy();

        _spawnedLegsPart[0] = legs[_legsIndex];
        _spawnedLegsPart[1] = legs[_legsIndex].adjacentPart;
        
        _spawnedLegs[0] = Instantiate(_spawnedLegsPart[0].partObject, legLPosition.position, legLPosition.rotation, legLPosition);
        _spawnedLegs[1] = Instantiate(_spawnedLegsPart[1].partObject, legRPosition.position, legRPosition.rotation, legRPosition);
        
        _spawnedLegs[0].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedLegs[1].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        
        CalculateCurrentStats();
    }

    private void CalculateCurrentStats()
    {
        var tempZData = (ZombieData)ScriptableObject.CreateInstance(typeof(ZombieData));
        
        tempZData.head = _spawnedHeadPart;
        tempZData.torso = _spawnedTorsoPart;
        tempZData.arms = _spawnedArmsPart;
        tempZData.legs = _spawnedLegsPart;
        
        tempZData.CalculateTotalModifiers();
        
        attackLabel.text = tempZData.totalAttack.ToString(CultureInfo.InvariantCulture);
        healthLabel.text = tempZData.totalHealth.ToString(CultureInfo.InvariantCulture);
        speedLabel.text = tempZData.totalSpeed.ToString(CultureInfo.InvariantCulture);
        costLabel.text =  tempZData.totalCost.ToString(CultureInfo.InvariantCulture);

        MapData.ZombieToSpawn = tempZData;
        
        MapEvents.ZombieCreated.Invoke(this, EventArgs.Empty);
    }

    public void DeleteZombie()
    {
        if (!Application.isEditor)
        {
            _spawnedHead.Destroy();
            _spawnedTorso.Destroy();   
            _spawnedArms[0].Destroy();   
            _spawnedArms[1].Destroy();   
            _spawnedLegs[0].Destroy();   
            _spawnedLegs[1].Destroy();   
        }
        else
        {
            _spawnedHead.DestroyImmediate();
            _spawnedTorso.DestroyImmediate();   
            _spawnedArms[0].DestroyImmediate();   
            _spawnedArms[1].DestroyImmediate();   
            _spawnedLegs[0].DestroyImmediate();   
            _spawnedLegs[1].DestroyImmediate();
        }
        
    }

    public void DisplayZombie()
    {
        DeleteZombie();
        
        _spawnedHeadPart = heads[0];
        _spawnedTorsoPart = torsos[0];
        _spawnedArmsPart[0] = arms[0];
        _spawnedArmsPart[1] = arms[0].adjacentPart;
        _spawnedLegsPart[0] = legs[0];
        _spawnedLegsPart[1] = legs[0].adjacentPart;
        
        _spawnedHead = Instantiate(heads[0].partObject, headPosition.position, headPosition.rotation, headPosition);
        _spawnedTorso = Instantiate(torsos[0].partObject, torsoPosition.position, torsoPosition.rotation, torsoPosition);
        _spawnedArms[0] = Instantiate(arms[0].partObject, armLPosition.position, armLPosition.rotation, armLPosition);
        _spawnedArms[1] = Instantiate(arms[0].adjacentPart.partObject, armRPosition.position, armRPosition.rotation, armRPosition);
        _spawnedLegs[0] = Instantiate(legs[0].partObject, legLPosition.position, legLPosition.rotation, legLPosition);
        _spawnedLegs[1] = Instantiate(legs[0].adjacentPart.partObject, legRPosition.position, legRPosition.rotation, legRPosition);

        _spawnedHead.GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedTorso.GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedArms[0].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedArms[1].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedLegs[0].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _spawnedLegs[1].GetComponentInChildren<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;

        CalculateCurrentStats();
    }
}