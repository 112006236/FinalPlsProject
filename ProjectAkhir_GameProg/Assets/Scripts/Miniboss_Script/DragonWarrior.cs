using UnityEngine;
using System.Collections;

public class DragonWarrior : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public float followRange = 20f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;

    [Header("Kick Attack Settings")]
    public float kickRange = 1.5f;
    public float kickForce = 5f;
    public int kickDamage = 20;
    public float kickCooldown = 0.8f;
    private float lastKickTime = 0f;

    [Header("Kick Effects")]
    public Transform kickWindPoint;
    public GameObject kickWindPrefab;

    [Header("Attack Ranges")]
    public float fireballRange = 10f;
    public float jumpApproachDistance = 2.5f;
    public int maxConsecutiveJumps = 2;
    public float jumpCooldown = 3f;

    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private bool isJumping = false;
    private bool facingRight = true;

    private int jumpCount = 0;
    private float lastJumpTime = -100f;


    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 1️⃣ Idle if out of follow range
        if (distance > followRange)
        {
            animator.Play("idle_DragonWarrior");
            return;
        }

        // 2️⃣ Walk if outside attack range and not attacking
        if (distance > attackRange && !isAttacking && !isJumping)
        {
            FollowPlayer();
        }
        // 3️⃣ Attack if within range
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

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        Vector3 movePos = Vector3.MoveTowards(transform.position, transform.position + direction.normalized, moveSpeed * Time.deltaTime);
        movePos.y = transform.position.y; // keep original Y
        transform.position = movePos;
    }

    private void AttackPlayer()
    {
        if (isAttacking || isJumping) return;

        Vector3 enemyFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerFlat = new Vector3(player.position.x, 0, player.position.z);
        float distance = Vector3.Distance(enemyFlat, playerFlat);

        // 👣 Kick if very close
        if (distance <= kickRange && Time.time - lastKickTime >= kickCooldown)
        {
            lastKickTime = Time.time;
            StartCoroutine(PerformKickAttack());
            return;
        }

        // Determine attack type
        int attackType = Random.Range(1, 3); // 1 = fireball, 2 = jumpAtk

        if (attackType == 1)
        {
            // Fireball attack
            if (distance > fireballRange)
            {
                Vector3 targetPos = player.position - (player.position - transform.position).normalized * fireballRange;
                StartCoroutine(MoveToPosition(targetPos, () => StartCoroutine(PerformAttack1())));
            }
            else
            {
                StartCoroutine(PerformAttack1());
            }
        }
        else if (attackType == 2)
        {
            // Jump attack if allowed
            if (distance > kickRange && (jumpCount < maxConsecutiveJumps || Time.time - lastJumpTime >= jumpCooldown))
            {
                StartCoroutine(JumpTowardsKickDistance());
            }
            else
            {
                StartCoroutine(PerformAttack1());
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, System.Action onReached)
    {
        isAttacking = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 movePos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            movePos.y = transform.position.y;
            transform.position = movePos;
            FlipTowardsPlayer();
            yield return null;
        }

        onReached?.Invoke();
        isAttacking = false;
    }

    private IEnumerator JumpTowardsKickDistance()
    {
        isAttacking = true;
        isJumping = true;
        jumpCount++;
        lastJumpTime = Time.time;

        animator.Play("jumpAtk_DragonWarrior", -1, 0f);
        yield return new WaitForSeconds(0.15f); // shorter delay before moving

        Vector3 jumpTarget = player.position - (player.position - transform.position).normalized * kickRange;
        Vector3 startPos = transform.position;

        // smaller jump arc (height = 0.4f instead of 1f)
        float jumpDuration = 0.35f; 
        float jumpHeight = 0.4f;

        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            transform.position = Vector3.Lerp(startPos, jumpTarget, t) + new Vector3(0, Mathf.Sin(t * Mathf.PI) * jumpHeight, 0);
            FlipTowardsPlayer();
            yield return null;
        }

        // ensure it ends flat on the ground
        Vector3 flatPos = transform.position;
        flatPos.y = startPos.y;
        transform.position = flatPos;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= kickRange)
            yield return StartCoroutine(PerformKickAttack());

        isJumping = false;
        isAttacking = false;
    }


    private IEnumerator PerformAttack1()
    {
        isAttacking = true;
        animator.Play("attack_DragonWarrior", -1, 0f);
        animator.Update(0f);

        yield return new WaitForSeconds(0.45f);

        if (fireballPrefab != null && firePoint != null)
        {
            GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Fireball fb = go.GetComponent<Fireball>();
            if (fb != null)
            {
                fb.Launch(player.position);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    private IEnumerator PerformKickAttack()
    {
        isAttacking = true;
        animator.Play("flyKick_DragonWarrior", -1, 0f);
        yield return new WaitForSeconds(0.2f);

        if (kickWindPrefab != null && kickWindPoint != null)
        {
            GameObject wind = Instantiate(kickWindPrefab, kickWindPoint.position, Quaternion.identity);
            if (!facingRight)
                wind.transform.localScale = new Vector3(-1, 1, 1);
        }

        yield return new WaitForSeconds(0.4f);

        // 💥 Check for hit and deal damage
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= kickRange)
        {
            // Push effect (optional since player uses CharacterController)
            Vector3 pushDir = (player.position - transform.position).normalized;
            player.position += pushDir * 0.5f; // small nudge effect

            // ✅ Deal damage
            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            if (playerCombat != null && !playerCombat.isDead)
            {
                playerCombat.TakeDamage(kickDamage);
            }
        }

        yield return new WaitForSeconds(0.5f);
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
