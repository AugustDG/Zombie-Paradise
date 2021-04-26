using System;
using UnityEngine;

[Serializable]
public class Target
{
    public Transform TargetTransform
    {
        get => targetTransform;
        set
        {
            if (value != null) node = Node.NodeFromWorldPoint(value.position);
            targetTransform = value;
        }
    }
    
    [SerializeField] private Transform targetTransform;

    public Node node;
    public bool isSearching;

    public Target(Transform initialTransform)
    {
        TargetTransform = initialTransform;
    }
}
