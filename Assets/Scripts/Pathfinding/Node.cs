using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Utilities.Extensions;
using Utility;
[Serializable]
public class Node
{
    public Vector3 worldPosition;
    public Vector2Int gridPosition;
    [NonSerialized] public List<Node> neighbours; // ReSharper disable once InconsistentNaming
    public bool canSpawn;
    public bool canWalk;

    //Constructor
    public Node(Vector3 position, Vector2Int gridPosition, bool canWalk, bool canSpawn)
    {
        worldPosition = position;
        this.gridPosition = gridPosition;
        this.canWalk = canWalk;
        this.canSpawn = canSpawn;
    }

    //Utility methods
    public static int ApproximateDistance(int dx, int dy)
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
    public static JobNode ToJobNode(Node node)
    {
        return new JobNode(node.gridPosition, node.canWalk);
    }

    public static NodeNeighbour ToJobNeighbour(Node node)
    {
        return new NodeNeighbour(node.gridPosition, node.canWalk ? 1 : 0);
    }

    public static Node[] ToNodes(JobNode[] jobNodes, int length)
    {
        var returnNodes = new Node[length];

        for (var i = 0; i < length; i++)
        {
            returnNodes[i] = MapData.Map[jobNodes[i].GridPosition.x, jobNodes[i].GridPosition.y];
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
    public NodeNeighbour Neighbour1;
    public NodeNeighbour Neighbour2;
    public NodeNeighbour Neighbour3;
    public NodeNeighbour Neighbour4;
    public NodeNeighbour Neighbour5;
    public NodeNeighbour Neighbour6;
    public NodeNeighbour Neighbour7;
    public NodeNeighbour Neighbour8;
    //public bool CanWalk;

    public JobNode(Vector2Int gridPosition, bool canWalk)
    {
        GridPosition = gridPosition;
        //CanWalk = canWalk;
        Neighbour1 = new NodeNeighbour();
        Neighbour2 = new NodeNeighbour();
        Neighbour3 = new NodeNeighbour();
        Neighbour4 = new NodeNeighbour();
        Neighbour5 = new NodeNeighbour();
        Neighbour6 = new NodeNeighbour();
        Neighbour7 = new NodeNeighbour();
        Neighbour8 = new NodeNeighbour();
    }

    public JobNode(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
        //CanWalk = true;
        Neighbour1 = new NodeNeighbour();
        Neighbour2 = new NodeNeighbour();
        Neighbour3 = new NodeNeighbour();
        Neighbour4 = new NodeNeighbour();
        Neighbour5 = new NodeNeighbour();
        Neighbour6 = new NodeNeighbour();
        Neighbour7 = new NodeNeighbour();
        Neighbour8 = new NodeNeighbour();
    }
}

public struct NodeNeighbour
{
    public Vector2Int GridPosition;
    public int CanWalk;

    public NodeNeighbour(Vector2Int gridPosition, int canWalk)
    {
        GridPosition = gridPosition;
        CanWalk = canWalk;
    }
}

public struct CostNode
{
    public Vector2Int GridPosition;
    public int Cost;

    public CostNode(Vector2Int gridPosition, int cost)
    {
        GridPosition = gridPosition;
        Cost = cost;
    }
}

