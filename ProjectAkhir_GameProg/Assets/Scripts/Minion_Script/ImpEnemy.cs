using UnityEngine;
using UnityEngine.AI;

public class ImpEnemy : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public ParticleSystem attackVFX;

    [Header("Graphics / Animation")]
    public Transform sprite;
    public bool flipInsteadOfRotate = true;
    private Animator anim;

    private float lastAttackTime;
    private Transform player;
    private NavMeshAgent agent;
    private EnemyStats stats;
    public Transform healthBar;

    private void Start()
    {
        stats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        anim = GetComponentInChildren<Animator>();
        if (sprite == null) sprite = transform;
    }

    private void Update()
    {
        if (player == null || stats.GetCurrentHealth() <= 0)
            return;

        MoveTowardsPlayer();
        HandleSpriteFlip();
        HandleAttack();
        HandleAnimation();
    }

    // ---------------- AI ---------------------

    private void MoveTowardsPlayer()
    {
        agent.SetDestination(player.position);
    }

    // ---------------- ANIMATION ---------------------

    private void HandleAnimation()
    {
        if (anim == null) return;

        bool isRunning = agent.velocity.magnitude > 0.1f;
        anim.SetBool("IsRunning", isRunning);
    }

    // ---------------- SPRITE FLIP ---------------------

    private void HandleSpriteFlip()
    {
        if (player.position.x > transform.position.x)
        {
            if (flipInsteadOfRotate)  sprite.localScale = new Vector3(1, 1, 1);
            else                      sprite.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            if (flipInsteadOfRotate)  sprite.localScale = new Vector3(-1, 1, 1);
            else                      sprite.rotation = Quaternion.Euler(0, 180, 0);
        }
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
        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
        if (playerCombat != null)
            playerCombat.TakeDamage(stats.attackDamage);

        if (attackVFX != null)
        {
            attackVFX.Play();
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
