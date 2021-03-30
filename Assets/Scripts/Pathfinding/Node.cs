using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Utilities.Extensions;
using Utility;
[Serializable]
public class Node
{
    public Vector3 worldPosition;
    public Vector2Int gridPosition;
    // ReSharper disable once InconsistentNaming
    [NonSerialized] public List<Node> neighbours;
    public NodeTypes nodeType;

    //Constructor
    public Node(Vector3 position, Vector2Int gridPosition, NodeTypes nodeType)
    {
        worldPosition = position;
        this.gridPosition = gridPosition;
        this.nodeType = nodeType;
    }

    //Utility methods
    public static JobNode ToJobNode(Node node)
    {
        return new JobNode(node.gridPosition, node.nodeType == NodeTypes.Blocked);
    }

    public static JobNeighbour ToJobNeighbour(Node node)
    {
        return new JobNeighbour(node.gridPosition, node.nodeType == NodeTypes.Blocked);
    }

    public static Node[] ToNodes(JobNode[] jobNode)
    {
        var returnNodes = new Node[jobNode.Length];

        for (var i = 0; i < jobNode.Length; i++)
        {
            returnNodes[i] = MapData.Map[jobNode[i].GridPosition.x, jobNode[i].GridPosition.y];
        }

        return returnNodes;
    }

    public static Node NodeFromWorldPoint(Vector3 wPoint, Vector2Int mapSize, Vector2Int gridSize, [NotNull] ref Node[,] map)
    {
        var percentX = Mathf.Clamp01((wPoint.x + mapSize.x / 2f) / mapSize.x);
        var percentY = Mathf.Clamp01((wPoint.z + mapSize.y / 2f) / mapSize.y);

        var x = ((gridSize.x - 1) * percentX).RoundToInt();
        var y = ((gridSize.y - 1) * percentY).RoundToInt();

        return map[x, y];
    }
    
    //To shorten the code, for my own use
    public static Node NodeFromWorldPoint(Vector3 wPoint)
    {
        var percentX = Mathf.Clamp01((wPoint.x + MapData.MapSize.x / 2f) / MapData.MapSize.x);
        var percentY = Mathf.Clamp01((wPoint.z + MapData.MapSize.y / 2f) / MapData.MapSize.y);

        var x = ((PathfindingManager.GridSizeX - 1) * percentX).RoundToInt();
        var y = ((PathfindingManager.GridSizeY - 1) * percentY).RoundToInt();

        return MapData.Map[x, y];
    }
}

public struct JobNode
{
    public Vector2Int GridPosition;
    public JobNeighbour Neighbour1;
    public JobNeighbour Neighbour2;
    public JobNeighbour Neighbour3;
    public JobNeighbour Neighbour4;
    public JobNeighbour Neighbour5;
    public JobNeighbour Neighbour6;
    public JobNeighbour Neighbour7;
    public JobNeighbour Neighbour8;
    public bool IsBlocked;

    public JobNode(Vector2Int gridPosition, bool isBlocked)
    {
        GridPosition = gridPosition;
        IsBlocked = isBlocked;
        Neighbour1 = new JobNeighbour();
        Neighbour2 = new JobNeighbour();
        Neighbour3 = new JobNeighbour();
        Neighbour4 = new JobNeighbour();
        Neighbour5 = new JobNeighbour();
        Neighbour6 = new JobNeighbour();
        Neighbour7 = new JobNeighbour();
        Neighbour8 = new JobNeighbour();
    }

    public JobNode(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
        IsBlocked = true;
        Neighbour1 = new JobNeighbour();
        Neighbour2 = new JobNeighbour();
        Neighbour3 = new JobNeighbour();
        Neighbour4 = new JobNeighbour();
        Neighbour5 = new JobNeighbour();
        Neighbour6 = new JobNeighbour();
        Neighbour7 = new JobNeighbour();
        Neighbour8 = new JobNeighbour();
    }
}

public struct JobNeighbour
{
    public Vector2Int GridPosition;
    public bool IsBlocked;

    public JobNeighbour(Vector2Int gridPosition, bool isBlocked)
    {
        GridPosition = gridPosition;
        IsBlocked = isBlocked;
    }
}

public enum NodeTypes
{
    Blocked,
    Free
}