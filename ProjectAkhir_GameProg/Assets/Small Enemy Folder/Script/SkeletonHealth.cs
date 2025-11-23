using UnityEngine;
using System.Collections;

public class SkeletonHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 50f;
    private float currentHealth;
    [HideInInspector] public bool isDead = false;

    [Header("References")]
    private SkeletonController skeletonController; 
    public EnemyHealthBar healthBar; 
    private UnityEngine.AI.NavMeshAgent agent;

    [Header("Damage Settings")]
    public float hurtDuration = 0.5f; 

    [Header("Knockback Settings")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.15f;
    private bool isKnockedback = false;

    [Header("Death Settings")]
    public float dieDuration = 2f; 

    void Start()
    {
        currentHealth = maxHealth;
        skeletonController = GetComponent<SkeletonController>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

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
            StartCoroutine(ApplyKnockback());
        }
    }

    private IEnumerator HurtRoutine()
    {
        if (skeletonController != null)
            skeletonController.enabled = false;

        if (agent != null)
            agent.isStopped = true;

        yield return new WaitForSeconds(hurtDuration); 

        if (!isDead)
        {
            if (skeletonController != null)
                skeletonController.enabled = true;

            if (agent != null && !isKnockedback)
                agent.isStopped = false;
        }
    }

    private IEnumerator ApplyKnockback()
    {
        if (isKnockedback) yield break;
        isKnockedback = true;

        if (agent != null) 
            agent.isStopped = true;

        // Get push direction by using player's position
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 dir = (transform.position - player.transform.position).normalized;

            float timer = 0f;
            while (timer < knockbackDuration)
            {
                transform.position += dir * knockbackForce * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        isKnockedback = false;

        if (!isDead && agent != null && !isKnockedback)
            agent.isStopped = false;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        
        if (skeletonController != null)
            skeletonController.enabled = false;

        if (agent != null)
            agent.isStopped = true;

        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;

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
