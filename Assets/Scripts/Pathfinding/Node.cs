using System;
using UnityEngine;
using Utility;
[Serializable]
public class Node
{
    public Vector3 worldPosition;
    public Vector2Int gridPosition;
    public Node[] neighbours;
    public NodeTypes nodeType;

    public Node(Vector3 position, Vector2Int gridPosition, NodeTypes nodeType)
    {
        worldPosition = position;
        this.gridPosition = gridPosition;
        this.nodeType = nodeType;
    }

    public static JobNode ToJobNode(Node node)
    {
        return new JobNode(node.gridPosition, node.nodeType == NodeTypes.Blocked);
    }

    public static JobNeighbour ToJobNeighbour(Node node)
    {
        return new JobNeighbour(node.gridPosition, node.nodeType == NodeTypes.Blocked);
    }

    public static Node[] ToNode(JobNode[] jobNode)
    {
        var returnNodes = new Node[jobNode.Length];

        for (var i = 0; i < jobNode.Length; i++)
        {
            returnNodes[i] = MapData.Map[jobNode[i].GridPosition.x, jobNode[i].GridPosition.y];
        }

        return returnNodes;
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