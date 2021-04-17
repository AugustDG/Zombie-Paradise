using System;
using System.Collections;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Random = UnityEngine.Random;

namespace Characters
{
    public class HumanBehaviour : CharacterBehaviour
    {
        [Header("Drops")]
        public int fingerDrop = 5;
        public int brainDrop = 1;
        [SerializeField] private ParticleSystem fingerParticles;
        [SerializeField] private ParticleSystem brainParticles;

        [Space(5f)]
        [Header("Custom Human Heads")]
        [SerializeField] private GameObject femaleHead;
        [SerializeField] private GameObject maleHead;

        [Space(5f)]
        [Header("Roaming")]
        public float distance = 5f;
        public Transform objective;
        private Vector3 _initialPos;

        protected override IEnumerator Start()
        {
            _initialPos = transform.position;
            SpawnHead();
            MapData.HumanList.Add(this);
            
            yield return base.Start();
        }

        public void SpawnHead()
        {
            Instantiate(Random.Range(0, 2) == 0 ? femaleHead : maleHead, headPosition.position, headPosition.rotation, headPosition); //gives a random custom head (M or F)   
        }

        protected override void FindTarget(bool newTarget = false)
        {
            Target.isSearching = true;
            UnityExtensions.DelayAction(this, ()=> StartCoroutine(FindTargetAndDelayRoaming()), Random.Range(1f, 5f));
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Zombie"))
            {
                isDealingDamage = true;
                DamageDealerCoroutine = StartCoroutine(DealDamage(other.GetComponent<CharacterBehaviour>()));
            }
            
            if (other.CompareTag("Z_Base") && isAttackingBuildings)
            {
                isDealingDamage = true;
                DamageDealerCoroutine = StartCoroutine(DealDamage(other.GetComponent<BuildingBehaviour>()));
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
                
                MapEvents.HumanKilledEvent.Invoke(this, EventArgs.Empty);

                for (var i = 0; i < fingerDrop; i++) Instantiate(fingerParticles, transform.position, Quaternion.identity).Play();
                for (var i = 0; i < fingerDrop; i++) Instantiate(brainParticles, transform.position, Quaternion.identity).Play();

                MapData.BrainAmount += brainDrop;
                MapData.FingerAmount += fingerDrop;
            }
        }

        private IEnumerator FindTargetAndDelayRoaming()
        {
            //find an objective position inside a certain radius of starting position
            do
            {
                objective.position = _initialPos + (Random.insideUnitSphere * distance).ChopTo2DVector3();
            } while (!Node.NodeFromWorldPoint(objective.position).canWalk);

            Target.TargetTransform = objective;
            
            var hasCompleted = false;
            
            StartCoroutine(Pathfinder.PathfindToTarget(objective, transform.position, queue =>
            {
                waypoints = queue;
                hasCompleted = true;
            }));

            yield return new WaitUntil(() => hasCompleted);
            Target.isSearching = false;
        }

        private IEnumerator DealDamage(CharacterBehaviour attackedChar)
        {
            if (attackedChar == null || attackedChar.IsScheduledForCleanup)
            {
                base.FindTarget(true);
                yield break;
            }

            attackedChar.SufferDamage(attack);

            yield return new WaitForSecondsRealtime(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(attackedChar));
        }
        
        private IEnumerator DealDamage(BuildingBehaviour atackedBuild)
        {
            if (atackedBuild == null)
            {
                base.FindTarget(true);
                yield break;
            }

            atackedBuild.SufferDamage(attack);

            yield return new WaitForSecondsRealtime(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(atackedBuild));
        }

        protected override IEnumerator Cleanup()
        {
            yield return new WaitForFixedUpdate();

            MapData.HumanList.Remove(this);
            gameObject.Destroy();
        }
    }
}