using UnityEngine;

public class DragonWarrior : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject fireballPrefab;   // For Attack1
    public Transform firePoint;         // Fire spawn position

    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public float followRange = 8f;      // Distance before following starts
    public float attackRange = 3f;      // Distance to start attacking
    public float attackCooldown = 2f;

    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private bool facingRight = true;

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 1️⃣ If player is outside follow range → idle
        if (distance > followRange)
        {
            animator.Play("idle_DragonWarrior");
            return;
        }

        // 2️⃣ If player is within follow range but outside attack range → walk
        if (distance > attackRange && !isAttacking)
        {
            FollowPlayer();
        }
        // 3️⃣ If within attack range → attack
        else if (distance <= attackRange)
        {
            AttackPlayer();
        }

        // 4️⃣ Face player
        FlipTowardsPlayer();
    }

    private void FollowPlayer()
    {
        animator.Play("walk_DragonWarrior");

        Vector3 direction = (player.position - transform.position);
        direction.y = 0f; // ignore vertical difference

        Vector3 movePos = Vector3.MoveTowards(transform.position, transform.position + direction.normalized, moveSpeed * Time.deltaTime);
        movePos.y = transform.position.y; // keep original Y
        transform.position = movePos;
    }



    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        isAttacking = true;

        // Randomly choose between 1 or 2
        int attackType = Random.Range(1, 2);

        if (attackType == 1)
            StartCoroutine(PerformAttack1());
        else
            StartCoroutine(PerformAttack2());
    }

    private System.Collections.IEnumerator PerformAttack1()
    {
        animator.Play("attack_DragonWarrior");
        // Wait until the frame where the fire should spawn (adjust delay)
        yield return new WaitForSeconds(0.45f);

        // instantiate and launch
        GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Fireball fb = go.GetComponent<Fireball>();
        if (fb != null)
        {
            Vector3 dir = player.position; // 3D target
            fb.Launch(dir);
        }
        // optionally wait for rest of animation
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }


    private System.Collections.IEnumerator PerformAttack2()
    {
        animator.Play("strike_DragonWarrior");
        yield return new WaitForSeconds(1.0f); // Duration of slash
        isAttacking = false;
    }

    private void FlipTowardsPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !facingRight)
            Flip();
        else if (player.position.x < transform.position.x && facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
