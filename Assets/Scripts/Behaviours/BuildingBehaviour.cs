using UnityEngine;
using Utility;

public class BuildingBehaviour : MonoBehaviour
{
    [Header("Properties")]
    private float _healthBit;
    private int _turnOffLights;

    [Space(5f)]
    [Header("References")]
    [SerializeField] private HealthLight[] healthLights = new HealthLight[10];

    private void Awake()
    {
        _healthBit = MapData.CurrentTreeHealth / 10f;
    }

    public void SufferDamage(float damage)
    {
        MapData.CurrentTreeHealth -= damage;

        _turnOffLights = 10 - (int)MapData.CurrentTreeHealth / (int)_healthBit;

        for (var i = 0; i < _turnOffLights; i++) healthLights[_turnOffLights].FadeOut();

        if (MapData.CurrentTreeHealth < 0)
        {
            MapData.GameManagerRef.GameFinished();
        }
    }
}