using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Utilities.Extensions;
using Utility;
public class CreationManager : MonoBehaviour
{
    public ZombieData defaultZombie;

    [SerializeField] private Transform headPosition;
    [SerializeField] private Transform torsoPosition;
    [SerializeField] private Transform armLPosition;
    [SerializeField] private Transform armRPosition;
    [SerializeField] private Transform legLPosition;
    [SerializeField] private Transform legRPosition;

    private List<GameObject> _spawnedParts = new List<GameObject>();

    public void Awake()
    {
        defaultZombie.CalculateTotalModifiers();
        MapData.ZombieToSpawn = defaultZombie;
    }

    public void DeleteZombie()
    {
        #if !UNITY_EDITOR
        _spawnedParts.ForEach((obj)=>obj.Destroy());
        #else
        _spawnedParts.ForEach(obj => obj.DestroyImmediate());
        #endif
    }

    public void DisplayZombie()
    {
        DeleteZombie();

        _spawnedParts.Clear();

        _spawnedParts.Add(Instantiate(defaultZombie.head.partObject, headPosition.position, Quaternion.identity, headPosition));
        _spawnedParts.Add(Instantiate(defaultZombie.torso.partObject, torsoPosition.position, Quaternion.identity, torsoPosition));
        _spawnedParts.Add(Instantiate(defaultZombie.arms[0].partObject, armLPosition.position, Quaternion.identity, armLPosition));
        _spawnedParts.Add(Instantiate(defaultZombie.arms[1].partObject, armRPosition.position, Quaternion.identity, armRPosition));
        _spawnedParts.Add(Instantiate(defaultZombie.legs[0].partObject, legLPosition.position, Quaternion.identity, legLPosition));
        _spawnedParts.Add(Instantiate(defaultZombie.legs[1].partObject, legRPosition.position, Quaternion.identity, legRPosition));
    }
}