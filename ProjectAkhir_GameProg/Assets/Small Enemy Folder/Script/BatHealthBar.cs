using UnityEngine;
using UnityEngine.UI;

public class BatHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image foreground; 
    public Gradient healthGradient; 

    private float maxHealth;

    public void Start()
    {
        foreground.color = Color.red;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        UpdateHealth(health);
    }

    public void UpdateHealth(float currentHealth)
    {
        float fill = currentHealth / maxHealth;
        foreground.fillAmount = fill;

        if (healthGradient != null)
            foreground.color = healthGradient.Evaluate(fill);
    }
}
