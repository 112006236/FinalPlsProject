using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BringerOfDeath : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Transform sprite; // reference to child sprite object ONLY

    private SpriteRenderer sr;
    private NavMeshAgent agent; // NEW: NavMeshAgent for movement

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

    [Header("Spawn / Entry")]
    [SerializeField] private GameObject spawnCircleEffect;
    [SerializeField] private float spawnCircleRadius = 5f;
    [SerializeField] private float entryRiseHeight = 1f;
    [SerializeField] private float entryDuration = 1.2f;
    [SerializeField] private float spawnVFXDuration = 1.2f;

    private bool hasEntered = false;

    // -----------------------------
    private enum BossState { Idle, Walking, Attacking, Healing }
    private BossState currentState = BossState.Idle;

    // -----------------------------
    private void Start()
    {
        animator.Rebind();
        animator.Update(0f);
        health = GetComponent<BOFHealth>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;

        // Init NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange;
        agent.updateRotation = false; // We'll handle rotation manually
        agent.updateUpAxis = false;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        StartCoroutine(EntrySequence());
    }

    private IEnumerator EntrySequence()
    {
        sr.enabled = true;
        float groundY = transform.position.y;

        Vector3 groundPos = new Vector3(transform.position.x, groundY, transform.position.z);
        Vector3 finalPos = new Vector3(transform.position.x, 1.140f, transform.position.z);

        // Start underground
        transform.position = finalPos - Vector3.up * entryRiseHeight;

        // Lock animation & state
        SetState(BossState.Idle);
        isAttacking = true;
        isHealing = true;

        // --- Spawn circle FIRST ---
        GameObject circle = null;
        if (spawnCircleEffect != null)
        {
            Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
            circle = Instantiate(spawnCircleEffect, groundPos, rot);

            // Scale circle correctly
            SpriteRenderer srFx = circle.GetComponentInChildren<SpriteRenderer>();
            if (srFx != null)
            {
                float spriteDiameter = srFx.bounds.size.x;
                float desiredDiameter = spawnCircleRadius * 2f;
                float scaleFactor = desiredDiameter / spriteDiameter;
                circle.transform.localScale = Vector3.one * scaleFactor;
            }
        }

        yield return new WaitForSeconds(0.25f);

        // --- Rise animation ---
        float t = 0f;
        while (t < entryDuration)
        {
            t += Time.deltaTime;
            float lerp = t / entryDuration;
            transform.position = Vector3.Lerp(
                finalPos - Vector3.up * entryRiseHeight,
                finalPos,
                lerp
            );
            yield return null;
        }

        transform.position = finalPos;

        if (circle != null)
            Destroy(circle, spawnVFXDuration);

        // Unlock AI
        isAttacking = false;
        isHealing = false;
        hasEntered = true;
        SetState(BossState.Idle);
    }

    private void Update()
    {
        if (health == null || player == null || !health.enabled)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        // -----------------------------
        // Priority 1: Healing
        if (isHealing)
        {
            SetState(BossState.Healing);
            agent.isStopped = true; // stop movement while healing
            return;
        }

        // Priority 2: Attacking
        if (isAttacking)
        {
            SetState(BossState.Attacking);
            agent.isStopped = true; // stop movement while attacking
            return;
        }

        // -----------------------------
        // Periodic healing check
        if (Time.time - lastHealTime >= healCooldown && !isHealing)
        {
            StartCoroutine(HealNearbyEnemies());
        }

        // -----------------------------
        // Movement / AI decisions
        if (dist > followRange)
        {
            SetState(BossState.Idle);
            agent.isStopped = true;
        }
        else if (dist > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetState(BossState.Walking);
        }
        else
        {
            agent.isStopped = true;
            TryAttack();
        }

        FlipSprite();
    }

    private IEnumerator HealNearbyEnemies()
    {
        isHealing = true;
        SetState(BossState.Healing);

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
            SetState(BossState.Idle);
            yield break;
        }

        lastHealTime = Time.time;

        if (firsttime)
        {
            firsttime = false;
            isHealing = false;
            SetState(BossState.Idle);
            yield break;
        }

        isAttacking = true;

        animator.CrossFade("BringerOfDeath_cast", 0.1f, 0, 0f);
        yield return new WaitForSeconds(1f);

        if (healEffectPrefab != null)
        {
            Vector3 pos = transform.position;
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

            GameObject effect = Instantiate(healEffectPrefab, pos, rotation);

            SpriteRenderer srFx = effect.GetComponentInChildren<SpriteRenderer>();
            if (srFx != null)
            {
                float spriteDiameter = srFx.bounds.size.x;
                float desiredDiameter = healRange * 2f + 4.5f;
                float scaleFactor = desiredDiameter / spriteDiameter;
                effect.transform.localScale = Vector3.one * scaleFactor;
            }

            Animator fxAnim = effect.GetComponent<Animator>();
            if (fxAnim != null)
                fxAnim.Play("healarea", 0, 0f);

            yield return new WaitForSeconds(0.7f);
            Destroy(effect);
        }

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
        isHealing = false;
        SetState(BossState.Idle);
    }

    private void FollowPlayer()
    {
        // No longer used — NavMeshAgent handles this
    }

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
        SetState(BossState.Attacking);

        animator.CrossFade("BringerOfDeath_attack", 0.1f, 0, 0f);

        yield return new WaitForSeconds(0.45f);
        if (Vector3.Distance(transform.position, player.position) <= (attackRange + attacktolerance))
        {
            PlayerCombat pc = player.GetComponent<PlayerCombat>();
            if (pc != null && !pc.isDead)
                pc.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
        SetState(BossState.Idle);
    }

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

    private void SetState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        switch (currentState)
        {
            case BossState.Idle:
                animator.CrossFade("BringerOfDeath_idle", 0.1f);
                break;
            case BossState.Walking:
                animator.CrossFade("BringerOfDeath_walk", 0.1f);
                break;
            case BossState.Attacking:
                animator.CrossFade("BringerOfDeath_attack", 0.1f);
                break;
            case BossState.Healing:
                animator.CrossFade("BringerOfDeath_cast", 0.1f);
                break;
        }
    }
}
