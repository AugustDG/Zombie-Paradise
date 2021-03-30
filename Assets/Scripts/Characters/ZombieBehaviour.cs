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

        protected override void FindTarget()
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

                Target.TargetTransform = closestHuman.transform;
                StartCoroutine(Pathfinder.PathfindToTarget(closestHuman.transform, transform.position, queue =>
                {
                    waypoints = queue;
                }));
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Human"))
            {
                isDealingDamage = true;
                DamageDealerCoroutine = StartCoroutine(DealDamage(other.GetComponent<CharacterBehaviour>()));
            }
        }

        protected override void OnTriggerExit(Collider other)
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
                Target.TargetTransform = null;
                yield break;
            }

            attackChar.SufferDamage(attack);

            yield return new WaitForSeconds(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(attackChar));
        }

        protected override IEnumerator Cleanup()
        {
            yield return new WaitForFixedUpdate();

            MapData.ZombieList.Remove(this);
            gameObject.Destroy();
        }
    }
}