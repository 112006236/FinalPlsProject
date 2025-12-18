using UnityEngine;
using System.Collections;

public class BringerOfDeath : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Transform sprite; // reference to child sprite object ONLY

    private SpriteRenderer sr;

    [Header("Combat")]
    public float attackRange = 2f;
    public int attackDamage = 30;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float followRange = 15f;
    public float moveSpeed = 1.6f;

    private bool isAttacking = false;
    private bool facingLeft = true;
    private float lastAttack = 0f;
    private BOFHealth health;

    [Header("Healing")]
    public float healAmount = 20f;           // Amount to heal each mini enemy
    public float healRange = 5f;             // Radius around miniboss
    public float healCooldown = 10f;         // Time between heals
    public GameObject healEffectPrefab;      // Visual circle effect

    private float lastHealTime = 0f;


    private void Start()
    {
        health = GetComponent<BOFHealth>();
        sr = GetComponent<SpriteRenderer>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("No GameObject with tag 'Player' found!");
        }
    }

    private void Update()
    {
        if (health == null || player == null) return;
        if (health.enabled == false) return; // dead

        float dist = Vector2.Distance(transform.position, player.position);

        // Periodic healing
        if (Time.time - lastHealTime >= healCooldown)
            StartCoroutine(HealNearbyEnemies());

        if (dist > followRange)
        {
            animator.Play("BringerOfDeath_idle");
            return;
        }

        if (dist > attackRange && !isAttacking)
            FollowPlayer();
        else if (dist <= attackRange)
            TryAttack();

        FlipSprite();
    }

    private IEnumerator HealNearbyEnemies()
    {
        // Find all enemies in range
        Collider[] hits = Physics.OverlapSphere(transform.position, healRange);
        bool hasHealableEnemy = false;

        foreach (Collider hit in hits)
        {
            EnemyStats enemy = hit.GetComponent<EnemyStats>();
            if (enemy != null && enemy.GetCurrentHealth() < enemy.maxHealth)
            {
                hasHealableEnemy = true;
                break;
            }
        }

        if (!hasHealableEnemy)
            yield break; // No one to heal, exit coroutine

        // Proceed with casting
        lastHealTime = Time.time;
        isAttacking = true; // prevent movement/attacks
        animator.Play("BringerOfDeath_cast", 0, 0);

        // Show healing circle effect
        if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = new Vector3(healRange * 2f, 0.1f, healRange * 2f);
            Destroy(effect, 1.5f);
        }

        yield return new WaitForSeconds(1f); // wait for cast animation

        // Heal all enemies in range whose health is not full
        foreach (Collider hit in hits)
        {
            EnemyStats enemy = hit.GetComponent<EnemyStats>();
            if (enemy != null && enemy.GetCurrentHealth() < enemy.maxHealth)
            {
                float newHealth = Mathf.Min(enemy.maxHealth, enemy.GetCurrentHealth() + healAmount);
                enemy.TakeDamage(-healAmount); // negative damage heals
            }
        }

        isAttacking = false;
    }



    // -----------------------------
    private void FollowPlayer()
    {
        animator.Play("BringerOfDeath_walk");
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    // -----------------------------
    private void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time - lastAttack < attackCooldown) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttack = Time.time;

        animator.Play("BringerOfDeath_attack", 0, 0);

        yield return new WaitForSeconds(0.45f);

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerCombat pc = player.GetComponent<PlayerCombat>();
            if (pc != null && !pc.isDead)
                pc.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    // -----------------------------
    private void FlipSprite()
    {
        if (player == null) return;
        bool shouldFaceLeft = player.position.x < transform.position.x;
        sr.flipX = !shouldFaceLeft; 
    }
}
