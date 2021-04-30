using System.Collections;
using Characters;
using UnityEngine;

public class ShootingBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject shot;
    [SerializeField] private AudioClip gunShot;
    
    private HumanBehaviour _humanBehaviour;
    private AudioSource _audioSource;

    private void Awake()
    {
        _humanBehaviour = GetComponentInParent<HumanBehaviour>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie") && !_humanBehaviour.isShooting)
        {
            _humanBehaviour.isShooting = true;
            _humanBehaviour.shootTrans = other.transform;
            StartCoroutine(Shoot());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie") || _humanBehaviour.shootTrans == null) _humanBehaviour.isShooting = false;
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(_humanBehaviour.shotDelay);
        
        _audioSource.PlayOneShot(gunShot);
        
        Instantiate(shot, transform.position + Vector3.up, transform.rotation);

        if (_humanBehaviour.isShooting) StartCoroutine(Shoot());
    }
}
