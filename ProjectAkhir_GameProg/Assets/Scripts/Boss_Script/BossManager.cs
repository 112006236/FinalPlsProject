using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mono.CompilerServices.SymbolWriter;
using UnityEditor.UIElements;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Animator animator;   // Reference to the boss's Animator
    private EnemyStats stats;
    private float maxHealth;
    private BossMeteorAttack bossMeteorAttack;
    private bool isFlipped = false;
     [Header("Phase Settings")]
    public float phase2Time = 0.5f;
    private float timer;
    private bool isPhase2 = false;
    private bool isPhase3 = false;

    private bool isPhase4 = false;

    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float chargeSpeed = 15f;
    

    private float ChargeTimer = 2f;

    public float AttackDistance = 4f;

    private bool isCharging = false;
    private Vector3 chargeTarget;
    public Transform player;

    [Header("Graphics / Animation")]
    public Transform sprite;
    private bool flipInsteadOfRotate = false;
    private Vector3 initialScale;
    private SpriteRenderer spriteRenderer;
    //private Animator anim;

    [Header("Effects")]
    public GameObject slashPrefab; // Drag your slash sprite/prefab here
    public Transform slashPoint;  // A child object positioned at the sword's tip
    private bool lookLeft;

    [Header("Area attack")]

    public GameObject AreaAttackWarningCircle;
    public float AreaAttackDelay = 1.5f;
    private bool isPerformingAreaAttack = false;


    [Header("Charge Attack Settings")]
    public GameObject lineWarningPrefab; // Drag a long rectangle prefab here
    public float trackingDuration = 2.5f; // Time indicator follows player
    public float lockDuration = 1.5f;     // Time indicator stays still before dash
    public float chargeDistanceMax = 40f; // Max length of the indicator/dash
    public float chargeDistance = 21f;

    public float chargeDamage = 20f;

    private bool hasHitPlayerThisCharge = false;

    [Header("Attack Cooldowns")]
    public float chargeCooldown = 5f;
    public float areaAttackCooldown = 2.5f;

    public float meteorCooldown = 10f;

    public float chargeCooldownRandom = 2f;

    private float chargeCooldownTimer = 0f;
    private float areaAttackCooldownTimer = 0f;

    private float meteorCooldownTimer = 0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (sprite != null) initialScale = sprite.localScale;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        stats = GetComponent<EnemyStats>();
        bossMeteorAttack = GetComponent<BossMeteorAttack>();
        maxHealth = stats.GetCurrentHealth();
    }

    void Update()
    {
        // Keep counting up time
        timer += Time.deltaTime;
        chargeCooldownTimer += Time.deltaTime;
        areaAttackCooldownTimer += Time.deltaTime;
        meteorCooldownTimer += Time.deltaTime;
        Debug.Log("current Boss Health: " + stats.GetCurrentHealth());
        Debug.Log(maxHealth*(70.0/100.0));
        //OnDrawGizmos();
        HandleSpriteFlip();

        if (!isPhase3 && stats.GetCurrentHealth() < maxHealth*(70.0/100.0))
        {
            setPhase3();
        }

        if (!isPhase4 && stats.GetCurrentHealth() < maxHealth*(50.0/100.0))
        {
            setPhase4();
        }
           
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

        if (isPhase3)
        {
            Phase3();
        }

        if (isPhase4)
        {
            Phase4();
        }
    }

    public void setPhase3()
    {
        Debug.Log("setPhase3");
        isPhase2 = false;
        isPhase3 = true;
    }

    public void setPhase4()
    {
        bossMeteorAttack.scatterChance = 0.5f;
        bossMeteorAttack.meteorCount = 8;
        isPhase2 = false;
        isPhase3 = false;
        isPhase4 = true;
    }

    void Phase2()
    {
        ChargeTimer += Time.deltaTime;
        if (isCharging || isPerformingAreaAttack) return;

        float distance = Vector3.Distance(transform.position, player.position);
  
         if (distance <= AttackDistance && areaAttackCooldownTimer >= areaAttackCooldown)
        {
           StartCoroutine(AreaAttackRoutine());
           areaAttackCooldownTimer = -1f;
           chargeCooldownTimer = 4f;
           return; 
        }       
        
    }
    
    void Phase3()
    {
        Debug.Log("in Phase 3");
        ChargeTimer += Time.deltaTime;
        if (isCharging || isPerformingAreaAttack) return;

        float distance = Vector3.Distance(transform.position, player.position);

  
         if (distance <= AttackDistance && areaAttackCooldownTimer >= areaAttackCooldown)
        {
           StartCoroutine(AreaAttackRoutine());
           areaAttackCooldownTimer = -1f;
           chargeCooldownTimer = 7f;
           return; 
        }
        
        
        if (distance <= chargeDistance && chargeCooldownTimer >= chargeCooldown)
        {
            StartCoroutine(ChargeAttack());
            chargeCooldownTimer = -Random.Range(0f, chargeCooldownRandom);
            areaAttackCooldownTimer -= 3f;
            return;
        }
    }

    void Phase4()
    {
        ChargeTimer += Time.deltaTime;
        if (isCharging || isPerformingAreaAttack) return;

        if (meteorCooldownTimer >= meteorCooldown)
        {
            animator.SetTrigger("WalktoMeteor");
            areaAttackCooldownTimer -= 3f;
            chargeCooldownTimer -= 3f;
            meteorCooldownTimer = 0;
        }

        float distance = Vector3.Distance(transform.position, player.position);

  
         if (distance <= AttackDistance && areaAttackCooldownTimer >= areaAttackCooldown)
        {
           StartCoroutine(AreaAttackRoutine());
           areaAttackCooldownTimer = 1f;
           chargeCooldownTimer = 8.5f;
           return; 
        }
        
        
        if (distance <= chargeDistance && chargeCooldownTimer >= chargeCooldown)
        {
            StartCoroutine(ChargeAttack());
            chargeCooldownTimer = -Random.Range(0f, -2f);
            areaAttackCooldownTimer = 0f;
            return;
        }
    }
    

    IEnumerator ChargeAttack()
    {
        isCharging = true;
        hasHitPlayerThisCharge = false;
        //ChargeTimer = 0f;

        animator.SetTrigger("WalktoIdle");
        Vector3 Offset = new Vector3(0,1,0);
        GameObject warningLine = Instantiate(
            lineWarningPrefab,
            transform.position - Offset,
            Quaternion.identity
        );

        warningLine.transform.SetParent(transform);
        warningLine.transform.localPosition = Vector3.zero;

        Vector3 lockedDir = transform.right;
        float elapsed = 0f;

        // TRACKING PHASE
        while (elapsed < trackingDuration)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            dir.Normalize();

            lockedDir = dir;

            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            warningLine.transform.rotation = Quaternion.Euler(0, -angle, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // LOCK PHASE
        var sr = warningLine.GetComponentInChildren<Renderer>();
        if (sr) sr.material.color = Color.red;

        yield return new WaitForSeconds(lockDuration);

        Destroy(warningLine);

        // DASH
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + lockedDir * chargeDistance;

        float dashDuration = 0.7f;
        float t = 0f;

        while (t < dashDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t / dashDuration);
            yield return null;
        }

        animator.SetTrigger("IdletoWalk");

        //yield return new WaitForSeconds(3.5f);
        isCharging = false;
    }

    IEnumerator AreaAttackRoutine()
    {
        isPerformingAreaAttack = true;
        animator.SetTrigger("WalktoIdle");

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
        GameObject warning = Instantiate(AreaAttackWarningCircle, spawnPos, Quaternion.Euler(0,0,0));

        // --- GROWING EFFECT START ---
        Vector3 finalScale = warning.transform.localScale; // Store the original prefab scale
        warning.transform.localScale = Vector3.zero;        // Start at size 0

        AreaAttackEffect attackEffect = warning.GetComponent<AreaAttackEffect>();

        float growthDuration = 0.2f; // How long it takes to grow (e.g., 0.5 seconds)
        float timer = 0f;

        while (timer < growthDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / growthDuration;
            // Use Lerp for a smooth linear growth
            warning.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, progress);
            yield return null;
        }
        
        yield return new WaitForSeconds(AreaAttackDelay);

        animator.SetTrigger("IdletoAttack");

        yield return new WaitForSeconds(0.2f);

        if (attackEffect != null)
        {
            attackEffect.TriggerDamage();
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(warning);

        //yield return new WaitForSeconds(3.5f);
        isPerformingAreaAttack = false;

    }

    private void Flip()
    {
        // if (player == null) return;
        // bool playerIsRight = player.position.x > transform.position.x;
        // sprite.localScale = new Vector3(playerIsRight ? sprite.localScale.x : -1*sprite.localScale.x, sprite.localScale.y, sprite.localScale.z);
        // if (player.position.x > transform.position.x)
        if (player.position.x > transform.position.x)
        {
            if (flipInsteadOfRotate)  sprite.localScale = new Vector3(sprite.localScale.x, sprite.localScale.y, sprite.localScale.z);
            else                      sprite.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            if (flipInsteadOfRotate)  sprite.localScale = new Vector3(-1*sprite.localScale.x, sprite.localScale.y, sprite.localScale.z);
            else                      sprite.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    
    private void HandleSpriteFlip()
    {
        if (player.position.x > transform.position.x)
        {
            //Debug.Log("look right");
            // if (flipInsteadOfRotate)  sprite.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
            // else                      sprite.rotation = Quaternion.Euler(0, 0, 0);
            spriteRenderer.flipX = false;  
            lookLeft = false;       
        }
        else
        {
            // if (flipInsteadOfRotate)  new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            // else                      sprite.rotation = Quaternion.Euler(0, 180, 0);
            //Debug.Log("look left");
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

    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log("ChargeCollisiotn");
        if (!isCharging) return;
        if (hasHitPlayerThisCharge) return;

        

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCombat health = collision.gameObject.GetComponent<PlayerCombat>();
            if (health != null)
            {
                health.TakeDamage(chargeDamage);
                hasHitPlayerThisCharge = true;
            }
        }
    }


    }
