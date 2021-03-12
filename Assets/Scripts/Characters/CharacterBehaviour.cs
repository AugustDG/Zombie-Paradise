using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
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
    public Vector2Int targetPosition;
    public bool canSkipCalculation;
    public bool hasTargetChanged = true;

    public Queue<Node> waypoints = new Queue<Node>();
    public Transform Target
    {
        get => target;
        protected set
        {
            FindValidTargetPosition(value);
            target = value;
        }
    }
    [SerializeProperty(nameof(Target))]
    public Transform target;

    private Node _nextNode;

    public virtual void FixedUpdate()
    {
        if (_nextNode is null)
        {
            if (waypoints.Count > 0) _nextNode = waypoints.Dequeue();

            if (Target == null || MapManager.NodeFromWorldPoint(Target.position).gridPosition != targetPosition) AssignTarget();
        }
        else
        {
            transform.LookAt(new Vector3(_nextNode.worldPosition.x, transform.position.y, _nextNode.worldPosition.z));
            transform.position += transform.forward * (Time.deltaTime * speed);
            if ((_nextNode.worldPosition - transform.position).sqrMagnitude < 1f) _nextNode = null;
        }
    }

    private void Start() => ChildStart();

    private void OnTriggerEnter(Collider other) => TriggerEntered(other);

    private void OnTriggerExit(Collider other) => TriggerExited(other);

    private void FindValidTargetPosition(Transform trans)
    {
        var tempNode = trans != null
            ? MapManager.NodeFromWorldPoint(trans.position)
            : MapManager.NodeFromWorldPoint(transform.position);

        while (tempNode.nodeType == NodeTypes.Blocked) tempNode = tempNode.neighbours[0];

        targetPosition = tempNode.gridPosition;
        hasTargetChanged = true;
    }

    //true: Target now has a valid value, false it does not
    protected virtual void AssignTarget() { }

    protected virtual void ChildStart() { }

    protected virtual void TriggerEntered(Collider other) { }

    protected virtual void TriggerExited(Collider other) { }

    public virtual void SufferDamage(float damage) { }

    //makes sure that no zombie has this object as a target during a fixed frame
    public virtual IEnumerator Cleanup() { yield break; }

    private void OnDrawGizmos()
    {
        foreach (var node in waypoints)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(node.worldPosition, 0.25f);
        }

        Gizmos.color = Color.yellow;
        if (_nextNode != null) Gizmos.DrawSphere(_nextNode.worldPosition, 0.25f);
    }
}