using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthBar; // optional

    private Animator animator;
    [HideInInspector] public bool isDead = false;

    // reference to enemy script (DragonWarrior)
    private DragonWarrior enemyScript;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<DragonWarrior>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Debug.Log("enemy damaged");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update health bar
        if (healthBar != null)
            healthBar.value = currentHealth;

        // Play hurt animation
        if (animator != null && !isDead)
            animator.Play("hurt_DragonWarrior", 0, 0);

        // Die if health <= 0
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Disable enemy AI/movement
        if (enemyScript != null)
            enemyScript.enabled = false;

        if (animator != null)
            animator.Play("die_DragonWarrior", 0, 0);

        Destroy(gameObject, 1.8f);
    }
}
