using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float maxLifetime = 6f;
    public float height = 1.0f;
    public int damage = 1;

    [Header("Visual")]
    public Transform sprite;      // Child object containing the fireball sprite
    public Camera mainCamera;

    private Rigidbody rb;
    private Animator animator;
    private Collider col;
    private bool hasExploded = false;
    private Vector3 target;
    private Vector3 shootDir;

    [Header("Effects")]
    public GameObject explosionEffectPrefab; 


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        // Keep consistent height
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        Destroy(gameObject, maxLifetime);

        if (mainCamera == null)
            mainCamera = Camera.main;

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Launch(Vector3 targetPosition)
    {
        target = targetPosition;
        shootDir = targetPosition - transform.position;
        shootDir.y = 0f;

        if (shootDir.sqrMagnitude < 0.0001f) return;

        shootDir.Normalize();
        rb.isKinematic = false;
        rb.velocity = shootDir * speed;

        // Orient the fireball (body) toward the target
        transform.forward = shootDir;
    }

    void LateUpdate()
    {
        // Keep fireball flying at fixed height
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        if (sprite == null || mainCamera == null)
            return;

        // --- 1️⃣ Always face the camera (billboard) ---
        sprite.forward = mainCamera.transform.forward;

        // --- 2️⃣ Rotate around Y-axis to face shooting direction ---
        Vector3 horizontalVel = rb.velocity;
        horizontalVel.y = 0f;

        if (horizontalVel.sqrMagnitude > 0.0001f)
        {
            // Calculate Y rotation angle based on direction
            float yAngle = Mathf.Atan2(horizontalVel.x, horizontalVel.z) * Mathf.Rad2Deg;
            sprite.localRotation = Quaternion.Euler(0f, yAngle+270, 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;
        if (other.CompareTag("Enemy")) return;
        if (other.CompareTag("Sword")) return;

        Debug.Log("Fireball hit: " + other.name);

        // Spawn explosion on the object hit
        Explode(other);

        if (other.CompareTag("Player"))
        {
            // TODO: damage logic here
        }
    }

    public void Explode(Collider hitObject)
    {
        if (hasExploded) return;
        hasExploded = true;

        // Stop movement
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        if (col) col.enabled = false;

        if (explosionEffectPrefab != null)
        {
            Vector3 spawnPos;

            // If the object has a collider, spawn in its center
            Collider hitCol = hitObject.GetComponent<Collider>();
            if (hitCol != null)
            {
                spawnPos = hitCol.bounds.center;
            }
            else
            {
                // fallback: use object's transform position
                spawnPos = hitObject.transform.position;
            }

            GameObject effect = Instantiate(explosionEffectPrefab, spawnPos, Quaternion.identity);

            // Parent it to the hit object so it follows
            effect.transform.SetParent(hitObject.transform);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effect, 0.5f);
            }
        }

        // Destroy fireball immediately
        Destroy(gameObject);

        animator.SetTrigger("explode"); // optional
    }



    public void OnExplodeAnimationEnd()
    {
        Destroy(gameObject);
    }
}
