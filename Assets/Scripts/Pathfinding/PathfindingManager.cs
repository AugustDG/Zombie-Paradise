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
using static Utility.MapData;
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

    public static bool MapHasGenerated;
    private PathfindingJob[] _jobs;
    private Unity.Mathematics.Random _random;
    private NativeList<JobHandle> _jobHandles;
    private NativeList<JobNode>[] _result;
    private Task _pathfindingTask;
    private NativeArray<JobNode> _nativeMap;

    private DateTime _timeThen;

    public void Awake()
    {
        MapHasGenerated = false;
        ClearLists();
    }
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => !_nativeMap.IsCreated);

        CreateMap();

        _random = Unity.Mathematics.Random.CreateFromIndex((uint)Random.Range(0, 10000));

        _pathfindingTask = new Task(PathfindIteration(), false);
        _pathfindingTask.Finished += OnTaskFinished;
        _pathfindingTask.Start();

        var initialHumans = GameObject.FindGameObjectsWithTag("Human");

        foreach (var humanObject in initialHumans)
        {
            var hBehaviour = humanObject.GetComponent<HumanBehaviour>();
            var instancedObjective = Instantiate(humanObjective, humanObject.transform.position, Quaternion.identity, humansParent).transform;
            hBehaviour.objective = instancedObjective;
            hBehaviour.Target = new Target(instancedObjective);
            HumanList.Add(hBehaviour);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(MapSize.x, 10, MapSize.y));

        if (Map == null || Map.Length == 0) return;

        var style = new GUIStyle
        {
            fontSize = 10,
            stretchWidth = false,
            stretchHeight = false
        };

        foreach (var nodeArray in Map)
        {
            if (nodeArray != null)
            {
                //Handles.Label(nodeArray.worldPosition, $"{nodeArray.gridPosition} {Environment.NewLine} {nodeArray.neighbours[2].gridPosition}", style);
                Gizmos.color = nodeArray.nodeType == NodeTypes.Blocked ? Color.red : Color.white;
                Gizmos.DrawCube(nodeArray.worldPosition + new Vector3(0, 2, 0), Vector3.one * (_nodeDiameter - 0.1f));
            }
        }

        //foreach (var jobNode in MapData.NativeMap)
        //{
        //Handles.Label(((Vector2)jobNode.GridPosition).TransformTo2DVector3() - new Vector3(12f, 0, 12f), $"{jobNode.GridPosition} {Environment.NewLine} {jobNode.Neighbour1.GridPosition}", style);
        //}
    }

    public JobNode NodeFromWorldPointTest(Vector3 worldPos)
    {
        var percentX = Mathf.Clamp01((worldPos.x + MapSize.x / 2f) / MapSize.x);
        var percentY = Mathf.Clamp01((worldPos.z + MapSize.y / 2f) / MapSize.y);

        var x = ((GridSizeX - 1) * percentX).RoundToInt();
        var y = ((GridSizeY - 1) * percentY).RoundToInt();

        return _nativeMap[y * MapSize.x + x];
    }

    public static Node NodeFromWorldPoint(Vector3 worldPos)
    {
        var percentX = Mathf.Clamp01((worldPos.x + MapSize.x / 2f) / MapSize.x);
        var percentY = Mathf.Clamp01((worldPos.z + MapSize.y / 2f) / MapSize.y);

        var x = ((GridSizeX - 1) * percentX).RoundToInt();
        var y = ((GridSizeY - 1) * percentY).RoundToInt();

        return Map[x, y];
    }

    #region Map Functions
    public void CreateMap()
    {
        _timeThen = DateTime.Now;
        MapSize = new Vector2Int(localMapSize, localMapSize);

        _nodeDiameter = nodeRadius * 2;
        GridSizeX = (MapSize.x / _nodeDiameter).RoundToInt();
        GridSizeY = (MapSize.y / _nodeDiameter).RoundToInt();

        Map = new Node[MapSize.x, MapSize.y];

        var worldBL =
            transform.position - Vector3.right * MapSize.x / 2 - Vector3.forward * MapSize.y / 2; //world's bottom left

        for (var x = 0; x < MapSize.x; x++)
        {
            for (var y = 0; y < MapSize.y; y++)
            {
                var worldPoint = worldBL + Vector3.right * (x * _nodeDiameter + nodeRadius) +
                    Vector3.forward * (y * _nodeDiameter + nodeRadius);

                var walkable = !Physics.CheckSphere(worldPoint, nodeRadius * 1.5f, obstacleMask);

                Map[x, y] = new Node(worldPoint, new Vector2Int(x, y),
                    walkable ? NodeTypes.Free : NodeTypes.Blocked);
            }
        }

        AssignNeighbours();
    }

    private void AssignNeighbours()
    {
        for (var x = 0; x < MapSize.x; x++)
        {
            for (var y = 0; y < MapSize.y; y++)
            {
                Map[x, y].neighbours = new List<Node>();

                var xpos = Map[x, y].gridPosition.x;
                var ypos = Map[x, y].gridPosition.y;
                var xm1 = Map[x, y].gridPosition.x - 1;
                var xp1 = Map[x, y].gridPosition.x + 1;
                var ym1 = Map[x, y].gridPosition.y - 1;
                var yp1 = Map[x, y].gridPosition.y + 1;

                if (IsValidNodeNeighbour(xm1, yp1))
                    Map[x, y].neighbours.Add(Map[xm1, yp1]);
                if (IsValidNodeNeighbour(xpos, yp1))
                    Map[x, y].neighbours.Add(Map[xpos, yp1]);
                if (IsValidNodeNeighbour(xp1, yp1))
                    Map[x, y].neighbours.Add(Map[xp1, yp1]);
                if (IsValidNodeNeighbour(xm1, ypos))
                    Map[x, y].neighbours.Add(Map[xm1, ypos]);
                if (IsValidNodeNeighbour(xp1, ypos))
                    Map[x, y].neighbours.Add(Map[xp1, ypos]);
                if (IsValidNodeNeighbour(xm1, ym1))
                    Map[x, y].neighbours.Add(Map[xm1, ym1]);
                if (IsValidNodeNeighbour(xpos, ym1))
                    Map[x, y].neighbours.Add(Map[xpos, ym1]);
                if (IsValidNodeNeighbour(xp1, ym1))
                    Map[x, y].neighbours.Add(Map[xp1, ym1]);
            }
        }

        CreateNativeMap();
    }

    private bool IsValidNodeNeighbour(int x, int y)
    {
        return x >= 0 && x < MapSize.x && y >= 0 && y < MapSize.y;
    }

    private void CreateNativeMap()
    {
        if (_nativeMap.IsCreated) _nativeMap.Dispose();

        var tempMap = new JobNode[MapSize.x * MapSize.y];

        for (var i = 0; i < tempMap.Length; i++)
        {
            var tempNode = Map[i % MapSize.x, i / MapSize.y];

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

        print((DateTime.Now - _timeThen).Milliseconds);
        MapHasGenerated = true;
    }

    public void SpawnBorderTrees()
    {
        if (Map == null || Map.Length == 0) CreateMap();
        if (spawnedBorderTrees.Count > 0) DeleteBorderTrees();

        var i = 0;

        for (var x = 0; x < MapSize.x; x += 4)
        {
            for (var y = 0; y < MapSize.y; y += 4)
            {
                if (x < 2 || y < 2 || x > MapSize.x - 3 || y > MapSize.y - 3)
                {
                    var rndRot = Random.Range(0, 5);
                    var rot = new Vector3(0, 90 * rndRot, 0);

                    var treeToSpawn = borderTrees[Random.Range(0, borderTrees.Length)];
                    treeToSpawn.transform.position = Map[x, y].worldPosition;
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
        if (Map == null || Map.Length == 0) CreateMap();
        if (spawnedMapTrees.Count > 0) DeleteMapTrees();

        foreach (var node in Map)
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
        if (Map == null || Map.Length == 0) CreateMap();
        if (spawnedProps.Count > 0) DeleteProps();

        foreach (var node in Map)
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
        if (Map == null || Map.Length == 0) CreateMap();
        if (spawnedHumans.Count > 0) DeleteHumans();

        var chanceMult = 1;

        foreach (var node in Map)
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
                            humanToSpawn.GetComponent<HumanBehaviour>().SpawnHead();

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
    #endregion

    #region Pathfinding Functions
    private IEnumerator PathfindIteration()
    {
        _jobs = new PathfindingJob[PathfindingList.Count];
        _result = new NativeList<JobNode>[PathfindingList.Count];

        using (_jobHandles = new NativeList<JobHandle>(PathfindingList.Count, Allocator.Persistent))
        {
            for (var i = 0; i < PathfindingList.Count; i++)
            {
                if (PathfindingList[i].Target.TargetTransform == null) continue;
                
                _result[i] = new NativeList<JobNode>(50, Allocator.Persistent);

                _jobs[i] = new PathfindingJob
                {
                    Map = _nativeMap,
                    CharPos = PathfindingList[i].transform.position,
                    TargetPos = PathfindingList[i].Target.node.gridPosition,
                    Randomizer = _random,
                    MapSizeX = MapSize.x,
                    MapSizeY = MapSize.y,
                    GridSizeX = GridSizeX,
                    GridSizeY = GridSizeY,
                    OutNodes = _result[i]
                };

                _jobHandles.Add(_jobs[i].Schedule());
            }

            JobHandle.CompleteAll(_jobHandles);

            yield return new WaitForSeconds(0.1f);

            for (var i = 0; i < _jobs.Length; i++)
            {
                if (PathfindingList[i].Target.TargetTransform == null) continue;
                
                PathfindingList[i].waypoints.Clear();
                PathfindingList[i].waypoints = new Queue<Node>(Node.ToNodes(_result[i].ToArray()));

                _result[i].Dispose();
            }
        }
        
        PathfindingList.Clear();
    }

    private void OnTaskFinished(bool wasManual)
    {
        _pathfindingTask = new Task(PathfindIteration(), false);
        _pathfindingTask.Finished += OnTaskFinished;
        _pathfindingTask.Start();
    }

    private void OnDestroy()
    {
        if (_pathfindingTask != null)
        {
            _pathfindingTask.Pause();

            if (_jobHandles.IsCreated) _jobHandles.Dispose();

            _pathfindingTask.Stop();
        }

        ClearLists();

        if (_nativeMap.IsCreated) _nativeMap.Dispose();

        foreach (var nativeList in _result)
        {
            if (nativeList.IsCreated) nativeList.Dispose();
        }
    }
    #endregion
}

//[BurstCompile]
public struct PathfindingJob : IJob
{
    [ReadOnly] public NativeArray<JobNode> Map;
    [ReadOnly] public Unity.Mathematics.Random Randomizer;
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

    private JobNode IsClosestNeighbour(JobNeighbour neighbour, JobNode currentNode, out bool hasChanged)
    {
        hasChanged = false;
        if (neighbour.IsBlocked)
        {
            hasChanged = false;
            return currentNode;
        }
        
        if ((TargetPos - neighbour.GridPosition).sqrMagnitude <
            (TargetPos - currentNode.GridPosition).sqrMagnitude && Randomizer.NextInt(2) == 0)
        {
            hasChanged = true;
            return Map[neighbour.GridPosition.y * MapSizeX + neighbour.GridPosition.x];
        }
        
        return currentNode;
    }

    public void Execute()
    {
        var currentNode = NodeFromWorldPoint(CharPos);

        var i = 0;

        while (currentNode.GridPosition != TargetPos && i < 500)
        {
            currentNode = IsClosestNeighbour(currentNode.Neighbour1, currentNode, out var hasChanged); //todo: check if this fixed the problem
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour2, currentNode, out hasChanged);
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour3, currentNode, out hasChanged);
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour4, currentNode, out hasChanged);
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour5, currentNode, out hasChanged);
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour6, currentNode, out hasChanged);
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour7, currentNode, out hasChanged);
            if (hasChanged) continue;
            currentNode = IsClosestNeighbour(currentNode.Neighbour8, currentNode, out hasChanged);
            if (hasChanged) continue;

            i++;
            OutNodes.Add(currentNode);
        }
    }
}