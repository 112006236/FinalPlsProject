using UnityEngine;

public class NHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    [HideInInspector] public bool isDead = false;

    private NinjaKnight enemyScript;
    public EnemyHealthBar healthBar; // reference to your new UI health bar script
    [SerializeField] private PlayerCombat playerCombat;
    private float playerDamage;

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

        if (animator != null && !isDead)
            animator.Play("hurt", 0, 0);

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
    }
}
