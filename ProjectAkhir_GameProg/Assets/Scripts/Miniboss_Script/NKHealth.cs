using UnityEngine;
using System.Collections;

public class NKHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    [HideInInspector] public bool isDead = false;

    private NinjaKnight enemyScript;
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
        enemyScript = GetComponent<NinjaKnight>();

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

        // Only play take_hit if not attacking or dashing
        if (animator != null && !isDead)
        {
            if (enemyScript != null)
            {
                if (enemyScript.canNormalAttack &&enemyScript.canDashAttack)
                    animator.Play("take_hit", 0, 0);
            }
            else
            {
                animator.Play("take_hit", 0, 0);
            }
        }

        if (currentHealth <= 0)
            Die();
    }


    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (enemyScript != null)
            enemyScript.enabled = false;

        if (animator != null)
            animator.Play("death", 0, 0);

        Destroy(gameObject, 2f);
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
}
