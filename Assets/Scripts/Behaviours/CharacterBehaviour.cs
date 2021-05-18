using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Collections;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Utility.Events;
using Random = UnityEngine.Random;
public class CharacterBehaviour : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 5f;
    public float health = 100f;
    public float attack = 2f;
    public float attackInterval = 1f;
    public bool IsScheduledForCleanup
    {
        get => isScheduledForCleanup;

        protected set
        {
            if (value) Cleanup();
            isScheduledForCleanup = value;
        }
    }
    [SerializeProperty(nameof(IsScheduledForCleanup))]
    public bool isScheduledForCleanup;

    [Space(5f)] [Header("Body Parts")]
    public Transform headPosition;
    public Transform torsoPosition;
    public Transform armLPosition;
    public Transform armRPosition;
    public Transform legLPosition;
    public Transform legRPosition;

    [Space(5f)] [Header("Attacking")]
    public bool isDealingDamage;
    protected Coroutine DamageDealerCoroutine;

    [Space(5f)] [Header("Pathfinding")]
    public Queue<Node> waypoints = new Queue<Node>();

    public Target Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField] private Target target;
    private Node _nextNode;

    private bool _isPlayingStep;
    public List<Node> nodes;
    

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => Pathfinder.HasInit);

        FindTarget();
    }

    private void FixedUpdate()
    {
        if (!Pathfinder.HasInit) return;

        nodes.Clear();
        nodes = new List<Node>(waypoints.ToArray());
        
        //when the humans are shooting, they don't move
        if (this is HumanBehaviour)
        {
            var behaviour = (HumanBehaviour)this;
            if (behaviour.isShooting)
            {
                transform.LookAt(behaviour.shootTrans);
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                return;
            }
        }

        //if we lost target (i.e. it died)
        if (!Target.isSearching && Target.TargetTransform == null)
        {
            waypoints.Clear();
            _nextNode = null;
            FindTarget();
            return;
        }

        //if target moved
        if (!Target.isSearching && Node.NodeFromWorldPoint(Target.TargetTransform.position) != Target.node) FindTarget();

        if (_nextNode == null)
        {
            if (waypoints.Count > 0) _nextNode = waypoints.Dequeue();
            else if (!Target.isSearching) FindTarget();
        }
        else
        {
            transform.LookAt(new Vector3(_nextNode.worldPosition.x, transform.position.y, _nextNode.worldPosition.z));
            transform.position += transform.forward * (Time.deltaTime * speed);

            if (!_isPlayingStep)
            {
                StartCoroutine(PlayStepSound());
                //StartCoroutine(PlayStepAnim());
            }

            var transformNode = Node.NodeFromWorldPoint(transform.position);

            if (transformNode == _nextNode) _nextNode = null;
            //if (transformNode == Target.node && !Target.isSearching) FindTarget();
        }
    }

    public virtual void SufferDamage(float damage) { }

    protected virtual void OnTriggerEnter(Collider other) { }

    protected virtual void OnTriggerExit(Collider other) { }

    //true: Target now has a valid value, false it does not
    protected virtual void FindTarget() { }

    private void OnDestroy() => Cleanup();

    protected virtual void Cleanup() { }

    private IEnumerator PlayStepAnim()
    {
        var duration = 0.25f;

        //AnimTweens[0] = legLPosition.DOLocalRotate(new Vector3(0f, 0f, 30f), duration).intId;
        //AnimTweens[1] = legRPosition.DOLocalRotate(new Vector3(0f, 0f, -30f), duration).intId;
       //AnimTweens[2] = armLPosition.DOLocalRotate(new Vector3(0f, 0f, 30f), duration).intId;
        //AnimTweens[3] = armRPosition.DOLocalRotate(new Vector3(0f, 0f, -30f), duration).intId;

        DOTween.PlayAll();

        yield return new WaitForSecondsRealtime(duration);

        //AnimTweens[0] = legLPosition.DOLocalRotate(new Vector3(0f, 0f, -30f), duration).intId;
        //AnimTweens[1] = legRPosition.DOLocalRotate(new Vector3(0f, 0f, 30f), duration).intId;
        //AnimTweens[2] = armLPosition.DOLocalRotate(new Vector3(0f, 0f, -30f), duration).intId;
        //AnimTweens[3] = armRPosition.DOLocalRotate(new Vector3(0f, 0f, 30f), duration).intId;

        DOTween.PlayAll();

        yield return new WaitForSecondsRealtime(duration);

        //AnimTweens[0] =legLPosition.DOLocalRotate(new Vector3(0f, 0f, 0f), duration).intId;
        //AnimTweens[1] =legRPosition.DOLocalRotate(new Vector3(0f, 0f, 0f), duration).intId;
        //AnimTweens[2] =armLPosition.DOLocalRotate(new Vector3(0f, 0f, 0f), duration).intId;
        //AnimTweens[3] =armRPosition.DOLocalRotate(new Vector3(0f, 0f, 0f), duration).intId;

        DOTween.PlayAll();

        yield return new WaitForSecondsRealtime(duration);
    }

    private IEnumerator PlayStepSound()
    {
        if (this is ZombieBehaviour) yield break;

        _isPlayingStep = true;

        var audioArgs = new AudioEventArgs();

        MapEvents.StepTakenEvent.Invoke(this, audioArgs);

        yield return new WaitForSeconds(0.75f);

        audioArgs.AudioObject.gameObject.Destroy();
        _isPlayingStep = false;
    }

    protected IEnumerator PlayAttackSound()
    {
        var audioArgs = new AudioEventArgs();

        MapEvents.AttackEvent.Invoke(this, audioArgs);

        yield return new WaitForSeconds(0.5f);

        audioArgs.AudioObject.gameObject.Destroy();
    }

    private void OnDrawGizmos()
    {
        for (var i = 0; i < waypoints.Count; i++)
        {
            var node = waypoints.ToArray()[i];
            
            if (node == null) continue;
            Gizmos.color = new Color(1, 0, 1f * i/waypoints.Count, 1);
            Gizmos.DrawSphere(node.worldPosition, 0.25f);
        }

        Gizmos.color = Color.yellow;
        if (_nextNode != null) Gizmos.DrawSphere(_nextNode.worldPosition, 0.25f);

        Gizmos.color = Color.red;
        if (Application.isPlaying) Gizmos.DrawSphere(PathfindingManager.NodeFromWorldPoint(transform.position).worldPosition, 0.25f);
    }
}