using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using Utilities.Extensions;

public class GoForwardAndDestroy : MonoBehaviour
{
    public float speed = 1f;
    public float lifetime = 5f;
    public float damage = 5f;

    private void Start()
    {
        StartCoroutine(DelayDeath());
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie")) other.GetComponent<ZombieBehaviour>().SufferDamage(damage);
    }

    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(lifetime);
        
        gameObject.Destroy();
    }
}