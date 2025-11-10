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
    public float followRange = 20f;      // Distance before following starts
    public float attackRange = 5f;      // Distance to start attacking
    public float attackCooldown = 2f;

    [Header("Kick Attack Settings")]
    public float kickRange = 1.5f;    // Distance for kick
    public float kickForce = 5f;      // Optional push force on player
    public int kickDamage = 20;       // Damage dealt by kick
    public float kickCooldown = 0.8f; // Small cooldown between kicks
    private float lastKickTime = 0f;  // Track last kick

    [Header("Kick Effects")]
    public Transform kickWindPoint;   // Drag the KickWindPoint here
    public GameObject kickWindPrefab; // Drag the animated wind prefab here


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
        Vector3 enemyFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerFlat = new Vector3(player.position.x, 0, player.position.z);
        float distance = Vector3.Distance(enemyFlat, playerFlat);


        // 👣 If player is very close → do kick attack
        if (distance <= kickRange)
        {
            Debug.Log("inside kick range");
            if (Time.time - lastKickTime >= kickCooldown)
            {
                Debug.Log("kick");
                lastKickTime = Time.time;
                StartCoroutine(PerformKickAttack());
            }
            return; // Skip other attacks while kicking
        }

        // 🔥 Otherwise randomly do attack1 or 2
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        isAttacking = true;

        int attackType = Random.Range(1, 2); // 1 or 2
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
        animator.Play("strike_DragonWarrior"); // or some other animation
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }


    private System.Collections.IEnumerator PerformKickAttack()
    {
        // Force restart animation
        animator.Play("flyKick_DragonWarrior", -1, 0f);

        yield return new WaitForSeconds(0.2f);

        // Spawn wind effect at feet
        if (kickWindPrefab != null && kickWindPoint != null)
        {
            GameObject wind = Instantiate(kickWindPrefab, kickWindPoint.position, Quaternion.identity);

            // Flip wind if dragon faces left
            if (!facingRight)
                wind.transform.localScale = new Vector3(-1, 1, 1);

            // Destroy automatically after animation
            // Make sure your prefab has AutoDestroyAfterAnim script
        }

        // Wait until the kick connects
        yield return new WaitForSeconds(0.4f);

        // Apply damage / push to player
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= kickRange)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 pushDir = (player.position - transform.position).normalized;
                rb.AddForce(pushDir * kickForce, ForceMode2D.Impulse);
            }
        }

        // Wait for rest of animation
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
