using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Utilities.Extensions;
using Utility;
using Random = UnityEngine.Random;

public class PathfindingManager : MonoBehaviour
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
    [SerializeField] private GameObject human;
    [SerializeField] private GameObject humanObjective;
    [SerializeField] private Transform tilesParent;
    [SerializeField] private Transform borderTreesParent;
    [SerializeField] private Transform mapTreesParent;
    [SerializeField] private Transform propsParent;
    [SerializeField] private Transform humansParent;
    [SerializeField] private List<GameObject> spawnedTiles = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedBorderTrees = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedMapTrees = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedProps = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedHumans = new List<GameObject>();
    [SerializeField] private int localMapSize;
    
    private NativeArray<JobNode> _nativeMap;

    public void Awake()
    {
        MapData.ClearLists();
    }
    public void Start()
    {
        CreateMap();

        /*_random = Unity.Mathematics.Random.CreateFromIndex((uint)Random.Range(0, 10000));

        _pathfindingTask = new Task(PathfindIteration(), false);
        _pathfindingTask.Finished += OnTaskFinished;
        _pathfindingTask.Start();*/

        var initialHumans = GameObject.FindGameObjectsWithTag("Human");

        foreach (var humanObject in initialHumans)
        {
            var hBehaviour = humanObject.GetComponent<HumanBehaviour>();
            var instancedObjective = Instantiate(humanObjective, humanObject.transform.position, Quaternion.identity, humansParent).transform;
            hBehaviour.objective = instancedObjective;
            hBehaviour.Target = new Target(instancedObjective);
            MapData.HumanList.Add(hBehaviour);
        }
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
                /*Gizmos.color = nodeArray.nodeType == NodeTypes.Blocked ? Color.red : Color.white;
                Gizmos.DrawCube(nodeArray.worldPosition + new Vector3(0, 2, 0), Vector3.one * (_nodeDiameter - 0.1f));*/
            }
        }

        /*foreach (var jobNode in _nativeMap)
        {
            Handles.Label(((Vector2)jobNode.GridPosition).TransformTo2DVector3() - new Vector3(12f, 0, 12f), $"{jobNode.GridPosition} {Environment.NewLine} {jobNode.Neighbour1.GridPosition}", style);
        }*/
    }

    public JobNode NodeFromWorldPointTest(Vector3 worldPos)
    {
        var percentX = Mathf.Clamp01((worldPos.x + MapData.MapSize.x / 2f) /MapData. MapSize.x);
        var percentY = Mathf.Clamp01((worldPos.z + MapData.MapSize.y / 2f) / MapData.MapSize.y);

        var x = ((GridSizeX - 1) * percentX).RoundToInt();
        var y = ((GridSizeY - 1) * percentY).RoundToInt();

        return _nativeMap[y * MapData.MapSize.x + x];
    }

    public static Node NodeFromWorldPoint(Vector3 worldPos)
    {
        var percentX = Mathf.Clamp01((worldPos.x + MapData.MapSize.x / 2f) / MapData.MapSize.x);
        var percentY = Mathf.Clamp01((worldPos.z + MapData.MapSize.y / 2f) / MapData.MapSize.y);

        var x = ((GridSizeX - 1) * percentX).RoundToInt();
        var y = ((GridSizeY - 1) * percentY).RoundToInt();

        return MapData.Map[x, y];
    }
    
    public void CreateMap()
    {
        MapData.MapSize = new Vector2Int(localMapSize, localMapSize);
        
        _nodeDiameter = nodeRadius * 2;
        GridSizeX = (MapData.MapSize.x / _nodeDiameter).RoundToInt();
        GridSizeY = (MapData.MapSize.y / _nodeDiameter).RoundToInt();
        
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
                MapData.Map[x, y].neighbours = new List<Node>();

                var xpos = MapData.Map[x, y].gridPosition.x;
                var ypos = MapData.Map[x, y].gridPosition.y;
                var xm1 = MapData.Map[x, y].gridPosition.x - 1;
                var xp1 = MapData.Map[x, y].gridPosition.x + 1;
                var ym1 = MapData.Map[x, y].gridPosition.y - 1;
                var yp1 = MapData.Map[x, y].gridPosition.y + 1;

                if (IsValidNodeNeighbour(xm1, yp1))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xm1, yp1]);
                if (IsValidNodeNeighbour(xpos, yp1))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xpos, yp1]);
                if (IsValidNodeNeighbour(xp1, yp1))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xp1, yp1]);
                if (IsValidNodeNeighbour(xm1, ypos))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xm1, ypos]);
                if (IsValidNodeNeighbour(xp1, ypos))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xp1, ypos]);
                if (IsValidNodeNeighbour(xm1, ym1))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xm1, ym1]);
                if (IsValidNodeNeighbour(xpos, ym1))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xpos, ym1]);
                if (IsValidNodeNeighbour(xp1, ym1))
                    MapData.Map[x, y].neighbours.Add(MapData.Map[xp1, ym1]);
            }
        }

        CreateNativeMap();
    }

    private bool IsValidNodeNeighbour(int x, int y)
    {
        return x >= 0 && x < MapData.MapSize.x && y >= 0 && y < MapData.MapSize.y;
    }

    private void CreateNativeMap()
    {
        if (_nativeMap.IsCreated) _nativeMap.Dispose();

        var tempMap = new JobNode[MapData.MapSize.x * MapData.MapSize.y];

        for (var i = 0; i < tempMap.Length; i++)
        {
            var tempNode = MapData.Map[i % MapData.MapSize.x, i / MapData.MapSize.y];

            tempMap[i] = Node.ToJobNode(tempNode);

            tempMap[i].Neighbour1 = Node.ToJobNeighbour(tempNode.neighbours[0]);
            tempMap[i].Neighbour2 = Node.ToJobNeighbour(tempNode.neighbours[1]);
            tempMap[i].Neighbour3 = Node.ToJobNeighbour(tempNode.neighbours[2]);
            tempMap[i].Neighbour4 = tempNode.neighbours.Count > 3 ? Node.ToJobNeighbour(tempNode.neighbours[3]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour5 = tempNode.neighbours.Count > 4 ? Node.ToJobNeighbour(tempNode.neighbours[4]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour6 = tempNode.neighbours.Count > 5 ? Node.ToJobNeighbour(tempNode.neighbours[5]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour7 = tempNode.neighbours.Count > 6 ? Node.ToJobNeighbour(tempNode.neighbours[6]) : new JobNeighbour(Vector2Int.zero, true);
            tempMap[i].Neighbour8 = tempNode.neighbours.Count > 7 ? Node.ToJobNeighbour(tempNode.neighbours[7]) : new JobNeighbour(Vector2Int.zero, true);
        }

        _nativeMap = new NativeArray<JobNode>(tempMap, Allocator.Persistent);

        Pathfinder.Init(new Vector2Int(localMapSize, localMapSize), new Vector2Int(GridSizeX, GridSizeY), MapData.Map, _nativeMap);
    }

    #region Map Functions
    #if UNITY_EDITOR
    public void SpawnBorderTrees()
    {
        if (MapData.Map == null || MapData.Map.Length == 0) CreateMap();
        if (spawnedBorderTrees.Count > 0) DeleteBorderTrees();

        var i = 0;

        for (var x = 0; x <MapData. MapSize.x; x += 4)
        {
            for (var y = 0; y < MapData.MapSize.y; y += 4)
            {
                if (x < 2 || y < 2 || x > MapData.MapSize.x - 3 || y > MapData.MapSize.y - 3)
                {
                    var rndRot = Random.Range(0, 5);
                    var rot = new Vector3(0, 90 * rndRot, 0);

                    var treeToSpawn = borderTrees[Random.Range(0, borderTrees.Length)];
                    treeToSpawn.transform.position = MapData.Map[x, y].worldPosition;
                    treeToSpawn.transform.rotation = Quaternion.Euler(rot);
                    spawnedBorderTrees.Add((GameObject)PrefabUtility.InstantiatePrefab(treeToSpawn, borderTreesParent));

                    i++;
                }
            }
        }
    }

    public void DeleteBorderTrees()
    {
        foreach (var tree in spawnedBorderTrees)
        {
            tree.DestroyImmediate();
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
                var rndRot = Random.Range(0, 5);
                var rot = new Vector3(0, 90 * rndRot, 0);

                var treeToSpawn = mapTrees[Random.Range(0, mapTrees.Length)];
                treeToSpawn.transform.position = node.worldPosition;
                treeToSpawn.transform.rotation = Quaternion.Euler(rot);

                spawnedMapTrees.Add((GameObject)PrefabUtility.InstantiatePrefab(treeToSpawn, mapTreesParent));
            }
        }

        CreateMap();
    }

    public void DeleteMapTrees()
    {
        foreach (var tree in spawnedMapTrees)
        {
            tree.DestroyImmediate();
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
                var rndRot = Random.Range(0, 5);
                var rot = new Vector3(0, 90 * rndRot, 0);

                var propToSpawn = props[Random.Range(0, props.Length)];
                propToSpawn.transform.position = node.worldPosition;
                propToSpawn.transform.rotation = Quaternion.Euler(rot);

                spawnedProps.Add((GameObject)PrefabUtility.InstantiatePrefab(propToSpawn, propsParent));
            }
        }
    }

    public void DeleteProps()
    {
        foreach (var prop in spawnedProps)
        {
            prop.DestroyImmediate();
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
                            var rndRot = Random.Range(0, 5);
                            var rot = new Vector3(0, 90 * rndRot, 0);

                            var humanToSpawn = human;
                            humanToSpawn.transform.position = neighbour.worldPosition;
                            humanToSpawn.transform.rotation = Quaternion.identity;

                            spawnedHumans.Add((GameObject)PrefabUtility.InstantiatePrefab(humanToSpawn, humansParent));
                            spawns++;
                        }
                    }

                    tempNode = tempNode.neighbours[Random.Range(0, tempNode.neighbours.Count)];

                    chanceMult++;
                } while (spawns <= 6);
            }
        }
    }

    public void DeleteHumans()
    {
        foreach (var humanBehaviour in spawnedHumans)
        {
            humanBehaviour.DestroyImmediate();
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
                
                var tileToSpawn = tiles[ RandomWeightedNumber()];
                tileToSpawn.transform.position = new Vector3(x/25f, 0, y/25f);
                tileToSpawn.transform.rotation = Quaternion.Euler(rot);
                
                spawnedTiles.Add((GameObject)PrefabUtility.InstantiatePrefab(tileToSpawn, tilesParent));
            }
        }
    }

    public void DeleteTiles()
    {
        foreach (var tile in spawnedTiles)
        {
            tile.DestroyImmediate();
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
    #endif
    #endregion
}