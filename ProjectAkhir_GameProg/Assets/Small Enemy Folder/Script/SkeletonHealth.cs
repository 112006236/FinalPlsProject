using UnityEngine;
using System.Collections;

public class SkeletonHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 50f;
    private float currentHealth;
    [HideInInspector] public bool isDead = false;

    [Header("References")]
    private Animator animator;
    private SkeletonController skeletonController; 
    public EnemyHealthBar healthBar; // Reference to your UI health bar script

    [Header("Damage Settings")]
    public float hurtDuration = 0.5f; 

    [Header("Death Settings")]
    public float dieDuration = 2f; // Duration of death animation (manually assigned)

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        skeletonController = GetComponent<SkeletonController>();

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }
    
    // --- Temporary Debug Damage ---
    void Update()
    {
        // Press 'H' to deal debug damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Skeleton took 5 damage from 'H' key press.");
            TakeDamage(5f);
        }
    }

    public void TakeDamage(float damage) 
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HurtRoutine());
        }
    }

    private IEnumerator HurtRoutine()
    {
        if (skeletonController != null)
            skeletonController.enabled = false;

        if (animator != null)
            animator.SetTrigger("isHurt"); 

        yield return new WaitForSeconds(hurtDuration); 

        if (skeletonController != null && !isDead)
            skeletonController.enabled = true;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        
        if (skeletonController != null)
            skeletonController.enabled = false;

        if (animator != null)
            animator.SetTrigger("isDead"); 

        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        // Start coroutine to destroy object after manually assigned die duration
        StartCoroutine(DieAndDestroy());
    }

    private IEnumerator DieAndDestroy()
    {
        yield return new WaitForSeconds(dieDuration);
        Destroy(gameObject);
    }
}
