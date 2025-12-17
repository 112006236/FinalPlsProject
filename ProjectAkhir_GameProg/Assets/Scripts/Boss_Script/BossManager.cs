using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Animator animator;   // Reference to the boss's Animator
    private bool isFlipped = false;
     [Header("Phase Settings")]
    public float phase2Time = 2f;
    private float timer;
    private bool isPhase2 = false;

    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float chargeSpeed = 15f;
    public float chargeDistance = 12f;

    private float ChargeTimer = 2f;

    public float AttackDistance = 3.2f;

    private bool isCharging = false;
    private Vector3 chargeTarget;
    public Transform player;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        // Keep counting up time
        timer += Time.deltaTime;

        // After 15 seconds, tell the Animator to go to state 2
        if (!isPhase2 && timer >= phase2Time)
        {
            animator.SetTrigger("toWalk");
            isPhase2 = true;
        }

        if (isPhase2)
        {
            Phase2();
        }
    }

    void Phase2()
    {
        ChargeTimer += Time.deltaTime;
        
        if (!isCharging)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Trigger charge if close enough
            if (distance <= chargeDistance && ChargeTimer >= 5f)
            {
                StartCoroutine(ChargeAttack());
            }
        }

        if (!isCharging)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= AttackDistance)
            {
                animator.SetTrigger("Attack2");
            }
        }
    }
    
     IEnumerator ChargeAttack()
    {
        isCharging = true;
        //agent.isStopped = true;

        animator.SetTrigger("Charge"); // Optional: charge animation trigger

        yield return new WaitForSeconds(0.5f); // wind-up delay before dash

        // Get player’s last known position
        chargeTarget = player.position;

        // 🔹 Dash straight at player
        float chargeDuration = 3f;
        float elapsed = 0f;

        while (elapsed < chargeDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, chargeTarget, chargeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // After charge ends, resume walking
        //agent.isStopped = false;
        isCharging = false;
        ChargeTimer = 0f;
        //agent.speed = walkSpeed;
    }

    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 flipped = transform.localScale;
            flipped.z *= -1f;

            if (transform.position.x < player.position.x && isFlipped)
            {
                transform.localScale = flipped;
                transform.Rotate(0f, 180f, 0f);
                isFlipped = false;
            }
            else if (transform.position.x > player.position.x && !isFlipped) {
                transform.localScale = flipped;
                transform.Rotate(0f, 180f, 0f);
                isFlipped = true;
            }
        }
    }
}
