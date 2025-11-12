using UnityEngine;

public class GolemFollow : MonoBehaviour
{
    public Transform player;          // Assign Player in Inspector
    public float speed = 2f;          // Movement speed
    public float stopDistance = 5f;   // Distance to stop following

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        // Calculate horizontal distance (x and z only, ignore y)
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 flatDistanceVector = targetPos - transform.position;
        float distance = new Vector2(flatDistanceVector.x, flatDistanceVector.z).magnitude;

        if (distance > stopDistance)
        {
            // Move towards player on x and z
            Vector3 direction = flatDistanceVector.normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Walking animation
            animator.SetBool("isWalking", true);
        }
        else
        {
            // Stop walking
            animator.SetBool("isWalking", false);
        }

        // Flip sprite based on x-axis
        spriteRenderer.flipX = (player.position.x < transform.position.x);
    }
}
