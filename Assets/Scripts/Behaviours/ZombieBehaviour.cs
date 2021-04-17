using System.Collections;
using UnityEngine;
using Utilities.Extensions;
using Utility;

namespace Characters
{
    public class ZombieBehaviour : CharacterBehaviour
    {
        public float spawnCost;

        protected override void FindTarget(bool newTarget = false)
        {
            if (!newTarget)
            {
                Target.isSearching = true;

                StartCoroutine(Pathfinder.PathfindToTarget(Target.TargetTransform, transform.position, queue =>
                {
                    foreach (var node in queue)
                    {
                        if (waypoints.Count > 0) waypoints.Dequeue();
                        waypoints.Enqueue(node);   
                    }
                    
                    Target.isSearching = false;
                }));

                return;
            }

            if (MapData.HumanList.Count > 0)
            {
                Target.isSearching = true;

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
                    Target.isSearching = false;
                }));
            }
            else Target.isSearching = false;
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
                base.FindTarget(true);
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