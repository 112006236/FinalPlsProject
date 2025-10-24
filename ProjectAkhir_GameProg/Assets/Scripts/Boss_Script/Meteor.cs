using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed = 40f;
    public GameObject impactEffect;  // Optional explosion prefab
    public float destroyDelay = 0.5f;  // Destroy delay after impact

    private bool hasLanded = false;

    void Update()
    {
        if (!hasLanded)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;
        hasLanded = true;

        // Spawn impact effect
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        // Optional: Damage logic here

        Destroy(gameObject, destroyDelay);
    }
}
