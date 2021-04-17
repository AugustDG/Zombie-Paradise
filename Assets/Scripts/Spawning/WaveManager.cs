using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Utility;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameObject robotSoldierPrefab;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        var i = 0;
        while (MapData.HumanList.Count > 10)
        {
            i++;
            
            yield return new WaitForSeconds(15f);

            for (var j = 0; j < Mathf.Clamp(2*i, 0, 50); j++)
            {
                Instantiate(robotSoldierPrefab, startPoint.position, Quaternion.identity, startPoint);
            }
        }
        
        Instantiate(robotSoldierPrefab, startPoint.position, Quaternion.identity, startPoint);
        
        yield return new WaitForSeconds(1f);

        StartCoroutine(SpawnWaves());
    }
}
