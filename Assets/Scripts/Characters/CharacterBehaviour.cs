using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public List<Node> waypointsList = new List<Node>();

    public Target Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField] private Target target;

    private Node _nextNode;

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => Pathfinder.HasInit);
        
        FindTarget();
    }
    
    private void FixedUpdate()
    {
        if (!Pathfinder.HasInit) return;

        waypointsList.Clear();
        waypointsList.AddRange(waypoints.ToArray());

        if (Target.TargetTransform == null || Node.NodeFromWorldPoint(Target.TargetTransform.position) != Target.node)
        {
            waypoints.Clear();
            _nextNode = null;
            FindTarget();
        }
        
        if (_nextNode == null)
        {
            if (waypoints.Count > 0) _nextNode = waypoints.Dequeue();
            else  FindTarget();   
        }
        else
        {
            transform.LookAt(new Vector3(_nextNode.worldPosition.x, transform.position.y, _nextNode.worldPosition.z));
            transform.position += transform.forward * (Time.deltaTime * speed);

            var transformNode = Node.NodeFromWorldPoint(transform.position);
            
            if (transformNode == _nextNode) _nextNode = null;
            if (transformNode == Target.node) FindTarget();
        }
    }

    protected virtual void OnTriggerEnter(Collider other) { }

    protected virtual void OnTriggerExit(Collider other) { }

    //true: Target now has a valid value, false it does not
    protected virtual void FindTarget() { }

    //makes sure that no zombie has this object as a target during a fixed frame
    protected virtual IEnumerator Cleanup() { yield break; }

    public virtual void SufferDamage(float damage) { }

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