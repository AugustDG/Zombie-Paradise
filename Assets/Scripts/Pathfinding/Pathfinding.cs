using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Random = Unity.Mathematics.Random;
public class Pathfinding : MonoBehaviour
{
    private PathfindingJob[] _jobs;
    private readonly Random[] _randoms = new Random[10];
    private NativeList<JobHandle> _jobHandles;
    private NativeList<JobNode>[] _result;
    private System.Random _rnd;

    // Start is called before the first frame update
    private void Start()
    {
        _rnd = new System.Random();

        for (var i = 0; i < _randoms.Length; i++) _randoms[i] = new Random((uint)_rnd.Next());
    }

    private void FixedUpdate()
    {
        _jobs = new PathfindingJob[MapData.CharacterList.Count];
        _result = new NativeList<JobNode>[MapData.CharacterList.Count];
        _jobHandles = new NativeList<JobHandle>(MapData.CharacterList.Count, Allocator.Persistent);

        for (var i = 0; i < MapData.CharacterList.Count; i++)
        {
            if (MapData.CharacterList[i].Target == null || !MapData.CharacterList[i].hasTargetChanged)
            {
                MapData.CharacterList[i].canSkipCalculation = true;
                continue;
            }

            MapData.CharacterList[i].hasTargetChanged = false;

            _result[i] = new NativeList<JobNode>(50, Allocator.Persistent);

            _jobs[i] = new PathfindingJob
            {
                Map = MapData.NativeMap,
                CharPos = MapData.CharacterList[i].transform.position,
                TargetPos = MapData.CharacterList[i].targetPosition,
                Randomizer = _randoms[_rnd.Next(0, _randoms.Length - 1)],
                MapSizeX = MapData.MapSize.x,
                MapSizeY = MapData.MapSize.y,
                GridSizeX = MapManager.GridSizeX,
                GridSizeY = MapManager.GridSizeY,
                OutNodes = _result[i]
            };

            _jobHandles.Add(_jobs[i].Schedule());
        }

        JobHandle.CompleteAll(_jobHandles);

        for (var i = 0; i < _jobs.Length; i++)
        {
            if (MapData.CharacterList[i].canSkipCalculation)
            {
                if (_result[i].IsCreated) _result[i].Dispose();
                MapData.CharacterList[i].canSkipCalculation = false;
                continue;
            }

            MapData.CharacterList[i].waypoints.Clear();
            MapData.CharacterList[i].waypoints = new Queue<Node>(Node.ToNode(_result[i].ToArray()));

            _result[i].Dispose();
        }

        _jobHandles.Dispose();
    }

    private void OnApplicationQuit()
    {
        MapData.ClearLists();

        if (_jobHandles.IsCreated) _jobHandles.Dispose();

        foreach (var nativeList in _result)
        {
            if (nativeList.IsCreated) nativeList.Dispose();
        }
    }
}

[BurstCompile]
public struct PathfindingJob : IJob
{
    [NativeMatchesParallelForLength]
    [ReadOnly] public NativeArray<JobNode> Map;

    [ReadOnly] public Random Randomizer;
    [ReadOnly] public Vector2Int TargetPos;
    [ReadOnly] public Vector3 CharPos;
    [ReadOnly] public int GridSizeX, GridSizeY, MapSizeX, MapSizeY;

    [WriteOnly] public NativeList<JobNode> OutNodes;

    private JobNode NodeFromWorldPoint(Vector3 worldPos)
    {
        var percentX = math.clamp((worldPos.z + MapSizeX / 2f) / MapSizeX, 0, 1);
        var percentY = math.clamp((worldPos.x + MapSizeY / 2f) / MapSizeY, 0, 1);

        var x = (int)math.round((GridSizeX - 1) * percentX);
        var y = (int)math.round((GridSizeY - 1) * percentY);

        return Map[x * MapSizeX + y];
    }

    private JobNode IsClosestNeighbour(JobNeighbour neighbour, JobNode currentNode)
    {
        if (neighbour.IsBlocked) return currentNode;
        if ((TargetPos - neighbour.GridPosition).sqrMagnitude <
            (TargetPos - currentNode.GridPosition).sqrMagnitude && Randomizer.NextInt(2) == 0)
        {
            return Map[neighbour.GridPosition.y * MapSizeX + neighbour.GridPosition.x];
        }

        return currentNode;
    }

    public void Execute()
    {
        var currentNode = NodeFromWorldPoint(CharPos);

        var i = 0;

        while (currentNode.GridPosition != TargetPos)
        {
            //todo: test this value
            if (i > 5000) return;

            currentNode = IsClosestNeighbour(currentNode.Neighbour1, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour2, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour3, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour4, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour5, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour6, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour7, currentNode);
            currentNode = IsClosestNeighbour(currentNode.Neighbour8, currentNode);

            OutNodes.Add(currentNode);
            i++;
        }
    }
}