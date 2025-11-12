using UnityEngine;
using System.Collections;

public class TreeHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    [HideInInspector] public bool isDead = false;

    [Header("References")]
    private Animator animator;
    private TreeController treeController; 
    public EnemyHealthBar healthBar; // Reference to your UI health bar script

    [Header("Damage Settings")]
    public float hurtDuration = 0.5f; 

    [Header("Death Settings")]
    public float dieDuration = 2f; // Duration of death animation (manually assigned)

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        treeController = GetComponent<TreeController>();

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }
    
    // --- Temporary Debug Damage ---
    void Update()
    {
        // Press 'H' to deal debug damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Tree took 5 damage from 'H' key press.");
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
        // Temporarily disable GolemController logic during hurt
        if (treeController != null)
            treeController.enabled = false;

        if (animator != null)
            animator.SetTrigger("isHurt"); 

        yield return new WaitForSeconds(hurtDuration); 

        if (treeController != null && !isDead)
            treeController.enabled = true;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        
        if (treeController != null)
            treeController.enabled = false;

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
