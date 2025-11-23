using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image foreground;      // Main health bar (instant)
    public Image damageBar;       // The delayed white bar
    public Gradient healthGradient;

    [Header("Settings")]
    public float shrinkDelay = 0.2f; // Time before the white bar starts shrinking
    public float shrinkSpeed = 0.4f; // How fast the white bar shrinks

    private float maxHealth;
    private float targetFill;
    private float shrinkTimer;

    void Start()
    {
        if (foreground != null)
            foreground.color = Color.red;

        if (damageBar != null)
            damageBar.fillAmount = 1f;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        UpdateHealth(health);
        if (damageBar != null) 
            damageBar.fillAmount = 1f;
    }

    public void UpdateHealth(float currentHealth)
    {
        targetFill = currentHealth / maxHealth;

        // Main bar updates instantly
        foreground.fillAmount = targetFill;

        // Color update
        if (healthGradient != null)
            foreground.color = healthGradient.Evaluate(targetFill);

        // Reset the delay timer
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

        // Smoothly shrink the white bar
        if (damageBar.fillAmount > targetFill)
        {
            damageBar.fillAmount = Mathf.Lerp(
                damageBar.fillAmount,
                targetFill,
                Time.deltaTime * shrinkSpeed
            );
        }
    }
    public class HealthBarFix : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.localScale = Vector3.one; 
        }
    }
}
