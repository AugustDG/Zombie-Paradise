using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Utility.Events;
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
            if (value) StartCoroutine(Cleanup());
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

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => Pathfinder.HasInit);

        FindTarget(true);
    }

    private void FixedUpdate()
    {
        if (!Pathfinder.HasInit) return;

        if (!Target.isSearching && Target.TargetTransform == null)
        {
            waypoints.Clear();
            _nextNode = null;
            FindTarget(true);
            return;
        }

        if (!Target.isSearching && Node.NodeFromWorldPoint(Target.TargetTransform.position) != Target.node) FindTarget(false);

        if (_nextNode == null)
        {
            if (waypoints.Count > 0) _nextNode = waypoints.Dequeue();
            else if (!Target.isSearching) FindTarget(true);
        }
        else
        {
            transform.LookAt(new Vector3(_nextNode.worldPosition.x, transform.position.y, _nextNode.worldPosition.z));
            transform.position += transform.forward * (Time.deltaTime * speed);

            if (!_isPlayingStep) StartCoroutine(PlayStepSound());

            var transformNode = Node.NodeFromWorldPoint(transform.position);

            if (transformNode == _nextNode) _nextNode = null;
            if (transformNode == Target.node && !Target.isSearching) FindTarget(true);
        }
    }

    public virtual void SufferDamage(float damage) { }

    protected virtual void OnTriggerEnter(Collider other) { }

    protected virtual void OnTriggerExit(Collider other) { }

    //true: Target now has a valid value, false it does not
    protected virtual void FindTarget(bool newTarget = false) { }

    //makes sure that no zombie has this object as a target during a fixed frame
    protected virtual IEnumerator Cleanup() { yield break; }

    private IEnumerator PlayStepSound()
    {
        _isPlayingStep = true;

        var audioArgs = new AudioEventArgs();
        
        MapEvents.StepTakenEvent.Invoke(this, audioArgs);

        yield return new WaitForSeconds(0.5f);

        audioArgs.AudioObject.gameObject.Destroy();
        _isPlayingStep = false;
    }
    
    private void OnDrawGizmos()
    {
        foreach (var node in waypoints)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(node.worldPosition, 0.25f);
        }

        Gizmos.color = Color.yellow;
        if (_nextNode != null) Gizmos.DrawSphere(_nextNode.worldPosition, 0.25f);

        Gizmos.color = Color.red;
        if (Application.isPlaying) Gizmos.DrawSphere(PathfindingManager.NodeFromWorldPoint(transform.position).worldPosition, 0.25f);
    }
}