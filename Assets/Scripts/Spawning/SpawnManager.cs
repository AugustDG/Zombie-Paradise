using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Extensions;
using Utility;
public class SpawnManager : MonoBehaviour
{
    public Vector3 startWorldPos, endWorldPos;
    
    [SerializeField] private GameObject humanPrefab;
    [SerializeField] private GameObject zombieTemplate;
    [SerializeField] private SpawnPreview zombiePreview;

    private Dictionary<Vector2Int, SpawnPreview> _spawnPreviews = new Dictionary<Vector2Int, SpawnPreview>();
    private ZombieBehaviour _zombieToSpawn;
    private Camera _camera;
    private LayerMask _rayLayerMask;
    private bool _canSpawn = true;
    private Ray _startRay;

    //todo:remove this
    IEnumerator SpawnHumans()
    {
        MapData.HumanList.Add(Instantiate(humanPrefab, Random.Range(-100, 100) * Vector3.right + Random.Range(-100, 100) * Vector3.forward, Quaternion.identity).GetComponent<HumanBehaviour>());

        yield return new WaitForSecondsRealtime(5f);

        StartCoroutine(SpawnHumans());
    }
    
    private void Awake()
    {
        _camera = Camera.main;
        _rayLayerMask = ~LayerMask.GetMask("Zombies", "Obstacles");
    }

    private void Start()
    {
        //todo: fix this as it is a temporary solution
        var previewBehaviour = Instantiate(zombieTemplate, new Vector3(0f, -150f, 0f), Quaternion.identity).GetComponent<ZombieBehaviour>();

        previewBehaviour.attack = MapData.ZombieToSpawn.totalAttack;
        previewBehaviour.health = MapData.ZombieToSpawn.totalHealth;
        previewBehaviour.speed = MapData.ZombieToSpawn.totalSpeed;

        var partsRotation = previewBehaviour.torsoPosition.rotation;

        Instantiate(MapData.ZombieToSpawn.head.partObject, previewBehaviour.headPosition.position, partsRotation, previewBehaviour.headPosition);
        Instantiate(MapData.ZombieToSpawn.torso.partObject, previewBehaviour.torsoPosition.position, partsRotation, previewBehaviour.torsoPosition);
        Instantiate(MapData.ZombieToSpawn.arms[0].partObject, previewBehaviour.armLPosition.position, partsRotation, previewBehaviour.armLPosition);
        Instantiate(MapData.ZombieToSpawn.arms[1].partObject, previewBehaviour.armRPosition.position, partsRotation, previewBehaviour.armRPosition);
        Instantiate(MapData.ZombieToSpawn.legs[0].partObject, previewBehaviour.legLPosition.position, partsRotation, previewBehaviour.legLPosition);
        Instantiate(MapData.ZombieToSpawn.legs[1].partObject, previewBehaviour.legRPosition.position, partsRotation, previewBehaviour.legRPosition);

        _zombieToSpawn = previewBehaviour;
        
        //StartCoroutine(SpawnHumans());
    }

    // Not showing bounding box, because it's useless if the units preview already shows up
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var startMousePos = Mouse.current.position.ReadValue();
            _startRay = _camera.ScreenPointToRay(startMousePos);

            if (Physics.Raycast(_startRay, out var hitInfo, 500f, _rayLayerMask))
            {
                startWorldPos = hitInfo.point;

                _canSpawn = true;
            }
        }

        if (Mouse.current.leftButton.isPressed && _canSpawn)
        {
            var endMousePos = Mouse.current.position.ReadValue();
            var endRay = _camera.ScreenPointToRay(endMousePos);

            if (Physics.Raycast(endRay, out var hitInfo, 500f, _rayLayerMask))
            {
                endWorldPos = hitInfo.point;

                var startGridPos = MapManager.NodeFromWorldPoint(startWorldPos).gridPosition;
                var endGridPos = MapManager.NodeFromWorldPoint(endWorldPos).gridPosition;

                if (startGridPos.x > endGridPos.x)
                {
                    var temp = startGridPos.x;
                    startGridPos.x = endGridPos.x;
                    endGridPos.x = temp;
                }

                if (startGridPos.y > endGridPos.y)
                {
                    var temp = startGridPos.y;
                    startGridPos.y = endGridPos.y;
                    endGridPos.y = temp;
                }

                foreach (var keyPair in _spawnPreviews)
                {
                    _spawnPreviews[keyPair.Key].hasJustSpawned = false;
                }

                for (var x = startGridPos.x; x <= endGridPos.x; x++)
                {
                    for (var y = startGridPos.y; y <= endGridPos.y; y++)
                    {
                        if (MapData.Map[x, y].nodeType == NodeTypes.Blocked) continue;
                        if (_spawnPreviews.ContainsKey(MapData.Map[x, y].gridPosition))
                        {
                            _spawnPreviews[MapData.Map[x, y].gridPosition].hasJustSpawned = true;
                            continue;
                        }

                        _spawnPreviews.Add(MapData.Map[x, y].gridPosition, Instantiate(zombiePreview,
                            MapData.Map[x, y].worldPosition,
                            Quaternion.identity));
                        ;
                    }
                }

                foreach (var keyPair in _spawnPreviews.ToArray())
                {
                    //if it hasn't just spawned, it's not part of the preview and can be destroyed
                    if (!keyPair.Value.hasJustSpawned)
                    {
                        _spawnPreviews[keyPair.Key].gameObject.Destroy();
                        _spawnPreviews.Remove(keyPair.Key);
                    }
                }
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _canSpawn = false;

                foreach (var keyPair in _spawnPreviews.ToArray()) _spawnPreviews[keyPair.Key].gameObject.Destroy();

                _spawnPreviews.Clear();
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _canSpawn)
        {
            var zombieBehaviours = new ZombieBehaviour[_spawnPreviews.Count];
            var i = 0;

            foreach (var preview in _spawnPreviews.ToArray())
            {
                zombieBehaviours[i] = Instantiate(_zombieToSpawn, MapData.Map[preview.Key.x, preview.Key.y].worldPosition, Quaternion.identity);
                _spawnPreviews[preview.Key].gameObject.Destroy();
                i++;
            }
            
            MapData.ZombieList.AddRange(zombieBehaviours);
            _spawnPreviews.Clear();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_startRay.origin, _startRay.direction * 500f);
    }
}