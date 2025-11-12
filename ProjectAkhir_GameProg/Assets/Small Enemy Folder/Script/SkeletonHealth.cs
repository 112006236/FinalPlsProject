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
    public EnemyHealthBar healthBar; 

    [Header("Damage Settings")]
    public float hurtDuration = 0.5f; 

    [Header("Death Settings")]
    public float dieDuration = 2f; 

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        skeletonController = GetComponent<SkeletonController>();

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }
    
    void Update()
    {
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

        StartCoroutine(DieAndDestroy());
    }

    private IEnumerator DieAndDestroy()
    {
        yield return new WaitForSeconds(dieDuration);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            TakeDamage(other.gameObject.GetComponentInParent<PlayerCombat>().attackDamage);
        }
    }

}
