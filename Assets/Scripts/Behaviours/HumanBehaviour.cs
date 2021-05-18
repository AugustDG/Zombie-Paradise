using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        public bool isRobotSoldier;
        public Transform objective;
        private Vector3 _initialPos;
        
        [Space(5f)]
        [Header("Shooting")]
        public bool isShooting;
        public float shotDelay = 2.5f;
        public Transform shootTrans;

        protected override IEnumerator Start()
        {
            _initialPos = transform.position;
            SpawnHead();
            MapData.HumanList.Add(this);
            
            yield return base.Start();
        }

        private void SpawnHead()
        {
            if (!isRobotSoldier) Instantiate(Random.Range(0, 2) == 0 ? femaleHead : maleHead, headPosition.position, headPosition.rotation, headPosition); //gives a random custom head (M or F)   
        }

        protected override void FindTarget()
        {
            if (isShooting) return;
            
            Target.isSearching = true;
            
            if (!isRobotSoldier)
            {
                /*if (MapData.ZombieList.Count > 0)
                {
                    var closestZombie = MapData.ZombieList[0];

                    foreach (var zombie in MapData.ZombieList)
                    {
                        if ((transform.position - zombie.transform.position).sqrMagnitude <
                            (transform.position - closestZombie.transform.position).sqrMagnitude && (transform.position - closestZombie.transform.position).sqrMagnitude <= 10)
                        {
                            closestZombie = zombie;
                        }
                    }
                    
                    if (closestZombie)
                    
                    StartCoroutine(Pathfinder.PathfindToTarget(Target.TargetTransform, transform.position, queue =>
                    {
                        waypoints = queue;
                        Target.isSearching = false;
                    }));
                }
                else
                {*/
                    UnityExtensions.DelayAction(this, FindTargetDelayed, Random.Range(2f, 15f));
                //}
            }
            else
            {
                StartCoroutine(Pathfinder.PathfindToTarget(Target.TargetTransform, transform.position, array =>
                {
                    waypoints = new Queue<Node>(array.Reverse());
                    Target.isSearching = false;
                }));   
            }
        }
        
        private void FindTargetDelayed()
        {
            //find an objective position inside a certain radius of starting position
            do
            {
                objective.position = _initialPos + (Random.insideUnitSphere * distance).ChopTo2DVector3();
            } while (!Node.NodeFromWorldPoint(objective.position).canWalk);

            Target.TargetTransform = objective;

            StartCoroutine(Pathfinder.PathfindToTarget(objective, transform.position, array =>
            {
                waypoints = new Queue<Node>(array);
                Target.isSearching = false;
            }));
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (isDealingDamage) return;
            
            if (other.CompareTag("Zombie"))
            {
                isDealingDamage = true;
                DamageDealerCoroutine = StartCoroutine(DealDamage(other.GetComponent<CharacterBehaviour>()));
            }
            
            if (other.CompareTag("Z_Base") && isRobotSoldier)
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

                MapEvents.HumanKilledEvent.Invoke(this, EventArgs.Empty);

                for (var i = 0; i < fingerDrop; i++) Instantiate(fingerParticles, transform.position, Quaternion.identity).Play();
                for (var i = 0; i < fingerDrop; i++) Instantiate(brainParticles, transform.position, Quaternion.identity).Play();

                MapData.BrainAmount += brainDrop;
                MapData.FingerAmount += fingerDrop;
            }
        }

        private IEnumerator DealDamage(CharacterBehaviour attackedChar)
        {
            if (attackedChar == null || attackedChar.IsScheduledForCleanup)
            {
                base.FindTarget();
                yield break;
            }

            StartCoroutine(PlayAttackSound());
            
            attackedChar.SufferDamage(attack);

            yield return new WaitForSecondsRealtime(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(attackedChar));
        }
        
        private IEnumerator DealDamage(BuildingBehaviour atackedBuild)
        {
            if (atackedBuild == null)
            {
                base.FindTarget();
                yield break;
            }
            
            StartCoroutine(PlayAttackSound());

            atackedBuild.SufferDamage(attack);

            yield return new WaitForSecondsRealtime(attackInterval);
            StartCoroutine(DealDamage(atackedBuild));
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            
            MapData.HumanList.Remove(this);
            gameObject.Destroy();
        }
    }
}