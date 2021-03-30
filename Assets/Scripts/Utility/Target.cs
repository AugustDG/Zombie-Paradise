using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Target
{
    public Transform TargetTransform
    {
        get => targetTransform;
        set
        {
            if (value != null) node = PathfindingManager.NodeFromWorldPoint(value.position);
            targetTransform = value;
        }
    }
    
    [SerializeField] private Transform targetTransform;

    public Node node;

    public Target(Transform initialTransform)
    {
        TargetTransform = initialTransform;
    }
}
