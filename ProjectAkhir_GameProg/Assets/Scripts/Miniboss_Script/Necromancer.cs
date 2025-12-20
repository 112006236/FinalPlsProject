using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Necromancer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Transform sprite; // child sprite object for flipping

    private SpriteRenderer sr;
    private NavMeshAgent agent; // NEW: NavMeshAgent

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

    private float lastFireballTime = -100f;
    private bool isAttacking = false;

    [Header("Spawn / Entry")]
    [SerializeField] private GameObject spawnCircleEffect;
    [SerializeField] private float spawnCircleRadius = 3f;
    [SerializeField] private float entryRiseHeight = 1f;
    [SerializeField] private float entryDuration = 1f;
    [SerializeField] private float spawnVFXDuration = 1.2f;

    private bool hasEntered = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null) agent = gameObject.AddComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        StartCoroutine(EntrySequence());
    }

    private IEnumerator EntrySequence()
    {
        sr.enabled = true;
        Vector3 groundPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 finalPos = new Vector3(transform.position.x, 1.093f, transform.position.z);

        transform.position = finalPos - Vector3.up * entryRiseHeight;

        isAttacking = true;
        animator.Play("idle");

        GameObject circle = null;
        if (spawnCircleEffect != null)
        {
            Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
            circle = Instantiate(spawnCircleEffect, groundPos, rot);

            SpriteRenderer fxSr = circle.GetComponentInChildren<SpriteRenderer>();
            if (fxSr != null)
            {
                float spriteDiameter = fxSr.bounds.size.x;
                float desiredDiameter = spawnCircleRadius * 2f;
                float scaleFactor = desiredDiameter / spriteDiameter;
                circle.transform.localScale = Vector3.one * scaleFactor;
            }
        }

        yield return new WaitForSeconds(0.25f);

        float t = 0f;
        while (t < entryDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(finalPos - Vector3.up * entryRiseHeight, finalPos, t / entryDuration);
            yield return null;
        }

        transform.position = finalPos;
        if (circle != null) Destroy(circle, spawnVFXDuration);

        isAttacking = false;
        hasEntered = true;
    }

    private void Update()
    {
        if (!hasEntered || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Movement logic
        if (!isAttacking)
        {
            if (dist > stopDistance)
            {
                agent.isStopped = false;
                agent.speed = moveSpeed;
                agent.SetDestination(player.position);
                animator.Play("idle"); // could use walk animation if you have one
            }
            else if (dist < retreatDistance)
            {
                agent.isStopped = false;
                agent.speed = moveSpeed;
                Vector3 retreatDir = (transform.position - player.position).normalized;
                agent.SetDestination(transform.position + retreatDir * 2f); // move backward
                animator.Play("idle");
            }
            else
            {
                agent.isStopped = true;
                animator.Play("idle");
            }
        }

        // Fireball attack
        if (dist <= attackRange && Time.time - lastFireballTime >= fireballCooldown && !isAttacking)
        {
            StartCoroutine(PerformAttack1());
        }

        FlipSprite();
    }

    private IEnumerator PerformAttack1()
    {
        isAttacking = true;
        agent.isStopped = true; // stop movement during attack
        lastFireballTime = Time.time;

        animator.Play("attack1", -1, 0f);
        animator.Update(0f);

        yield return new WaitForSeconds(0.45f);

        if (fireballPrefab != null && firePoint != null)
        {
            GameObject go = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Fireball fb = go.GetComponent<Fireball>();
            if (fb != null)
            {
                fb.speed = 6f;
                fb.maxLifetime = 6f;
                fb.Launch(player.position);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    private void FlipSprite()
    {
        if (player == null || sr == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        // Vector from enemy to player
        Vector3 toPlayer = player.position - transform.position;

        // Camera's right direction
        Vector3 camRight = cam.transform.right;

        // Project onto camera right vector
        float dot = Vector3.Dot(toPlayer, camRight);

        // Sprite faces LEFT by default
        // Player on camera-right → flip
        sr.flipX = dot < 0f;
    }
}
