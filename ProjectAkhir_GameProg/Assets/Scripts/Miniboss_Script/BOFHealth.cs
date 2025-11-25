using UnityEngine;

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

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<DragonWarrior>();

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

        if (animator != null && !isDead)
            animator.Play("BringerOfDeath_hurt", 0, 0);

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
            animator.Play("BringerOfDeath_die", 0, 0);

        Destroy(gameObject, 1.15f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            TakeDamage(playerDamage);
        }
    }
}
