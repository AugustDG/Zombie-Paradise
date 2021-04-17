using UnityEngine;
using Utility;
[CreateAssetMenu(fileName = "ZP_", menuName = "Custom/Zombie Part", order = 0)]
public class ZombiePart : ScriptableObject
{
    [Header("Properties")]
    public PartType partType;
    public GameObject partObject;
    public ZombiePart adjacentPart;
    
    [Space(5f)]
    [Header("Modifiers")]
    public float attackModifier;
    public ModifierType attackType;
    
    [Space(5f)]
    public float healthModifier;
    public ModifierType healthType;
    
    [Space(5f)]
    public float speedModifier;
    public ModifierType speedType;
    
    [Space(5f)]
    public float costModifier;
    public ModifierType costType;
}