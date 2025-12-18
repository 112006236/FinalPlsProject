using UnityEngine;

public class MeteorBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float damage = 5.0f;

    public GameObject impactEffect;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    void Start()
    {
        Invoke(nameof(EnableCollision), 0.05f);
        Destroy(gameObject, lifeTime);
    }

    void EnableCollision()
    {
        col.enabled = true;
    }

    public void Launch(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Optional: small explosion or damage
        UnityEngine.Debug.Log("Bullet Hit: " + collision.gameObject.name);
        //if (collision.gameObject.CompareTag("Boss")) return;
        PlayerCombat pc = collision.gameObject.GetComponent<PlayerCombat>();
        if (pc != null)
        {
            Debug.Log("player take damage from bullet");
            pc.TakeDamage(damage);
        }

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (collision.gameObject.CompareTag("Boss"))
        {
            return;
        } 
        if (collision.gameObject.name == "MeteorBullet") return;
        Destroy(gameObject);
    }
}
