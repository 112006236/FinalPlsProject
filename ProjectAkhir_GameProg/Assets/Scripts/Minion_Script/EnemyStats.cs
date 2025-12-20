using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStats : MonoBehaviour
{
    public event Action<EnemyStats> OnDeath;
    [Header("Settings")]
    public bool isBoss = false;
    [Header("Base Stats")]
    public float maxHealth = 50f;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;

    [Header("UI")]
    public EnemyHealthBar healthBar;

    [Header("Effects")]
    public GameObject hurtVFX;
    public ParticleSystem swordImpactVFX;

    [Header("Knockback Settings")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.15f;

    private float currentHealth;
    private bool isKnockedback;
    private NavMeshAgent agent;
    private Transform player;
    private Collider enemyCollider;

    private bool isStunned = false;
    [SerializeField] private Animator stunVFXAnimator;

    // 🔥 NEW: cache PlayerCombat
    private PlayerCombat playerCombat;
    [SerializeField] private ParticleSystem critVFX;

    private void Awake()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerCombat = p.GetComponent<PlayerCombat>(); // 🔥 NEW
        }

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);

        enemyCollider = GetComponent<Collider>();
        if (enemyCollider == null)
            enemyCollider = gameObject.AddComponent<CapsuleCollider>();
    }

    // ---------------- DAMAGE -----------------
    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth);

        if (dmg - playerCombat.attackDamage > 0.1f)
        {
            Instantiate(critVFX, transform.position, Quaternion.identity);
        } else
        {
            Instantiate(swordImpactVFX, transform.position, Quaternion.identity);
        }
        

        if (currentHealth <= 0)
        {
            if (isBoss) return;
            Die();
            return;
        }

        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        if (isKnockedback || player == null) yield break;

        isKnockedback = true;
        agent.isStopped = true;

        Vector3 dir = (transform.position - player.position).normalized;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            transform.position += dir * knockbackForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedback = false;
        agent.isStopped = false;
    }

    // ---------------- TRIGGER DAMAGE -----------------
    private void OnTriggerEnter(Collider other)
    {
        // ---------- MELEE ----------
        if (other.CompareTag("Sword"))
        {
            Vector3 impactPoint = other.ClosestPoint(transform.position);
            if (swordImpactVFX != null)
                Instantiate(swordImpactVFX, impactPoint, Quaternion.identity);

            if (playerCombat == null) return;

            float dmg = playerCombat.CalculateDamage(playerCombat.attackDamage, this);
            TakeDamage(dmg);
            playerCombat.ApplyLifesteal(dmg);
        }

        // ---------- PROJECTILE ----------
        SwordProjectile projectile = other.GetComponent<SwordProjectile>();
        if (projectile != null)
        {
            float dmg = projectile.attackDamage;

            if (playerCombat != null)
            {
                dmg = playerCombat.CalculateDamage(projectile.attackDamage, this);
                playerCombat.ApplyLifesteal(dmg);
            }
            Debug.Log("projecte");

            TakeDamage(dmg);

            if (swordImpactVFX != null)
                Instantiate(swordImpactVFX, transform.position, Quaternion.identity);

            Destroy(other.gameObject);
        }
    }

    public void ApplyStun(float duration)
    {
        if (isStunned) return;
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        stunVFXAnimator.SetTrigger("Enter");
        // if (agent != null) agent.isStopped = true;
        if (agent != null) agent.speed = 0f;
        yield return new WaitForSeconds(duration);
        // if (agent != null) agent.isStopped = false;
        if (agent != null) agent.speed = moveSpeed;
        isStunned = false;
        stunVFXAnimator.SetTrigger("Exit");
    }

    public void ApplyStagger(float multiplier)
    {
        // Example: briefly reduce speed or interrupt attack
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            StartCoroutine(StaggerRoutine(multiplier));
    }

    private IEnumerator StaggerRoutine(float multiplier)
    {
        float originalSpeed = agent.speed;
        agent.speed *= (1f / multiplier); // reduce movement
        yield return new WaitForSeconds(0.3f);
        agent.speed = originalSpeed;
    }

    // ---------------- DEATH -----------------
    private void Die()
    {
        if (agent != null) agent.isStopped = true;
        if (enemyCollider != null) enemyCollider.enabled = false;

        // 🔥 COOLDOWN REFUND TRIGGER
        if (playerCombat != null)
        {
            playerCombat.OnEnemyKilled();
        }

        // Notify listeners
        OnDeath?.Invoke(this);

        if (EnemyKillTracker.Instance != null)
            EnemyKillTracker.Instance.AddKill();

        Destroy(gameObject, 0.2f);
    }

    // ---------------- HELPERS -----------------
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
