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
    public Transform sprite;      // Assign child object containing the fireball sprite
    public Camera mainCamera;

    Rigidbody rb;
    Animator animator;
    Collider col;
    bool hasExploded = false;

    private Vector3 lastDirection = Vector3.zero;
    public float directionThreshold = 0.01f; // minimum change to update rotation

    private Vector3 target;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        // Set initial height
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
        target=targetPosition;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        direction.Normalize();
        rb.isKinematic = false;
        rb.velocity = direction * speed;

        // Rotate the WHOLE fireball (not just the sprite) toward flight direction
        transform.forward = direction;
    }


    void LateUpdate()
    {
        // Keep fireball at fixed height
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;

        if (mainCamera != null && sprite != null)
        {
            // --- Face the camera horizontally (Y billboard) ---
            Vector3 toCam = mainCamera.transform.position - sprite.position;
            toCam.y = 0f;
            Quaternion faceCamY = Quaternion.LookRotation(-toCam, Vector3.up);
            sprite.rotation = faceCamY;

            // --- Adjust Z rotation based on shooting direction ---
            // Project fireball forward vector onto the camera's plane
            Vector3 ballPos = transform.position;    // enemy's position
            Vector3 dirBallToPlayer = (target - ballPos).normalized;

            Vector3 fireDir = target - dirBallToPlayer*1000f;
            Debug.Log(fireDir);
            Vector3 camRight = mainCamera.transform.right;
            Vector3 camUp = mainCamera.transform.up;

            // Compute angle between fireball's direction and camera's right vector
            float zAngle = Mathf.Atan2(
                Vector3.Dot(fireDir, camUp),
                Vector3.Dot(fireDir, camRight)
            ) * Mathf.Rad2Deg;

            // Apply the rotation around local Z (so head points toward direction of shot)
            sprite.Rotate(0f, 0f, -zAngle);
        }
    }




    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        if (other.CompareTag("Player"))
        {
            // Deal damage here
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
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
