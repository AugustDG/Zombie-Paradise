using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Z_", menuName = "Custom/Zombie", order = 0)]
    public class ZombieData : ScriptableObject
    {
        [Header("Parts")]
        public ZombiePart head;
        public ZombiePart torso;
        public ZombiePart[] arms = new ZombiePart[2];
        public ZombiePart[] legs = new ZombiePart[2];

        [Header("Total Modifiers")]
        public float totalAttack;
        public float totalHealth;
        public float totalSpeed;
        public float totalCost;

        public void CalculateTotalModifiers()
        {
            totalAttack = 0;
            totalHealth = 0;
            totalSpeed = 0;
            totalCost = 0;

            totalAttack = totalAttack
              + (head is { attackType   : ModifierType.Add } ? head.attackModifier : 0)
              + (torso is { attackType  : ModifierType.Add } ? torso.attackModifier : 0)
              + (arms[0] is { attackType: ModifierType.Add } ? arms[0].attackModifier : 0)
              + (arms[1] is { attackType: ModifierType.Add } ? arms[1].attackModifier : 0)
              + (legs[0] is { attackType: ModifierType.Add } ? legs[0].attackModifier : 0)
              + (legs[1] is { attackType: ModifierType.Add } ? legs[1].attackModifier : 0);

            totalAttack = totalAttack
              * (head is { attackType   : ModifierType.Mult } ? head.attackModifier : 1)
              * (torso is { attackType  : ModifierType.Mult } ? torso.attackModifier : 1)
              * (arms[0] is { attackType: ModifierType.Mult } ? arms[0].attackModifier : 1)
              * (arms[1] is { attackType: ModifierType.Mult } ? arms[1].attackModifier : 1)
              * (legs[0] is { attackType: ModifierType.Mult } ? legs[0].attackModifier : 1)
              * (legs[1] is { attackType: ModifierType.Mult } ? legs[1].attackModifier : 1);

            totalHealth = totalHealth
              + (head is { healthType   : ModifierType.Add } ? head.healthModifier : 0)
              + (torso is { healthType  : ModifierType.Add } ? torso.healthModifier : 0)
              + (arms[0] is { healthType: ModifierType.Add } ? arms[0].healthModifier : 0)
              + (arms[1] is { healthType: ModifierType.Add } ? arms[1].healthModifier : 0)
              + (legs[0] is { healthType: ModifierType.Add } ? legs[0].healthModifier : 0)
              + (legs[1] is { healthType: ModifierType.Add } ? legs[1].healthModifier : 0);

            totalHealth = totalHealth
              * (head is { healthType   : ModifierType.Mult } ? head.healthModifier : 1)
              * (torso is { healthType  : ModifierType.Mult } ? torso.healthModifier : 1)
              * (arms[0] is { healthType: ModifierType.Mult } ? arms[0].healthModifier : 1)
              * (arms[1] is { healthType: ModifierType.Mult } ? arms[1].healthModifier : 1)
              * (legs[0] is { healthType: ModifierType.Mult } ? legs[0].healthModifier : 1)
              * (legs[1] is { healthType: ModifierType.Mult } ? legs[1].healthModifier : 1);

            totalSpeed = totalSpeed
              + (head is { speedType   : ModifierType.Add } ? head.speedModifier : 0)
              + (torso is { speedType  : ModifierType.Add } ? torso.speedModifier : 0)
              + (arms[0] is { speedType: ModifierType.Add } ? arms[0].speedModifier : 0)
              + (arms[1] is { speedType: ModifierType.Add } ? arms[1].speedModifier : 0)
              + (legs[0] is { speedType: ModifierType.Add } ? legs[0].speedModifier : 0)
              + (legs[1] is { speedType: ModifierType.Add } ? legs[1].speedModifier : 0);

            totalSpeed = totalSpeed
              * (head is { speedType   : ModifierType.Mult } ? head.speedModifier : 1)
              * (torso is { speedType  : ModifierType.Mult } ? torso.speedModifier : 1)
              * (arms[0] is { speedType: ModifierType.Mult } ? arms[0].speedModifier : 1)
              * (arms[1] is { speedType: ModifierType.Mult } ? arms[1].speedModifier : 1)
              * (legs[0] is { speedType: ModifierType.Mult } ? legs[0].speedModifier : 1)
              * (legs[1] is { speedType: ModifierType.Mult } ? legs[1].speedModifier : 1);
            
            totalCost = totalCost
            + (head is { costType   : ModifierType.Add } ? head.costModifier : 0)
            + (torso is { costType  : ModifierType.Add } ? torso.costModifier : 0)
            + (arms[0] is { costType: ModifierType.Add } ? arms[0].costModifier : 0)
            + (arms[1] is { costType: ModifierType.Add } ? arms[1].costModifier : 0)
            + (legs[0] is { costType: ModifierType.Add } ? legs[0].costModifier : 0)
            + (legs[1] is { costType: ModifierType.Add } ? legs[1].costModifier : 0);

            totalCost = totalCost
            * (head is { costType   : ModifierType.Mult } ? head.costModifier : 1)
            * (torso is { costType  : ModifierType.Mult } ? torso.costModifier : 1)
            * (arms[0] is { costType: ModifierType.Mult } ? arms[0].costModifier : 1)
            * (arms[1] is { costType: ModifierType.Mult } ? arms[1].costModifier : 1)
            * (legs[0] is { costType: ModifierType.Mult } ? legs[0].costModifier : 1)
            * (legs[1] is { costType: ModifierType.Mult } ? legs[1].costModifier : 1);
        }
    }
}