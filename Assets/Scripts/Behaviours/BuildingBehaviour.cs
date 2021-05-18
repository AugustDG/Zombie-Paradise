using System;
using System.Collections;
using UnityEngine;
using Utility;

public class BuildingBehaviour : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float healthBit;
    [SerializeField] private int turnOffLightsPos;
    [SerializeField] private bool isDealingDamage;
    [SerializeField] private float attackInterval;
    private Coroutine DamageDealerCoroutine;

    [Space(5f)]
    [Header("References")]
    [SerializeField] private HealthLight[] healthLights = new HealthLight[10];

    private void Start()
    {
        healthBit = MapData.CurrentTreeHealth / 10;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Human") && !isDealingDamage)
        {
            isDealingDamage = true;
            DamageDealerCoroutine = StartCoroutine(DealDamage(other.GetComponent<CharacterBehaviour>()));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (DamageDealerCoroutine != null)
        {
            isDealingDamage = false;
            StopCoroutine(DamageDealerCoroutine);
        }
    }

    private IEnumerator DealDamage(CharacterBehaviour attackChar)
    {
        attackChar.SufferDamage(MapData.CurrentTreeAttack);

        yield return new WaitForSeconds(attackInterval);
        if (isDealingDamage) StartCoroutine(DealDamage(attackChar));
    }

    public void SufferDamage(float damage)
    {
        MapData.CurrentTreeHealth -= damage;

        //bigger than base health, no lights turn off
        if (MapData.CurrentTreeHealth >= 500) return;
        
        print(MapData.CurrentTreeHealth);

        if (MapData.CurrentTreeHealth < 0)
        {
            MapData.GameManagerRef.GameFinished(GameEndType.LossByTree);
        }
        else
        {
            turnOffLightsPos = 9 - (int)MapData.CurrentTreeHealth / (int)healthBit;

            for (var i = 0; i < turnOffLightsPos; i++) healthLights[turnOffLightsPos].FadeOut();
        }
    }
}