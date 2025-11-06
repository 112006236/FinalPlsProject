using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Fireball : MonoBehaviour
{
    public float speed = 6f;
    public float maxLifetime = 6f;
    public float height = 1.0f;

    public int damage = 1;

    Rigidbody rb;
    Animator animator;
    Collider col;
    bool hasExploded = false;

    public Camera mainCamera;

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
    }

    public void Launch(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.magnitude < 0.01f) return;
        direction.Normalize();

        rb.isKinematic = false;
        rb.velocity = direction * speed;

        // rotate fireball to face shooting direction
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        animator.Play("Fireball_Fly");
    }

    void LateUpdate()
    {
        // keep Y fixed
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        // make fireball face camera (2D-style)
        if (mainCamera != null)
        {
            Vector3 toCamera = mainCamera.transform.position - transform.position;
            toCamera.y = 0f; // lock upright
            if (toCamera.sqrMagnitude > 0.001f)
            {
                Quaternion camRotation = Quaternion.LookRotation(-toCamera);
                // only apply rotation around Y axis so shooting direction stays
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, camRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        // Only explode if it hits the player
        if (other.CompareTag("Player"))
        {
            // You can also deal damage here, e.g.,
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        // Optionally, still explode on walls/obstacles
        else if (!other.isTrigger)
        {
            Explode();
        }
    }


    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        if (col) col.enabled = false;

        animator.SetTrigger("explode");
    }

    public void OnExplodeAnimationEnd()
    {
        Destroy(gameObject);
    }
}
