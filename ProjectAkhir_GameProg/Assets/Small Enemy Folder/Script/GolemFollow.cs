using UnityEngine;

public class GolemFollow : MonoBehaviour
{
    public Transform player;          
    public float speed = 2f;          
    public float stopDistance = 5f;   

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

        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 flatDistanceVector = targetPos - transform.position;
        float distance = new Vector2(flatDistanceVector.x, flatDistanceVector.z).magnitude;

        if (distance > stopDistance)
        {
            Vector3 direction = flatDistanceVector.normalized;
            transform.position += direction * speed * Time.deltaTime;

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        spriteRenderer.flipX = (player.position.x < transform.position.x);
    }
}
