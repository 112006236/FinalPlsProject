using UnityEngine;

public class MultiHealthObject : MonoBehaviour
{
    [Header("Health Settings")]
    public float healthPerBar = 100f;       // Health per bar
    public int numberOfBars = 1;            // Total bars
    public bool useHealthBar = false;       // Should we update a UI bar?

    [Header("UI")]
    public EnemyHealthBar healthBar;        // Can also be reused for breakables

    private float currentHealth;            // Total health
    private float currentBarHealth;         // Health left in current bar
    private int currentBarIndex = 1;        // 1-based

    [Header("Optional VFX")]
    public GameObject destroyVFX;
    public GameObject barBreakVFX;

    public event System.Action OnDestroyed;

    private void Start()
    {
        ArenaControl.Instance?.RegisterCage(this);
    }


    private void Awake()
    {
        currentHealth = healthPerBar * numberOfBars;
        currentBarHealth = healthPerBar;

        if (useHealthBar && healthBar != null)
            healthBar.SetMaxHealth(healthPerBar);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= damage;
        currentBarHealth -= damage;

        // Update health bar UI
        if (useHealthBar && healthBar != null)
            healthBar.UpdateHealth(currentBarHealth);

        // Check if current bar depleted
        while (currentBarHealth <= 0f && currentHealth > 0f)
        {
            currentBarIndex++;
            currentBarHealth += healthPerBar;

            if (useHealthBar && healthBar != null)
                healthBar.SetMaxHealth(healthPerBar);

            if (barBreakVFX != null)
                Instantiate(barBreakVFX, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0f)
            Die();
    }

    //IMPORTANT IN ALL OBJECT THAT REQUIRE PLAYER COMBAT MECHANIC,
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            PlayerCombat playerCombat = other.GetComponentInParent<PlayerCombat>();
            if (playerCombat != null)
                TakeDamage(playerCombat.attackDamage);
        }
    }

    private void Die()
    {
        if (destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);

        OnDestroyed?.Invoke();

        Destroy(gameObject);
    }


    // Helper methods
    public float GetCurrentHealth() => currentHealth;
    public float GetCurrentBarHealth() => currentBarHealth;
    public int GetCurrentBarIndex() => currentBarIndex;
    public int GetTotalBars() => numberOfBars;
}
