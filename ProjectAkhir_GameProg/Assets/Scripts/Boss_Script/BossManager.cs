using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Animator animator;   // Reference to the boss's Animator
    private bool isFlipped = false;
     [Header("Phase Settings")]
    public float phase2Time = 0.5f;
    private float timer;
    private bool isPhase2 = false;

    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float chargeSpeed = 15f;
    public float chargeDistance = 12f;

    private float ChargeTimer = 2f;

    public float AttackDistance = 3f;

    private bool isCharging = false;
    private Vector3 chargeTarget;
    public Transform player;

    [Header("Graphics / Animation")]
    public Transform sprite;
    public bool flipInsteadOfRotate = true;
    private Vector3 initialScale;
    private SpriteRenderer spriteRenderer;
    //private Animator anim;

    [Header("Effects")]
    public GameObject slashPrefab; // Drag your slash sprite/prefab here
    public Transform slashPoint;  // A child object positioned at the sword's tip

    private bool lookLeft;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (sprite != null) initialScale = sprite.localScale;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // Keep counting up time
        timer += Time.deltaTime;
        //OnDrawGizmos();
        HandleSpriteFlip();
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
        Debug.Log("in phase 2");
        ChargeTimer += Time.deltaTime;
        
        if (!isCharging)
        {
            //Debug.Log("test");
            float distance = Vector3.Distance(transform.position, player.position);
            //Debug.Log(distance);
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
                Debug.Log("ToAttack");
                //SpawnSlash();
                animator.SetTrigger("Attack2");
            }
        }
    }
    
     IEnumerator ChargeAttack()
    {
        isCharging = true;
        //agent.isStopped = true;
        Debug.Log("charge Attack");
        //animator.SetTrigger("Charge"); // Optional: charge animation trigger

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

    private void HandleSpriteFlip()
    {
        if (player.position.x > transform.position.x)
        {
            Debug.Log("look right");
            // if (flipInsteadOfRotate)  sprite.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
            // else                      sprite.rotation = Quaternion.Euler(0, 0, 0);
            spriteRenderer.flipX = false;  
            lookLeft = false;       
        }
        else
        {
            // if (flipInsteadOfRotate)  new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            // else                      sprite.rotation = Quaternion.Euler(0, 180, 0);
            Debug.Log("look left");
            spriteRenderer.flipX = true;
            lookLeft = true;
        }
    }

    public void SpawnSlash() 
    {
        // Instantiate the slash effect

        float slashScale = 10f;

        Quaternion flatRotation = Quaternion.Euler(90, 0, 0);

        GameObject slash = Instantiate(slashPrefab, transform.position, flatRotation);
        Debug.Log("slash spawn");
        // If the boss is flipped, flip the slash too!
        //slash.transform.localScale = new Vector3(slashScale, slashScale, slashScale);
        if (!lookLeft) 
        {
            slash.transform.localScale = new Vector3(-slashScale, slashScale, slashScale);
        } 
        else
        {
            slash.transform.localScale = new Vector3(slashScale, slashScale, slashScale);
        }
    }


    }
