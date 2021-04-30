using System;
using System.Collections;
using System.Collections.Generic;
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
        
        JobNodeMap.Dispose();

        HasInit = false;
        return true;
    }

    //Functions that convert different inputs to required ones for the scheduler
    public static IEnumerator PathfindToTarget(Vector2Int targetPosition, Vector3 startWorldPosition, [NotNull] Action<Queue<Node>> onPathFound)
    {
        return SchedulePathfinding(targetPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    public static IEnumerator PathfindToTarget(Node targetNode, Vector3 startWorldPosition, [NotNull] Action<Queue<Node>> onPathFound)
    {
        return SchedulePathfinding(targetNode.gridPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    public static IEnumerator PathfindToTarget(Vector3 targetWorldPos, Vector3 startWorldPosition, [NotNull] Action<Queue<Node>> onPathFound)
    {
        return SchedulePathfinding(Node.NodeFromWorldPoint(targetWorldPos, _mapSize, _gridSize, ref NodeMap).gridPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    public static IEnumerator PathfindToTarget(Transform targetTrans, Vector3 startWorldPosition, [NotNull] Action<Queue<Node>> onPathFound)
    {
        return SchedulePathfinding(Node.NodeFromWorldPoint(targetTrans.position, _mapSize, _gridSize, ref NodeMap).gridPosition, Node.NodeFromWorldPoint(startWorldPosition, _mapSize, _gridSize, ref NodeMap).gridPosition, onPathFound);
    }

    //schedules and completes each pathfinding job
    private static IEnumerator SchedulePathfinding(Vector2Int targetGridPosition, Vector2Int startGridPosition, [NotNull] Action<Queue<Node>> onPathFound)
    {
        if (!HasInit) yield break;

        var result = new NativeList<JobNode>(Allocator.Persistent);

        var job = new PathfindJob
        {
            Map = JobNodeMap,
            CharPos = startGridPosition,
            TargetPos = targetGridPosition,
            Randomizer = _random,
            MapSizeX = _mapSize.x,
            MapSizeY = _mapSize.y,
            OutNodes = result
        };

        job.Schedule().Complete();

        yield return null;

        onPathFound.Invoke(new Queue<Node>(Node.ToNodes(result.ToArray())));

        result.Dispose();
    }
}

[BurstCompile]
public struct PathfindJob : IJob
{
    [ReadOnly] public NativeArray<JobNode> Map;
    [ReadOnly] public Random Randomizer;
    [ReadOnly] public Vector2Int TargetPos;
    [ReadOnly] public Vector2Int CharPos;
    [ReadOnly] public int MapSizeX, MapSizeY;

    public NativeList<JobNode> OutNodes;

    private void IsClosestNeighbour(JobNeighbour neighbour, ref JobNode currentNode, out bool hasChanged)
    {
        if (!neighbour.CanWalk)
        {
            hasChanged = false;
            return;
        }

        if ((TargetPos - neighbour.GridPosition).sqrMagnitude <
            (TargetPos - currentNode.GridPosition).sqrMagnitude && Randomizer.NextInt(0, 3) == 0)
        {
            hasChanged = true;
            currentNode = Map[neighbour.GridPosition.y * MapSizeX + neighbour.GridPosition.x];
        }

        hasChanged = false;
    }

    public void Execute()
    {
        var currentNode = Map[CharPos.y * MapSizeX + CharPos.x];
        var tempNode = currentNode;
        
        var i = 0;

        while (currentNode.GridPosition != TargetPos && i < MapSizeX * 2)
        {
            IsClosestNeighbour(currentNode.Neighbour1, ref tempNode, out var hasChanged); //todo: check if this fixed the problem
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour2, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour3, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour4, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour5, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour6, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour7, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;
            IsClosestNeighbour(currentNode.Neighbour8, ref tempNode, out hasChanged);
            if (hasChanged) tempNode = currentNode;

            i++;
            OutNodes.Add(tempNode);
            currentNode = tempNode;
            
            continue;
            
            if (OutNodes.Length > 0 && OutNodes[OutNodes.Length - 1].GridPosition == tempNode.GridPosition)
            {
                if (currentNode.Neighbour1.CanWalk)
                    tempNode = Map[currentNode.Neighbour1.GridPosition.y *MapSizeX + currentNode.Neighbour1.GridPosition.x];
                if (currentNode.Neighbour2.CanWalk)
                    tempNode = Map[currentNode.Neighbour2.GridPosition.y *MapSizeX + currentNode.Neighbour2.GridPosition.x];
                if (currentNode.Neighbour3.CanWalk)
                    tempNode = Map[currentNode.Neighbour3.GridPosition.y *MapSizeX + currentNode.Neighbour3.GridPosition.x];
                if (currentNode.Neighbour4.CanWalk)
                    tempNode = Map[currentNode.Neighbour4.GridPosition.y *MapSizeX + currentNode.Neighbour4.GridPosition.x];
                if (currentNode.Neighbour5.CanWalk)
                    tempNode = Map[currentNode.Neighbour5.GridPosition.y *MapSizeX + currentNode.Neighbour5.GridPosition.x];
                if (currentNode.Neighbour6.CanWalk)
                    tempNode = Map[currentNode.Neighbour6.GridPosition.y *MapSizeX + currentNode.Neighbour6.GridPosition.x];
                if (currentNode.Neighbour7.CanWalk)
                    tempNode = Map[currentNode.Neighbour7.GridPosition.y *MapSizeX + currentNode.Neighbour7.GridPosition.x];
                if (currentNode.Neighbour8.CanWalk)
                    tempNode = Map[currentNode.Neighbour8.GridPosition.y *MapSizeX + currentNode.Neighbour8.GridPosition.x];
            }
            
            
        }
    }
}