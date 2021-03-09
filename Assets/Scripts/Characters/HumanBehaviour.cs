using System.Collections;
using UnityEngine;
using Utilities.Extensions;
using Utility;

namespace Characters
{
    public class HumanBehaviour : CharacterBehaviour
    {
        [Space(5f)]
        [Header("Custom Human Heads")]
        [SerializeField] private GameObject femaleHead;
        [SerializeField] private GameObject maleHead;

        [Space(5f)]
        [Header("Roaming")]
        public float distance = 5f;
        [SerializeField] private Transform objective;
        private Vector3 _initialPos;
        private bool _canDelay = true;

        public void Awake()
        {
            Instantiate(Random.Range(0, 2) == 0 ? femaleHead : maleHead, headPosition.position, headPosition.rotation, headPosition); //gives a random custom head (M or F)

            MapData.HumanList.Add(this); //adds itself to the global human & character list
        }

        public void Start()
        {
            _initialPos = transform.position;
        }

        protected override void AssignTarget()
        {
            if (_canDelay) StartCoroutine(DelayRoamingAndAssign());
        }

        protected override void TriggerEntered(Collider other)
        {
            if (other.CompareTag("Zombie"))
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

        private IEnumerator DelayRoamingAndAssign()
        {
            _canDelay = false;
            
            //find an objective position inside a certain radius from starting position
            objective.position = _initialPos + (Random.insideUnitSphere * distance).ChopTo2DVector3();

            Target = objective;
            
            yield return new WaitForSecondsRealtime(Random.Range(5f, 10f));
            _canDelay = true;
        }

        private IEnumerator DealDamage(CharacterBehaviour attackedChar)
        {
            if (attackedChar == null || attackedChar.IsScheduledForCleanup)
            {
                Target = null;
                yield break;
            }

            attackedChar.SufferDamage(attack);

            yield return new WaitForSecondsRealtime(attackInterval);
            if (isDealingDamage) StartCoroutine(DealDamage(attackedChar));
        }
        
        public override IEnumerator Cleanup()
        {
            yield return new WaitForFixedUpdate();

            MapData.HumanList.Remove(this);
            gameObject.Destroy();
        }
    }
}