using UnityEngine;
using UnityEngine.AI;

public class BigBoi : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2.4f;
    public float attackCooldown = 2.2f;
    public ParticleSystem attackVFX;

    [Header("Graphics / Animation")]
    public Transform sprite;
    private Animator anim;

    [Header("Death Spawn Settings")]
    public GameObject spawnEnemyPrefab;
    public int spawnCount = 3;

    private float lastAttackTime;
    private Transform player;
    private NavMeshAgent agent;
    private EnemyStats stats;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        anim = GetComponentInChildren<Animator>();
        if (sprite == null) sprite = transform;

        if (stats != null)
            stats.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        if (stats != null)
            stats.OnDeath -= HandleDeath;
    }

    private void Update()
    {
        if (player == null || stats.GetCurrentHealth() <= 0) return;

        MoveTowardsPlayer();
        HandleSpriteFlip();
        HandleAttack();
        UpdateAnimatorParameters();
    }

    // ---------------- MOVEMENT ---------------------
    private void MoveTowardsPlayer()
    {
        agent.SetDestination(player.position);
    }

    // ---------------- ATTACK ---------------------
    private void HandleAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            DoAttack();
        }
    }

    private void DoAttack()
    {
        if (anim != null)
            anim.SetBool("IsAttacking", true);

        PlayerCombat pc = player.GetComponent<PlayerCombat>();
        if (pc != null)
            pc.TakeDamage(stats.attackDamage);

        if (attackVFX != null)
            attackVFX.Play();

        // Reset attack parameter after animation duration
        float clipLength = anim.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(StopAttackAnimation), clipLength);
    }

    private void StopAttackAnimation()
    {
        if (anim != null)
            anim.SetBool("IsAttacking", false);
    }

    // ---------------- ANIMATION PARAMETERS ---------------------
    private void UpdateAnimatorParameters()
    {
        if (anim == null) return;

        float speed = agent.velocity.magnitude;
        anim.SetBool("IsRunning", speed > 0.1f);
    }

    // ---------------- SPRITE FLIP ---------------------
    private void HandleSpriteFlip()
    {
        if (player == null) return;
        bool playerIsRight = player.position.x > transform.position.x;
        sprite.localScale = new Vector3(playerIsRight ? 1 : -1, 1, 1);
    }

    // ---------------- DEATH SPAWN ---------------------
    private void HandleDeath(EnemyStats es)
    {
        if (spawnEnemyPrefab == null) return;

        for (int i = 0; i < spawnCount; i++)
            Instantiate(spawnEnemyPrefab,
                        transform.position + Random.insideUnitSphere * 0.5f,
                        Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
