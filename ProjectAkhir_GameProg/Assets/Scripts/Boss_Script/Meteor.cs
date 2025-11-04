using System.Diagnostics;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed = 60f;
    public GameObject impactEffect;
    public float destroyDelay = 0.5f;

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
        hasLanded = true;

        UnityEngine.Debug.Log("Hit: " + collision.gameObject.name);
        // Spawn explosion effect
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        // Destroy marker
        if (warningMarker != null)
            Destroy(warningMarker);

        Destroy(gameObject);
    }
}
