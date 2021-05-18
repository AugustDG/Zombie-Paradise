using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.Rendering;
using Utility;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private GameObject robotSoldierPrefab;
    [SerializeField] private Transform treeGoal;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        var i = 0;
        HumanBehaviour behaviour;
        
        while (MapData.HumanList.Count > 10)
        {
            i++;
            
            yield return new WaitForSeconds(45f);

            for (var j = 0; j < Mathf.Clamp(2*i, 0, 10); j++)
            {
                yield return new WaitForSeconds(0.5f);
                
                behaviour = Instantiate(robotSoldierPrefab, startPoint.position, Quaternion.identity, startPoint).GetComponent<HumanBehaviour>();

                behaviour.Target.TargetTransform = treeGoal;
            }
        }
        
        behaviour = Instantiate(robotSoldierPrefab, startPoint.position, Quaternion.identity, startPoint).GetComponent<HumanBehaviour>();
        
        behaviour.Target.TargetTransform = treeGoal;
        
        yield return new WaitForSeconds(1f);

        StartCoroutine(SpawnWaves());
    }
}
