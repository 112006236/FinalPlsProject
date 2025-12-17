using System.Diagnostics;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed = 60f;
    public GameObject impactEffect;
    public float destroyDelay = 0.5f;
    public GameObject bulletPrefab;

    public bool isScatter = false;

    public int bulletCount = 12;       // number of bullets to spawn
    public float bulletSpeed = 15f;    // how fast bullets fly

    [HideInInspector] public GameObject warningMarker; // assigned by spawner

    private bool hasLanded = false;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.down * 1200f);
    }
    void Update()
    {
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;
        hasLanded = true;

        UnityEngine.Debug.Log("Hit: " + collision.gameObject.name);
        // Spawn explosion effect
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);


        if (bulletPrefab != null)
        {
            if (isScatter)
            SpawnBullets();
        }


        // Destroy marker
        if (warningMarker != null)
            Destroy(warningMarker);

        Destroy(gameObject);
    }


    void SpawnBullets()
    {
        UnityEngine.Debug.Log("Spawn Bullets");
        
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward; // horizontal circle
            Vector3 spawnOffset = dir * 1.2f + Vector3.up * 0.8f;
            GameObject bullet = Instantiate(
                bulletPrefab,
                transform.position + spawnOffset,
                Quaternion.identity
            );
            //GameObject bullet = Instantiate(bulletPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(dir * bulletSpeed, ForceMode.Impulse);
        }
    }

}
