using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image foreground;
    public Image damageBar;
    public Gradient healthGradient;

    [Header("Settings")]
    public float shrinkDelay = 0.2f;
    public float shrinkSpeed = 0.5f; // units per second

    private float maxHealth;
    private float targetFill;
    private float shrinkTimer;

    void Start()
    {
        if (foreground != null)
        {
            foreground.fillAmount = 1f;
            foreground.color = Color.red;
        }

        if (damageBar != null)
            damageBar.fillAmount = 1f;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;

        targetFill = 1f;

        if (foreground != null)
            foreground.fillAmount = 1f;

        if (damageBar != null)
            damageBar.fillAmount = 1f;
    }

    public void UpdateHealth(float currentHealth)
    {
        if (maxHealth <= 0) return;

        targetFill = Mathf.Clamp01(currentHealth / maxHealth);

        // Instant update
        if (foreground != null)
        {
            foreground.fillAmount = targetFill;

            if (healthGradient != null)
                foreground.color = healthGradient.Evaluate(targetFill);
        }

        // Reset timer
        shrinkTimer = shrinkDelay;
    }

    void Update()
    {
        if (damageBar == null) return;

        // Wait before shrinking
        if (shrinkTimer > 0)
        {
            shrinkTimer -= Time.deltaTime;
            return;
        }

        // Smooth shrink using MoveTowards
        if (damageBar.fillAmount > targetFill)
        {
            damageBar.fillAmount = Mathf.MoveTowards(
                damageBar.fillAmount,
                targetFill,
                shrinkSpeed * Time.deltaTime
            );
        }
    }
}
