using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Enemy Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Settings")]
    public Slider healthSlider;      // assign in Inspector
    public Image healthFill;         // optional — for color change
    public Gradient healthGradient;  // optional — green to red gradient

    [Header("Destroy Settings")]
    public GameObject enemyObject;   // optional — defaults to self

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (enemyObject == null)
            enemyObject = gameObject;

        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;

            if (healthFill != null && healthGradient != null)
                healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
        }
    }

    void Die()
    {
        Debug.Log(enemyObject.name + " has been defeated!");
        Destroy(enemyObject);
    }
}
