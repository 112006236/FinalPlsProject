using UnityEngine;
using UnityEngine.UI;

public class CageHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image foreground;
    public Image damageBar;

    [Header("Settings")]
    public float shrinkDelay = 0.2f;
    public float shrinkSpeed = 0.5f;

    [Header("Health Bar Layers")]
    public int barCount = 3;               // how many bars (layers)
    public Color[] barColors;              // color for each layer (0 = top layer)

    private float maxHealth;
    private float targetFill;
    private float shrinkTimer;

    void Start()
    {
        if (foreground != null)
        {
            foreground.fillAmount = 1f;

            if (barColors != null && barColors.Length > 0)
                foreground.color = barColors[0];  // first layer color
        }

        if (damageBar != null)
            damageBar.fillAmount = 1f;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        targetFill = 1f;

        foreground.fillAmount = 1f;
        damageBar.fillAmount = 1f;

        UpdateColor(); // initialize correct color
    }

    public void UpdateHealth(float currentHealth)
    {
        if (maxHealth <= 0) return;

        targetFill = Mathf.Clamp01(currentHealth / maxHealth);

        // Instant update on the foreground bar
        if (foreground != null)
        {
            foreground.fillAmount = targetFill;
            UpdateColor();
        }

        shrinkTimer = shrinkDelay;
    }

    private void UpdateColor()
    {
        if (barColors == null || barColors.Length == 0) return;

        // which layer are we in?
        // Example: 3 bars → each bar = 33%
        float segmentSize = 1f / barCount;
        int barIndex = Mathf.Clamp((int)(foreground.fillAmount / segmentSize), 0, barCount - 1);

        foreground.color = barColors[barIndex];
    }

    void Update()
    {
        if (damageBar == null) return;

        if (shrinkTimer > 0)
        {
            shrinkTimer -= Time.deltaTime;
            return;
        }

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
