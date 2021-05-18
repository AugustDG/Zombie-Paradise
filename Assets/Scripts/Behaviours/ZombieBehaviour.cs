using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Target.isSearching = true;

            if (MapData.HumanList.Count > 0)
            {
                Target.isSearching = true;

                Target.TargetTransform = MapData.HumanList[0].transform;

                foreach (var human in MapData.HumanList)
                {
                    if ((transform.position - human.transform.position).sqrMagnitude <
                        (transform.position - Target.TargetTransform.position).sqrMagnitude)
                    {
                        Target.TargetTransform = human.transform;
                    }
                }

                StartCoroutine(Pathfinder.PathfindToTarget(Target.TargetTransform, transform.position, array =>
                {
                    waypoints = new Queue<Node>(array.Reverse());
                    Target.isSearching = false;
                }));
            }
            else Target.isSearching = false;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Human") && !isDealingDamage)
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

            if (health < 0) IsScheduledForCleanup = true;
        }

        private IEnumerator DealDamage(CharacterBehaviour attackChar)
        {
            if (attackChar == null || attackChar.IsScheduledForCleanup)
            {
                base.FindTarget();
                yield break;
            }

            attackChar.SufferDamage(attack);

            yield return new WaitForSeconds(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(attackChar));
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            MapData.ZombieList.Remove(this);
            gameObject.Destroy();
        }
    }
}