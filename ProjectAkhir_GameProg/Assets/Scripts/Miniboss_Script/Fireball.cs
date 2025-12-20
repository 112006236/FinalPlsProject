using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float maxLifetime = 6f;
    public float height = 1.0f;
    public int damage = 15;

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

        transform.forward = shootDir;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        if (sprite == null || mainCamera == null)
            return;

        sprite.forward = mainCamera.transform.forward;

        Vector3 horizontalVel = rb.velocity;
        horizontalVel.y = 0f;

        if (horizontalVel.sqrMagnitude > 0.0001f)
        {
            float yAngle = Mathf.Atan2(horizontalVel.x, horizontalVel.z) * Mathf.Rad2Deg;
            sprite.localRotation = Quaternion.Euler(0f, yAngle + 270f, 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;
        if (other.CompareTag("Enemy")) return;
        if (other.CompareTag("Sword")) return;
        if (other.CompareTag("Shrine")) return;
        if (other.CompareTag("Princess")) return;

        Debug.Log("Fireball hit: " + other.name);

        if (other.CompareTag("Player"))
        {
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
            if (playerCombat != null && !playerCombat.isDead)
            {
               playerCombat.TakeDamage(damage);
            }
        }

        // Spawn explosion effect
        Explode(other);
    }

    public void Explode(Collider hitObject)
    {
        if (hasExploded) return;
        hasExploded = true;

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        if (col) col.enabled = false;

        if (explosionEffectPrefab != null)
        {
            Vector3 spawnPos;
            Collider hitCol = hitObject.GetComponent<Collider>();
            if (hitCol != null)
                spawnPos = hitCol.bounds.center;
            else
                spawnPos = hitObject.transform.position;

            GameObject effect = Instantiate(explosionEffectPrefab, spawnPos, Quaternion.identity);
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

        animator.SetTrigger("explode");
        Destroy(gameObject);
    }

    public void OnExplodeAnimationEnd()
    {
        Destroy(gameObject);
    }
}
