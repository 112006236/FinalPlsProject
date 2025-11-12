using UnityEngine;
using System.Collections;

public class MushroomController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MushroomHealth mushroomHealth;
    private Coroutine currentAttackCoroutine;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public float followRange = 5f;
    public float attackRange = 2f;

    private bool isAttacking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mushroomHealth = GetComponent<MushroomHealth>();
    }

    private void Update()
    {
        if (!mushroomHealth || mushroomHealth.isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- Attack Logic ---
        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking)
            {
                // Start attack only if not attacking
                animator.ResetTrigger("isAttacking");
                currentAttackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // Stop attack if player leaves range
            if (isAttacking && currentAttackCoroutine != null)
            {
                StopCoroutine(currentAttackCoroutine);
                isAttacking = false;
                currentAttackCoroutine = null;

                // Reset attack animation properly
                animator.ResetTrigger("isAttacking");
                animator.SetBool("isWalking", true); // allow follow animation to play
            }

            // --- Follow if inside followRange ---
            if (distanceToPlayer <= followRange)
            {
                animator.SetBool("isWalking", true);
                FollowPlayer(distanceToPlayer);
            }
            else
            {
                // Idle if out of follow range
                animator.SetBool("isWalking", false);
            }
        }

        // Flip sprite
        spriteRenderer.flipX = (player.position.x > transform.position.x);
    }

    private void FollowPlayer(float distanceToPlayer)
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");

        // Wait for attack animation duration
        yield return new WaitForSeconds(1f);

        isAttacking = false;
        currentAttackCoroutine = null;
    }
}
