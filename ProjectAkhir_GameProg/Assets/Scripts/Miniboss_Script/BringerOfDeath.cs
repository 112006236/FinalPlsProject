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
    public float attacktolerance = 2.5f;
    public int attackDamage = 30;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float followRange = 15f;
    public float moveSpeed = 1.6f;

    public bool isAttacking = false;
    private bool facingLeft = true;
    private float lastAttack = 0f;
    private BOFHealth health;

    [Header("Healing")]
    public float healAmount = 20f;           // Amount to heal each mini enemy
    public float healRange = 5f;             // Radius around miniboss
    public float healCooldown = 10f;         // Time between heals
    public GameObject healEffectPrefab;      // Visual circle effect

    private float lastHealTime = 0f;
    private bool firsttime = true;
    public bool isHealing = false;          // NEW: prevent multiple coroutines

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

        float dist = Vector3.Distance(transform.position, player.position);

        // Periodic healing
        if (Time.time - lastHealTime >= healCooldown && !isHealing){
            animator.Play("BringerOfDeath_idle");
            StartCoroutine(HealNearbyEnemies());
        }

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
        isHealing = true; // prevent multiple coroutines

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
        {
            isHealing = false;
            yield break; // No one to heal, exit coroutine
        }

        lastHealTime = Time.time;

        if (firsttime)
        {
            firsttime = false;
            isHealing = false;
            yield break;
        }

        isAttacking = true; // prevent movement/attacks

        // Play cast animation
        animator.Play("BringerOfDeath_cast", 0, 0f);

        // Wait for cast animation duration (adjust to match your animation length)
        yield return new WaitForSeconds(1f);

        // Spawn healing circle
        if (healEffectPrefab != null)
        {
            Vector3 pos = transform.position;// slightly above ground
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

            GameObject effect = Instantiate(healEffectPrefab, pos, rotation);

            // Scale the circle based on its sprite size
            SpriteRenderer srFx = effect.GetComponentInChildren<SpriteRenderer>();
            if (srFx != null)
            {
                float spriteDiameter = srFx.bounds.size.x;
                float desiredDiameter = healRange * 2f + 4.5f;
                float scaleFactor = desiredDiameter / spriteDiameter;
                effect.transform.localScale = Vector3.one * scaleFactor;
            }

            // Play healing circle animation
            Animator fxAnim = effect.GetComponent<Animator>();
            if (fxAnim != null)
                fxAnim.Play("healarea", 0, 0f);

            // Wait for animation to finish before destroying
            yield return new WaitForSeconds(0.7f);
            Destroy(effect);
        }

        // Heal all enemies in range
        hits = Physics.OverlapSphere(transform.position, healRange);
        foreach (Collider hit in hits)
        {
            EnemyStats enemy = hit.GetComponent<EnemyStats>();
            if (enemy != null && enemy.GetCurrentHealth() < enemy.maxHealth)
            {
                enemy.TakeDamage(-healAmount);
            }
        }

        isAttacking = false;
        isHealing = false; // reset flag
    }

    private void OnDrawGizmos()
    {
        // Draw actual heal range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);

        // Optional: Draw intended circle sprite size for comparison
        Gizmos.color = Color.cyan;
        float diameter = healRange * 2f;
        Vector3 pos = transform.position;
        pos.y += 0.05f;
        Gizmos.DrawWireSphere(pos, diameter / 2f);
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
        if (isAttacking || isHealing) return;
        if (Time.time - lastAttack < attackCooldown) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttack = Time.time;

        animator.Play("BringerOfDeath_attack", 0, 0f);

        yield return new WaitForSeconds(0.45f);
        if (Vector3.Distance(transform.position, player.position) <= (attackRange + attacktolerance))
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
        if (player == null || sr == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 toPlayer = player.position - transform.position;
        Vector3 camRight = cam.transform.right;
        float dot = Vector3.Dot(toPlayer, camRight);
        sr.flipX = dot > 0f;
    }
}
