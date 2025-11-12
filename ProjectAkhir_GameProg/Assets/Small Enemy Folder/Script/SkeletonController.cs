using UnityEngine;
using System.Collections;

public class SkeletonController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SkeletonHealth skeletonHealth;
    private Coroutine currentAttackCoroutine;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public float followRange = 5f;
    public float attackRange = 2f;

    [Header("Combat")]
    public float attackDamage = 10f;
    [Tooltip("Time in seconds from attack start to when damage is applied")]
    public float damageTime = 1.1f; 

    private bool isAttacking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        skeletonHealth = GetComponent<SkeletonHealth>();
    }

    private void Update()
    {
        if (!skeletonHealth || skeletonHealth.isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking)
            {
                animator.ResetTrigger("isAttacking");
                currentAttackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            if (isAttacking && currentAttackCoroutine != null)
            {
                StopCoroutine(currentAttackCoroutine);
                isAttacking = false;
                currentAttackCoroutine = null;

                animator.ResetTrigger("isAttacking");
                animator.SetBool("isWalking", true); 
            }

            if (distanceToPlayer <= followRange)
            {
                animator.SetBool("isWalking", true);
                FollowPlayer(distanceToPlayer);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        spriteRenderer.flipX = (player.position.x < transform.position.x);
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

        yield return new WaitForSeconds(damageTime);
        DealDamageToPlayer(); 

        float remainingTime = animator.GetCurrentAnimatorStateInfo(0).length - damageTime;
        if (remainingTime > 0f)
            yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
        currentAttackCoroutine = null;
    }

    private void DealDamageToPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            if (playerCombat != null && !playerCombat.isDead)
            {
                playerCombat.TakeDamage(attackDamage);
                Debug.Log("Skeleton dealt " + attackDamage + " damage!");
            }
        }
    }
}
