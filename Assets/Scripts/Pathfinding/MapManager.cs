using System;
using System.Collections.Generic;
using Characters;
using Unity.Collections;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public static int GridSizeX, GridSizeY;

    public float nodeRadius;
    public LayerMask obstacleMask;

    private float _nodeDiameter;

    [Space(5f)]
    [Header("Map Generation")]
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private GameObject[] borderTrees;
    [SerializeField] private GameObject[] mapTrees;
    [SerializeField] private GameObject[] props;
    [SerializeField] private HumanBehaviour human;
    [SerializeField] private Transform tilesParent;
    [SerializeField] private Transform borderTreesParent;
    [SerializeField] private Transform mapTreesParent;
    [SerializeField] private Transform propsParent;
    [SerializeField] private Transform humansParent;
    [SerializeField] private List<GameObject> spawnedTiles = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedBorderTrees = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedMapTrees = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedProps = new List<GameObject>();
    [SerializeField] private List<HumanBehaviour> spawnedHumans = new List<HumanBehaviour>();
    [SerializeField] private int localMapSize;

    public void Start()
    {
        transform.localScale = new Vector3(localMapSize / 10f, 1, localMapSize / 10f);
        MapData.MapSize = new Vector2Int(localMapSize, localMapSize);

        _nodeDiameter = nodeRadius * 2;
        GridSizeX = (MapData.MapSize.x / _nodeDiameter).RoundToInt();
        GridSizeY = (MapData.MapSize.y / _nodeDiameter).RoundToInt();

        CreateMap();
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
                Gizmos.DrawCube(nodeArray.worldPosition, Vector3.one * (_nodeDiameter - 0.1f));
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

    #region Editor Functions
    public void SpawnBorderTrees()
    {
        if (MapData.Map == null || MapData.Map.Length == 0) CreateMap();
        if (spawnedBorderTrees.Count > 0) DeleteBorderTrees();

        var i = 0;

        for (var x = 0; x < MapData.MapSize.x; x += 4)
        {
            for (var y = 0; y < MapData.MapSize.y; y += 4)
            {
                if (x < 2 || y < 2 || x > MapData.MapSize.x - 3 || y > MapData.MapSize.y - 3)
                {
                    var rndRot = Random.Range(0, 5);
                    var rot = new Vector3(0, 90 * rndRot, 0);

                    spawnedBorderTrees.Add(Instantiate(borderTrees[Random.Range(0, borderTrees.Length)], MapData.Map[x, y].worldPosition, Quaternion.Euler(rot), borderTreesParent));
                    i++;
                }
            }
        }
    }

    public void DeleteBorderTrees()
    {
        foreach (var tree in spawnedBorderTrees)
        {
            tree.gameObject.DestroyImmediate();
        }

        spawnedBorderTrees.Clear();
    }

    public void SpawnMapTrees()
    {
        if (MapData.Map == null || MapData.Map.Length == 0) CreateMap();
        if (spawnedMapTrees.Count > 0) DeleteMapTrees();

        foreach (var node in MapData.Map)
        {
            if (Random.Range(0, 250) == 0)
            {
                spawnedMapTrees.Add(Instantiate(mapTrees[Random.Range(0, mapTrees.Length)], node.worldPosition, Quaternion.identity, mapTreesParent));
            }
        }
    }

    public void DeleteMapTrees()
    {
        foreach (var tree in spawnedMapTrees)
        {
            tree.gameObject.DestroyImmediate();
        }

        spawnedMapTrees.Clear();
    }

    public void SpawnProps()
    {
        if (MapData.Map == null || MapData.Map.Length == 0) CreateMap();
        if (spawnedProps.Count > 0) DeleteProps();

        foreach (var node in MapData.Map)
        {
            if (Random.Range(0, 1000) == 0)
            {
                spawnedProps.Add(Instantiate(props[Random.Range(0, props.Length)], node.worldPosition, Quaternion.identity, propsParent));
            }
        }
    }

    public void DeleteProps()
    {
        foreach (var prop in spawnedProps)
        {
            prop.gameObject.DestroyImmediate();
        }

        spawnedProps.Clear();
    }

    public void SpawnHumans()
    {
        if (MapData.Map == null || MapData.Map.Length == 0) CreateMap();
        if (spawnedHumans.Count > 0) DeleteHumans();

        var chanceMult = 1;

        foreach (var node in MapData.Map)
        {
            var tempNode = node;

            if (tempNode.nodeType == NodeTypes.Blocked) continue;
            if (Random.Range(0, GridSizeX * chanceMult) == 0)
            {
                var spawns = 0;
                do
                {
                    foreach (var neighbour in tempNode.neighbours)
                    {
                        if (Random.Range(0, 10 * spawns) == 0 && neighbour.nodeType != NodeTypes.Blocked)
                        {
                            var spawnedBehaviour = Instantiate(human, neighbour.worldPosition, Quaternion.identity, humansParent);
                            spawnedBehaviour.SpawnHead();
                            spawnedHumans.Add(spawnedBehaviour);
                            spawns++;
                        }
                    }

                    tempNode = tempNode.neighbours[Random.Range(0, tempNode.neighbours.Length)];

                    chanceMult++;
                } while (spawns <= 6);
            }
        }
    }

    public void DeleteHumans()
    {
        foreach (var humanBehaviour in spawnedHumans)
        {
            humanBehaviour.gameObject.DestroyImmediate();
        }

        spawnedHumans.Clear();
    }

    public void SpawnTiles()
    {
        if (spawnedTiles.Count > 0) DeleteTiles();

        foreach (var tile in tiles)
        {
            tile.transform.localScale = new Vector3(1f / (localMapSize / 10f), 1, 1f / (localMapSize / 10f));
        }

        var i = 0;

        for (var x = -(localMapSize - 50) / 2; x <= (localMapSize - 50) / 2; x += 50)
        {
            for (var y = -(localMapSize - 50) / 2; y <= (localMapSize - 50) / 2; y += 50)
            {
                i++;

                if (x == 0 && y == 0) continue;

                var rndRot = Random.Range(0, 5);
                var rot = new Vector3(0, 90 * rndRot, 0);

                spawnedTiles.Add(Instantiate(tiles[RandomWeightedNumber()], new Vector3(x, 0, y), Quaternion.Euler(rot), tilesParent));
            }
        }
    }

    public void DeleteTiles()
    {
        foreach (var tile in spawnedTiles)
        {
            tile.gameObject.DestroyImmediate();
        }

        spawnedTiles.Clear();
    }

    private int RandomWeightedNumber()
    {
        var rnd = Random.value;

        if (rnd < 0.45f)
            return 0;
        else if (rnd < 0.9f)
            return 1;
        else if (rnd < 0.95f)
            return 2;
        else
            return 3;
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

                if (IsValidNodeNeighbour(xm1, yp1, out var temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xpos, yp1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xp1, yp1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xm1, ypos, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xp1, ypos, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xm1, ym1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xpos, ym1, out temp))
                    nodesToBe.Add(temp);
                if (IsValidNodeNeighbour(xp1, ym1, out temp))
                    nodesToBe.Add(temp);

                currentNode.neighbours = nodesToBe.ToArray();

                MapData.Map[x, y] = currentNode;
            }
        }

        CreateNativeMap();
    }

    private void CreateNativeMap()
    {
        if (MapData.NativeMap.IsCreated) MapData.NativeMap.Dispose();

        var tempMap = new JobNode[MapData.MapSize.x * MapData.MapSize.y];

        for (var i = 0; i < tempMap.Length; i++)
        {
            var tempNode = MapData.Map[i % MapData.MapSize.x, i / MapData.MapSize.y];

            tempMap[i] = Node.ToJobNode(tempNode);

            tempMap[i].Neighbour1 = Node.ToJobNeighbour(tempNode.neighbours[0]);
            tempMap[i].Neighbour2 = Node.ToJobNeighbour(tempNode.neighbours[1]);
            tempMap[i].Neighbour3 = Node.ToJobNeighbour(tempNode.neighbours[2]);
            tempMap[i].Neighbour4 = tempNode.neighbours.Length > 3 ? Node.ToJobNeighbour(tempNode.neighbours[3]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour4 = tempNode.neighbours.Length > 4 ? Node.ToJobNeighbour(tempNode.neighbours[4]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour4 = tempNode.neighbours.Length > 5 ? Node.ToJobNeighbour(tempNode.neighbours[5]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour4 = tempNode.neighbours.Length > 6 ? Node.ToJobNeighbour(tempNode.neighbours[6]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour4 = tempNode.neighbours.Length > 7 ? Node.ToJobNeighbour(tempNode.neighbours[7]) : new JobNeighbour(Vector2Int.zero, true);
        }

        MapData.NativeMap = new NativeArray<JobNode>(tempMap, Allocator.Persistent);
    }

    private bool IsValidNodeNeighbour(int x, int y, out Node result)
    {
        if (x >= 0 && x < MapData.MapSize.x && y >= 0 && y < MapData.MapSize.y)
        {
            result = MapData.Map[x, y];
            return true;
        }

        result = null;
        return false;
    }
    #endregion
}