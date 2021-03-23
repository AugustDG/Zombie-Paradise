using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Extensions;
using Utility;

public class SpawnManager : MonoBehaviour
{
    public Vector3 startWorldPos, endWorldPos;

    [SerializeField] private GameObject zombieTemplate;
    [SerializeField] private SpawnPreview zombiePreview;
    [SerializeField] private TMP_Text costText;

    private List<SpawnPreview> _spawnPreviews = new List<SpawnPreview>();
    private ZombieBehaviour _zombieToSpawn;
    private Camera _camera;
    private LayerMask _rayLayerMask;
    private bool _canSpawn = true;
    private Ray _startRay;
    private float _currentCost;

    private void Awake()
    {
        _camera = Camera.main;
        _rayLayerMask = ~LayerMask.GetMask("Zombies", "Obstacles");
    }

    private void Start()
    {
        //todo: fix this as it is a temporary solution
        var previewBehaviour = Instantiate(zombieTemplate, new Vector3(0f, -150f, 0f), Quaternion.identity).GetComponent<ZombieBehaviour>();

        MapData.ZombieToSpawn.CalculateTotalModifiers();

        previewBehaviour.attack = MapData.ZombieToSpawn.totalAttack;
        previewBehaviour.health = MapData.ZombieToSpawn.totalHealth;
        previewBehaviour.speed = MapData.ZombieToSpawn.totalSpeed;
        previewBehaviour.spawnCost = MapData.ZombieToSpawn.totalCost;

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

                _spawnPreviews.ForEach(preview => preview.hasJustSpawned = false);

                _currentCost = 0;

                for (var x = startGridPos.x; x <= endGridPos.x; x++)
                {
                    for (var y = startGridPos.y; y <= endGridPos.y; y++)
                    {
                        if (MapData.Map[x, y].nodeType == NodeTypes.Blocked) continue;
                        if (_currentCost.RoundToInt() > MapData.FingerAmount || MapData.FingerAmount == 0) break;

                        var evalPreview = _spawnPreviews.FirstOrDefault(preview => preview.gridPosition == MapData.Map[x, y].gridPosition);

                        if (evalPreview != null)
                        {
                            evalPreview.hasJustSpawned = true;
                            _currentCost += _zombieToSpawn.spawnCost;
                            continue;
                        }

                        var objToInstantiate = Instantiate(zombiePreview,
                            MapData.Map[x, y].worldPosition,
                            Quaternion.identity);

                        objToInstantiate.gridPosition = MapData.Map[x, y].gridPosition;

                        _spawnPreviews.Add(objToInstantiate);
                        _currentCost += _zombieToSpawn.spawnCost;
                    }
                }

                foreach (var preview in _spawnPreviews.ToArray())
                {
                    //if it hasn't spawned this fixed frame, it's not part of the preview and can be destroyed
                    if (!preview.hasJustSpawned)
                    {
                        preview.gameObject.Destroy();
                        _spawnPreviews.Remove(preview);
                    }
                }

                costText.gameObject.SetActive(true);
                costText.GetComponent<RectTransform>().anchoredPosition = endMousePos;
                costText.text = _currentCost.RoundToInt().ToString();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _canSpawn = false;

                foreach (var preview in _spawnPreviews) preview.gameObject.Destroy();

                _spawnPreviews.Clear();

                costText.gameObject.SetActive(false);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _canSpawn)
        {
            _spawnPreviews.RemoveAll(preview => preview == null);
            var zombieBehaviours = new List<ZombieBehaviour>();

            foreach (var preview in _spawnPreviews.ToArray())
            {
                zombieBehaviours.Add(Instantiate(_zombieToSpawn, MapData.Map[preview.gridPosition.x, preview.gridPosition.y].worldPosition, Quaternion.identity));
                preview.gameObject.Destroy();
            }

            MapData.ZombieList.AddRange(zombieBehaviours);
            _spawnPreviews.Clear();

            MapData.FingerAmount -= _currentCost.RoundToInt();

            costText.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_startRay.origin, _startRay.direction * 500f);
    }
}