using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public static class Pathfinder
{
    public static NativeArray<JobNode> JobNodeMap;
    public static Node[,] NodeMap;

    public static bool HasInit;

    private static Vector2Int _mapSize;
    private static Vector2Int _gridSize;
    private static Random _random;
    private static Dictionary<string, NativeArray<JobNode>> _pathfindingNodes = new Dictionary<string, NativeArray<JobNode>>();

    //returns true if it has successfully initialized
    public static bool Init(Vector2Int mapSize, Vector2Int gridSize, [NotNull] Node[,] map, NativeArray<JobNode> jobMap, uint seed = 666505999)
    {
        if (HasInit) return false;
        if (mapSize.x == 0 || mapSize.y == 0) return false;

        _mapSize = mapSize;
        _gridSize = gridSize;
        NodeMap = map;
        JobNodeMap = jobMap;

        _random = Random.CreateFromIndex(seed);

        HasInit = true;
        return true;
    }

    public static bool Cleanup()
    {
        if (!HasInit) return false;

        foreach (var node in _pathfindingNodes)
        {
            if (node.Value.IsCreated) node.Value.Dispose();
            //if (node.Value.Costs.IsCreated) node.Value.Costs.Dispose();
        }

        JobNodeMap.Dispose();

        HasInit = false;
        return true;
    }

    //Functions that convert different inputs to required ones for the scheduler
    public static IEnumerator PathfindToTarget(Vector2Int targetPosition, Vector3 startWorldPosition, [NotNull] Action<IEnumerable<Node>> onPathFound)
    {
        return SchedulePathfinding(targetPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    public static IEnumerator PathfindToTarget(Node targetNode, Vector3 startWorldPosition, [NotNull] Action<IEnumerable<Node>> onPathFound)
    {
        return SchedulePathfinding(targetNode.gridPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    public static IEnumerator PathfindToTarget(Vector3 targetWorldPos, Vector3 startWorldPosition, [NotNull] Action<IEnumerable<Node>> onPathFound)
    {
        return SchedulePathfinding(Node.NodeFromWorldPoint(targetWorldPos, _mapSize, _gridSize, ref NodeMap).gridPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    public static IEnumerator PathfindToTarget(Transform targetTrans, Vector3 startWorldPosition, [NotNull] Action<IEnumerable<Node>> onPathFound)
    {
        return SchedulePathfinding(Node.NodeFromWorldPoint(targetTrans.position, _mapSize, _gridSize, ref NodeMap).gridPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    //schedules and completes each pathfinding job
    private static IEnumerator SchedulePathfinding(Vector2Int targetGridPosition, Vector2Int startGridPosition, [NotNull] Action<IEnumerable<Node>> onPathFound)
    {
        var qty = new NativeArray<int>(1, Allocator.Persistent);
        var uid = Guid.NewGuid().ToString();
        var handle = new JobHandle();
        
        try
        {
            if (!HasInit) yield break;

            _pathfindingNodes.Add(uid, new NativeArray<JobNode>(100, Allocator.Persistent));

            var job = new PathfindJob
            {
                Map = JobNodeMap,
                CharPos = targetGridPosition,
                TargetPos = startGridPosition,
                Randomizer = Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 500000)),
                MapSize = _mapSize.x,
                NodeQty = qty,
                OutNodes = _pathfindingNodes[uid]
            };

            handle = job.Schedule();

            handle.Complete();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        
        yield return new WaitUntil(() => handle.IsCompleted);

        try
        {
            onPathFound.Invoke(Node.ToNodes(_pathfindingNodes[uid].ToArray(), qty[0]));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            _pathfindingNodes[uid].Dispose();
            qty.Dispose();
            _pathfindingNodes.Remove(uid);
        }
    }
}

[BurstCompile]
public struct PathfindJob : IJob
{
    [ReadOnly] public NativeArray<JobNode> Map;
    [ReadOnly] public Vector2Int TargetPos;
    [ReadOnly] public Vector2Int CharPos;
    [ReadOnly] public Random Randomizer;
    [ReadOnly] public int MapSize;

    public NativeArray<int> NodeQty;
    public NativeArray<JobNode> OutNodes;

    private int ApproximateDistance(int dx, int dy)
    {
        //From here: https://www.flipcode.com/archives/Fast_Approximate_Distance_Functions.shtml

        int min, max;

        if (dx < 0) dx = -dx;
        if (dy < 0) dy = -dy;

        if (dx < dy)
        {
            min = dx;
            max = dy;
        }
        else
        {
            min = dy;
            max = dx;
        }

        var approx = max * 1007 + min * 441;

        if (max < min << 4)
            approx -= max * 40;

        //add 512 for proper rounding
        return (approx + 512) >> 10;
    }

    public void Execute()
    {
        var costs = new NativeArray<int>(8, Allocator.Temp);
        var currentNode = Map[CharPos.y * MapSize + CharPos.x];

        while (!currentNode.GridPosition.Equals(TargetPos) && NodeQty[0] < 100)
        {
            costs[0] = ApproximateDistance( currentNode.Neighbour1.GridPosition.x - TargetPos.x,  currentNode.Neighbour1.GridPosition.y - TargetPos.y);
            costs[1] = ApproximateDistance(currentNode.Neighbour2.GridPosition.x - TargetPos.x, currentNode.Neighbour2.GridPosition.y - TargetPos.y);
            costs[2] = ApproximateDistance(currentNode.Neighbour3.GridPosition.x - TargetPos.x, currentNode.Neighbour3.GridPosition.y - TargetPos.y);
            costs[3] = currentNode.Neighbour4.CanWalk == 1 ? ApproximateDistance( currentNode.Neighbour4.GridPosition.x - TargetPos.x, currentNode.Neighbour4.GridPosition.y - TargetPos.y) : int.MaxValue;
            costs[4] = currentNode.Neighbour5.CanWalk == 1 ? ApproximateDistance(currentNode.Neighbour5.GridPosition.x - TargetPos.x, currentNode.Neighbour5.GridPosition.y - TargetPos.y) : int.MaxValue;
            costs[5] = currentNode.Neighbour6.CanWalk == 1 ? ApproximateDistance(currentNode.Neighbour6.GridPosition.x - TargetPos.x, currentNode.Neighbour6.GridPosition.y - TargetPos.y) : int.MaxValue;
            costs[6] = currentNode.Neighbour7.CanWalk == 1 ? ApproximateDistance(currentNode.Neighbour7.GridPosition.x - TargetPos.x, currentNode.Neighbour7.GridPosition.y - TargetPos.y) : int.MaxValue;
            costs[7] = currentNode.Neighbour8.CanWalk == 1 ? ApproximateDistance(currentNode.Neighbour8.GridPosition.x - TargetPos.x, currentNode.Neighbour8.GridPosition.y - TargetPos.y) : int.MaxValue;

            //var lowestCostNode = new CostNode(Vector2Int.zero, int.MaxValue);

            var lowestCost = int.MaxValue;
            var lowestCostPos = currentNode.Neighbour2.GridPosition;

            for (var i = 0; i < costs.Length; i++)
            {
                if (costs[i] < lowestCost && Randomizer.NextInt(0, 20) != 0)
                {
                    lowestCost = costs[i];
                    
                    switch (i)
                    {
                        case 0:
                            lowestCostPos = currentNode.Neighbour1.GridPosition;
                            break;
                        case 1:
                            lowestCostPos = currentNode.Neighbour2.GridPosition;
                            break;
                        case 2:
                            lowestCostPos = currentNode.Neighbour3.GridPosition;
                            break;
                        case 3:
                            lowestCostPos = currentNode.Neighbour4.GridPosition;
                            break;
                        case 4:
                            lowestCostPos = currentNode.Neighbour5.GridPosition;
                            break;
                        case 5:
                            lowestCostPos = currentNode.Neighbour6.GridPosition;
                            break;
                        case 6:
                            lowestCostPos = currentNode.Neighbour7.GridPosition;
                            break;
                        case 7:
                            lowestCostPos = currentNode.Neighbour8.GridPosition;
                            break;
                    }
                }   
            }
            
            currentNode = Map[lowestCostPos.y * MapSize + lowestCostPos.x];
            OutNodes[NodeQty[0]] = currentNode;
            NodeQty[0]++;
        }
        costs.Dispose();
    }
}