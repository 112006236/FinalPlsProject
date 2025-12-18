using UnityEngine;
using System.Collections;

public class Necromancer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Transform sprite; // child sprite object for flipping

    private SpriteRenderer sr;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 10f;  // distance to stop and shoot fireball
    public float retreatDistance = 4f; // distance to back away from player

    [Header("Combat")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballCooldown = 3f;
    public int fireballDamage = 15;
    public float attackRange = 12f;

    private float lastFireballTime = 0f;
    private bool isAttacking = false;
    private bool facingLeft = true;

    [Header("Health")]
    public float maxHealth = 200f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;
        if (currentHealth <= 0) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Handle movement
        if (!isAttacking)
        {
            if (dist > stopDistance)
                MoveTowardsPlayer();
            else if (dist < retreatDistance)
                RetreatFromPlayer();
            else
                animator.Play("idle");
        }

        // Handle fireball attack
        if (dist <= attackRange && Time.time - lastFireballTime >= fireballCooldown && !isAttacking)
        {
            StartCoroutine(PerformAttack1());
        }

        FlipSprite();
    }

    // ---------------- MOVEMENT ----------------
    private void MoveTowardsPlayer()
    {
        animator.Play("idle");
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.position += dir * moveSpeed * Time.deltaTime;
        animator.Play("idle"); // or walk if you add walk animation
    }

    private void RetreatFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        dir.y = 0;
        transform.position += dir * moveSpeed * Time.deltaTime;
        animator.Play("idle"); // or walk animation backward
    }

    // ---------------- FIREBALL ATTACK ----------------
    private IEnumerator PerformAttack1()
    {
        isAttacking = true;
        animator.Play("attack1", -1, 0f);
        animator.Update(0f);

        yield return new WaitForSeconds(0.45f);

        if (fireballPrefab != null && firePoint != null)
        {
            GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Fireball fb = go.GetComponent<Fireball>();
            if (fb != null)
            {
                fb.speed = 6f;        // ensure it's fast enough
                fb.maxLifetime = 6f;  // ensure it doesn't disappear immediately
                fb.Launch(player.position);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }


    // ---------------- FLIP SPRITE ----------------
    private void FlipSprite()
    {
        if (player == null) return;
        bool shouldFaceLeft = player.position.x < transform.position.x;
        sr.flipX = !shouldFaceLeft;
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.Play("hurt");
        }
    }

    private void Die()
    {
        animator.Play("death");
        Destroy(gameObject, 1f); // delay to let death animation play
    }
}
