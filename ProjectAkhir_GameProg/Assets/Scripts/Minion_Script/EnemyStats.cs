using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStats : MonoBehaviour
{
    public event Action<EnemyStats> OnDeath;

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

    private void Awake()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

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

        // Update health bar
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth);

        // Hurt VFX
        if (hurtVFX != null)
            Instantiate(hurtVFX, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
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
        if (other.CompareTag("Sword"))
        {
            Vector3 impactPoint = other.ClosestPoint(transform.position);
            if (swordImpactVFX != null)
                Instantiate(swordImpactVFX, impactPoint, Quaternion.identity);

            PlayerCombat playerCombat = other.GetComponentInParent<PlayerCombat>();
            if (playerCombat != null)
                TakeDamage(playerCombat.attackDamage);
        }

        SwordProjectile projectile = other.GetComponent<SwordProjectile>();
        if (projectile != null)
        {
            Debug.Log("Hit by projectile...");
            TakeDamage(projectile.attackDamage);

            // Optional: spawn VFX on projectile hit
            if (swordImpactVFX != null)
                Instantiate(swordImpactVFX, transform.position, Quaternion.identity);

            Destroy(other.gameObject); // remove projectile
        }
    }

    // ---------------- DEATH -----------------
    private void Die()
    {
        if (agent != null) agent.isStopped = true;
        if (enemyCollider != null) enemyCollider.enabled = false;

        // Notify listeners
        OnDeath?.Invoke(this);

        // Increment kill tracker
        if (EnemyKillTracker.Instance != null)
            EnemyKillTracker.Instance.AddKill();

        Destroy(gameObject, 0.2f);
    }

    // ---------------- HELPERS -----------------
    public float GetCurrentHealth() => currentHealth;
}
