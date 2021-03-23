using System.Collections;
using ScriptableObjects;
using UnityEngine;
using Utilities.Extensions;
using Utility;

namespace Characters
{
    public class ZombieBehaviour : CharacterBehaviour
    {
        public float spawnCost;
        
        protected override void AssignTarget()
        {
            if (MapData.HumanList.Count > 0)
            {
                var closestHuman = MapData.HumanList[0];

                foreach (var human in MapData.HumanList)
                {
                    if ((closestHuman.transform.position - human.transform.position).sqrMagnitude <
                        (closestHuman.transform.position - transform.position).sqrMagnitude)
                    {
                        closestHuman = human;
                    }
                }

                Target = closestHuman.transform;
            }
        }

        protected override void TriggerEntered(Collider other)
        {
            if (other.CompareTag("Human"))
            {
                isDealingDamage = true;
                DamageDealerCoroutine = StartCoroutine(DealDamage(other.GetComponent<CharacterBehaviour>()));
            }
        }

        protected override void TriggerExited(Collider other)
        {
            if (DamageDealerCoroutine != null)
            {
                isDealingDamage = false;
                StopCoroutine(DamageDealerCoroutine);
            }
        }

        public override void SufferDamage(float damage)
        {
            health -= damage;

            if (health < 0)
            {
                IsScheduledForCleanup = true;
                StartCoroutine(Cleanup());
            }
        }

        private IEnumerator DealDamage(CharacterBehaviour attackChar)
        {
            if (attackChar == null || attackChar.IsScheduledForCleanup)
            {
                Target = null;
                yield break;
            }

            attackChar.SufferDamage(attack);

            yield return new WaitForSecondsRealtime(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(attackChar));
        }
        
        public override IEnumerator Cleanup()
        {
            yield return new WaitForFixedUpdate();

            MapData.ZombieList.Remove(this);
            gameObject.Destroy();
        }
    }
}