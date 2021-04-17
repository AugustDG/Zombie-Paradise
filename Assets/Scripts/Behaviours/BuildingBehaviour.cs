using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    [Header("Properties")]
    public float health;
    private float _healthBit;
    private int _turnOffLights;

    [Space(5f)]
    [Header("References")]
    [SerializeField] private HealthLight[] healthLights = new HealthLight[10];

    private void Awake()
    {
        _healthBit = health / 10f;
    }

    public void SufferDamage(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            _turnOffLights = 10 - (int)health / (int)_healthBit;

            for (var i = 0; i < _turnOffLights; i++) healthLights[_turnOffLights].FadeOut();   
        }
    }
}