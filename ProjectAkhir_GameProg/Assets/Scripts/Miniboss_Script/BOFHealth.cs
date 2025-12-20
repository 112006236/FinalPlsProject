using UnityEngine;
using System.Collections;

public class BOFHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    [HideInInspector] public bool isDead = false;

    private DragonWarrior enemyScript;
    public EnemyHealthBar healthBar; // reference to your new UI health bar script
    [SerializeField] private PlayerCombat playerCombat;
    private float playerDamage;

    [Header("Effects")]
    public ParticleSystem swordImpactVFX;

    [Header("Knockback Settings")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.15f;

    private bool isKnockedback;

    private Transform player;



    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<DragonWarrior>();

        if (playerCombat == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerCombat = playerObj.GetComponent<PlayerCombat>();
                player = playerObj.transform;
        }

        playerDamage = playerCombat.attackDamage;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth);

        // Only play hurt if not attacking or healing
        BringerOfDeath bossScript = GetComponent<BringerOfDeath>();
        if (animator != null && !isDead)
        {
            if (bossScript == null || (!bossScript.isAttacking && !bossScript.isHealing))
            {
                animator.Play("BringerOfDeath_hurt", 0, 0);
            }
        }

        if (currentHealth <= 0)
            Die();
    }

    public IEnumerator KnockbackRoutine()
    {
        Debug.Log("knocked");
        if (isKnockedback || player == null) yield break;

        isKnockedback = true;

        Vector3 dir = (transform.position - player.position).normalized;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            transform.position += dir * knockbackForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedback = false;
    }


    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (enemyScript != null)
            enemyScript.enabled = false;

        if (animator != null)
            animator.Play("BringerOfDeath_die", 0, 0);

        Destroy(gameObject, 1.15f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            TakeDamage(playerDamage);
        }
        SwordProjectile projectile = other.GetComponent<SwordProjectile>();
        if (projectile != null)
        {
            float dmg = projectile.attackDamage;

            if (playerCombat != null)
            {
                dmg = playerCombat.CalculateDamage(projectile.attackDamage, this);
                playerCombat.ApplyLifesteal(dmg);
            }

            TakeDamage(dmg);

            if (swordImpactVFX != null)
                Instantiate(swordImpactVFX, transform.position, Quaternion.identity);

            Destroy(other.gameObject);
        }
    }
}
