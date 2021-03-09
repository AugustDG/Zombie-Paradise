using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Utilities.Extensions;
using Utility;
[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    public float nodeRadius;
    public LayerMask obstacleMask;

    public static int GridSizeX, GridSizeY;

    private float _nodeDiameter;

    [Space(5f)]
    [Header("Map Generation")]
    [SerializeField] private Transform[] trees;
    [SerializeField] private Transform treesParent;
    [SerializeField] private List<Transform> spawnedTrees;
    [SerializeField] private int localMapSize;

    private void Start()
    {
        _nodeDiameter = nodeRadius * 2;
        GridSizeX = (MapData.MapSize.x / _nodeDiameter).RoundToInt();
        GridSizeY = (MapData.MapSize.y / _nodeDiameter).RoundToInt();

        CreateMap();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        transform.localScale = new Vector3(localMapSize / 10f, 1, localMapSize / 10f);
        MapData.MapSize = new Vector2Int(localMapSize, localMapSize);
        #endif
    }

    private void OnApplicationQuit()
    {
        MapData.NativeMap.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(MapData.MapSize.x, 10, MapData.MapSize.y));

        if (MapData.Map == null || MapData.Map.Length == 0) return;

        var style = new GUIStyle
        {
            fontSize = 10,
            stretchWidth = false,
            stretchHeight = false
        };

        foreach (var nodeArray in MapData.Map)
        {
            if (nodeArray != null)
            {
                //Handles.Label(nodeArray.worldPosition, $"{nodeArray.gridPosition} {Environment.NewLine} {nodeArray.neighbours[2].gridPosition}", style);
                Gizmos.color = nodeArray.nodeType == NodeTypes.Blocked ? Color.red : Color.white;
                //Gizmos.DrawCube(nodeArray.worldPosition, Vector3.one * (_nodeDiameter - 0.1f));
            }
        }

        foreach (var jobNode in MapData.NativeMap)
        {
            //Handles.Label(((Vector2)jobNode.GridPosition).TransformTo2DVector3() - new Vector3(12f, 0, 12f), $"{jobNode.GridPosition} {Environment.NewLine} {jobNode.Neighbour1.GridPosition}", style);
        }
    }

    public static JobNode NodeFromWorldPointTest(Vector3 worldPos)
    {
        var percentX = Mathf.Clamp01((worldPos.x + MapData.MapSize.x / 2f) / MapData.MapSize.x);
        var percentY = Mathf.Clamp01((worldPos.z + MapData.MapSize.y / 2f) / MapData.MapSize.y);

        var x = ((GridSizeX - 1) * percentX).RoundToInt();
        var y = ((GridSizeY - 1) * percentY).RoundToInt();

        return MapData.NativeMap[y * MapData.MapSize.x + x];
    }

    public static Node NodeFromWorldPoint(Vector3 worldPos)
    {
        var percentX = Mathf.Clamp01((worldPos.x + MapData.MapSize.x / 2f) / MapData.MapSize.x);
        var percentY = Mathf.Clamp01((worldPos.z + MapData.MapSize.y / 2f) / MapData.MapSize.y);

        var x = ((GridSizeX - 1) * percentX).RoundToInt();
        var y = ((GridSizeY - 1) * percentY).RoundToInt();

        return MapData.Map[x, y];
    }
    public void SpawnTrees()
    {
        if (MapData.Map == null || MapData.Map.Length == 0) CreateMap();
        if (spawnedTrees.Count > 0) DeleteTrees();

        var i = 0;

        for (var x = 0; x < MapData.MapSize.x; x += 4)
        {
            for (var y = 0; y < MapData.MapSize.y; y += 4)
            {
                if (x < 2 || y < 2 || x > MapData.MapSize.x - 3 || y > MapData.MapSize.y - 3)
                {
                    var rndRot = Random.Range(0, 5);
                    var rot = new Vector3(0, 90 * rndRot, 0);

                    spawnedTrees.Add(Instantiate(trees[Random.Range(0, trees.Length)], MapData.Map[x, y].worldPosition, Quaternion.Euler(rot), treesParent));
                    i++;
                }
            }
        }

        print($"Trees planted: {i}!");
    }

    public void DeleteTrees()
    {
        foreach (var tree in spawnedTrees)
        {
            tree.gameObject.DestroyImmediate();
        }

        spawnedTrees.Clear();
    }

    private void CreateNativeMap()
    {
        var tempMap = new JobNode[MapData.MapSize.x * MapData.MapSize.y];

        for (var i = 0; i < tempMap.Length; i++)
        {
            Node tempNode;

            tempNode = MapData.Map[i % MapData.MapSize.x, i / MapData.MapSize.y];

            tempMap[i] = Node.ToJobNode(tempNode);

            tempMap[i].Neighbour1 = Node.ToJobNeighbour(tempNode.neighbours[0]);
            tempMap[i].Neighbour2 = Node.ToJobNeighbour(tempNode.neighbours[1]);
            tempMap[i].Neighbour3 = Node.ToJobNeighbour(tempNode.neighbours[2]);
            if (tempNode.neighbours.Length > 3) tempMap[i].Neighbour4 = Node.ToJobNeighbour(tempNode.neighbours[3]);
            if (tempNode.neighbours.Length > 4) tempMap[i].Neighbour5 = Node.ToJobNeighbour(tempNode.neighbours[4]);
            if (tempNode.neighbours.Length > 5) tempMap[i].Neighbour6 = Node.ToJobNeighbour(tempNode.neighbours[5]);
            if (tempNode.neighbours.Length > 6) tempMap[i].Neighbour7 = Node.ToJobNeighbour(tempNode.neighbours[6]);
            if (tempNode.neighbours.Length > 7) tempMap[i].Neighbour8 = Node.ToJobNeighbour(tempNode.neighbours[7]);
        }

        MapData.NativeMap = new NativeArray<JobNode>(tempMap, Allocator.Persistent);
    }

    public void CreateMap()
    {
        MapData.Map = new Node[MapData.MapSize.x, MapData.MapSize.y];

        var worldBL =
            transform.position - Vector3.right * MapData.MapSize.x / 2 - Vector3.forward * MapData.MapSize.y / 2; //world's bottom left

        for (var x = 0; x < MapData.MapSize.x; x++)
        {
            for (var y = 0; y < MapData.MapSize.y; y++)
            {
                var worldPoint = worldBL + Vector3.right * (x * _nodeDiameter + nodeRadius) +
                    Vector3.forward * (y * _nodeDiameter + nodeRadius);

                var walkable = !Physics.CheckSphere(worldPoint, nodeRadius * 1.5f, obstacleMask);

                MapData.Map[x, y] = new Node(worldPoint, new Vector2Int(x, y),
                    walkable ? NodeTypes.Free : NodeTypes.Blocked);
            }
        }

        AssignNeighbours();
    }

    private void AssignNeighbours()
    {
        for (var x = 0; x < MapData.MapSize.x; x++)
        {
            for (var y = 0; y < MapData.MapSize.y; y++)
            {
                var nodesToBe = new List<Node>();

                var currentNode = MapData.Map[x, y];

                var xpos = currentNode.gridPosition.x;
                var ypos = currentNode.gridPosition.y;
                var xm1 = currentNode.gridPosition.x - 1;
                var xp1 = currentNode.gridPosition.x + 1;
                var ym1 = currentNode.gridPosition.y - 1;
                var yp1 = currentNode.gridPosition.y + 1;

                if (IsValidNeighbour(xm1, yp1, out var temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xpos, yp1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xp1, yp1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xm1, ypos, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xp1, ypos, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xm1, ym1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xpos, ym1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNeighbour(xp1, ym1, out temp))
                    nodesToBe.Add(temp);

                currentNode.neighbours = nodesToBe.ToArray();

                MapData.Map[x, y] = currentNode;
            }
        }

        CreateNativeMap();
    }

    private bool IsValidNeighbour(int x, int y, out Node result)
    {
        if (x >= 0 && x < MapData.MapSize.x && y >= 0 && y < MapData.MapSize.y)
        {
            result = MapData.Map[x, y];
            return true;
        }

        result = null;
        return false;
    }
}