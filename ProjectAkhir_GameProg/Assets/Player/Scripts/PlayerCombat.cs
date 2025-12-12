using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Stats")]
    public float initHP = 100.0f;
    [System.NonSerialized] public float HP;
    [System.NonSerialized] public bool isDead;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private Image healthBarFill;
    private float hpTargetFill;
    private float hpFullFill = .43f;
    private float smoothTime = 0.25f;
    private float currVelocity;
    private float hpTargetAngle;

    [Header("Combo System")]
    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    public float attackDamage = 25f;    // Damage per hit

    public float timeBetweenCombo = 0.5f;
    public float timeBetweenAttacks = 0.2f;
    public float exitComboTime = 0.5f;

    // SET ALL TO FALSE LATER
    [HideInInspector] public bool[] specialMoveUnlocked = { true, true, true };

    [Header("Special Move 1")]
    [SerializeField] private float sm1Cooldown = 5f;
    private float sm1StartTime;
    private float sm1Duration;
    [SerializeField] private float sm1Damage = 15f;
    [SerializeField] private GameObject sm1Projectile;
    [SerializeField] private GameObject sm1ProjectileSpawnEffect;
    [SerializeField] private float radiusFromPlayer = 1f;
    [SerializeField] private int projectilePerWave = 6;
    [SerializeField] private int waveCount = 2;
    [SerializeField] private float waveDuration = 0.5f;
    [SerializeField] private float waveDetaTime = 0.8f;
    [SerializeField] private float waveDeltaAngle = 15f;
    [SerializeField] private Image sm1Icon;
    [SerializeField] private Image sm1CooldownFill;

    [Header("Special Move 2")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float sm2Cooldown = 5f;
    private float sm2StartTime;
    [SerializeField] private float sm2Radius = 10f;
    [SerializeField] private float sm2Damage = 10f;
    private float sm2VFXScale;
    [SerializeField] private ParticleSystem sm2ImpactVFX;
    [SerializeField] private ParticleSystem sm2WaveVFX;
    [SerializeField] private ParticleSystem sm2EnemyHitVFX;
    [SerializeField] private Image sm2Icon;
    [SerializeField] private Image sm2CooldownFill;
    

    [Header("Animation")]
    public Animator anim;
    public List<ParticleSystem> slashVFXs;
    public float slashVFXScale = 2.0f;
    public Transform attackPoint;       // Empty object in front of the sword
    [SerializeField] private Collider swordCollider;

    [Header("UI")]
    [SerializeField] private GameObject deathUI;

    
    PlayerInputActions inputs;
    [HideInInspector] public bool inCombo;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Init player health
        HP = initHP;
        isDead = false;
        hpTargetFill = hpFullFill;

        // Init player combo attack
        inCombo = false;

        // Init player special moves
        sm1Duration = waveCount * (waveDetaTime + waveDuration);
        sm1StartTime = Time.time - sm1Duration;

        sm2VFXScale = .84f * sm2Radius;
    }

    void Start()
    {
        inputs = new PlayerInputActions();
        inputs.Player.Enable();
        inputs.Player.Attack.performed += Attack;
        inputs.Player.SM1.performed += SM1;
        inputs.Player.SM2.performed += SM2;
    }

    void Update()
    {
        ExitAttack();
        // Optional: Flip facing based on input

        swordCollider.enabled = inCombo;

        // Update health bar fill        
        float _fill = Mathf.SmoothDampAngle(healthBarFill.fillAmount, hpTargetFill, ref currVelocity, smoothTime);
        healthBarFill.fillAmount = _fill;

        // Update SM cooldown fill
        UpdateSMCooldownUI();

        // Debug purposes only! 
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(40.0f);
        }
    }

    private void UpdateSMCooldownUI()
    {
        // SM1
        Color c = sm1Icon.color;
        if (Time.time - sm1Duration - sm1StartTime < sm1Cooldown)
        {
            c.a = .7f;
            sm1Icon.color = c;

            if (Time.time - sm1StartTime < sm1Duration)
            {
                sm1CooldownFill.fillAmount = 0f;
            }
            else
            {
                sm1CooldownFill.fillAmount = (Time.time - sm1StartTime - sm1Duration) / sm1Cooldown;
            }
        }
        else
        {
            c.a = 1f;
            sm1Icon.color = c;
        }

        // SM2
        c = sm2Icon.color;
        if (Time.time - sm2StartTime < sm2Cooldown)
        {
            c.a = .7f;
            sm2Icon.color = c;

            sm2CooldownFill.fillAmount = (Time.time - sm2StartTime) / sm2Cooldown;
        }
        else
        {
            c.a = 1f;
            sm2Icon.color = c;
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (isDead) return;

        inCombo = true;
        CancelInvoke("ExitCombo");

        if (Time.time - lastClickedTime >= timeBetweenAttacks)
        {
            // Debug.Log("Attack Pattern " + comboCounter);

            // Play the correct attack animation
            anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
            anim.Play("Attack", 0, 0);

            // Correct VFX flipping
            if (playerMovement.facingRight)
            {
                slashVFXs[comboCounter].transform.localScale = slashVFXScale * new Vector3(1, 1, 1);
            }
            else
            {
                slashVFXs[comboCounter].transform.localScale = slashVFXScale * new Vector3(-1, 1, 1);
            }

            slashVFXs[comboCounter].Play();

            // Deal damage in front of player
            // DealDamage();

            comboCounter++;
            lastClickedTime = Time.time;

            if (comboCounter >= combo.Count)
                comboCounter = 0;
        }
        else
        {
            // Debug.Log("Waiting attack cooldown...");
        }
    }

    void SM1(InputAction.CallbackContext context)
    {
        if (Time.time - sm1Duration - sm1StartTime >= sm1Cooldown)
        {
            sm1StartTime = Time.time;
            StartCoroutine(PerformSM1());
        }
    }

    IEnumerator PerformSM1()
    {
        for (int i = 0; i < waveCount; i++)
        {
            List<Vector3> forwards = new List<Vector3>();
            for (int j = 0; j < projectilePerWave; j++)
            {
                forwards.Add(
                    Quaternion.AngleAxis((float)j / projectilePerWave * 360.0f + i * waveDeltaAngle, transform.up) 
                    * transform.forward);
            }

            StartCoroutine(FireProjectiles(forwards));

            yield return new WaitForSeconds(waveDetaTime);
        }
    }

    IEnumerator FireProjectiles(List<Vector3> forwards)
    {
        for (int i = 0; i < projectilePerWave; i++)
        {
            // Fire sword projectile
            Vector3 spawnPoint = transform.position + radiusFromPlayer * forwards[i] + .5f * transform.up;

            Instantiate(sm1ProjectileSpawnEffect, spawnPoint, Quaternion.Euler(0, 0, 0));

            GameObject proj = Instantiate(sm1Projectile, spawnPoint, Quaternion.Euler(0, 0, 0));
            proj.transform.up = forwards[i];
            proj.GetComponent<SwordProjectile>().attackDamage = sm1Damage;
            // Add delay between projectiles
            yield return new WaitForSeconds(waveDuration / projectilePerWave);
        }   
    }

    void SM2(InputAction.CallbackContext context)
    {
        if (Time.time - sm2StartTime >= sm2Cooldown)
        {
            sm2StartTime = Time.time;

            ParticleSystem impactVFX = Instantiate(sm2ImpactVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            ParticleSystem waveVFX = Instantiate(sm2WaveVFX, transform.position, Quaternion.Euler(90, 0, 0));

            impactVFX.transform.localScale = sm2VFXScale * new Vector3(1f, 1f, 1f);
            waveVFX.transform.localScale = sm2VFXScale * new Vector3(1f, 1f, 1f);



            // Detect all enemy in range
            Collider[] enemies = Physics.OverlapSphere(transform.position, sm2Radius, enemyLayer);
            foreach (Collider enemy in enemies)
            {
                enemy.GetComponent<EnemyStats>().TakeDamage(sm2Damage);
                Instantiate(sm2EnemyHitVFX, enemy.transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
    }

    void SM3(InputAction.CallbackContext context)
    {

    }

//    void DealDamage()
//     {
//         float radius = attackRange;
//         Vector3 origin = attackPoint.position;

    //         Collider[] enemiesHit = Physics.OverlapSphere(origin, radius, LayerMask.GetMask("Enemy"));

    //         foreach (Collider enemy in enemiesHit)
    //         {
    //             Vector3 toEnemy = enemy.transform.position - transform.position;
    //             if (Vector3.Dot(toEnemy.normalized, transform.forward) > 0)
    //             {
    //                 // Check for different health scripts
    //                 GolemHealth golemHealth = enemy.GetComponent<GolemHealth>();
    //                 TreeHealth treeHealth = enemy.GetComponent<TreeHealth>();
    //                 BatHealth batHealth = enemy.GetComponent<BatHealth>();
    //                 MushroomHealth mushroomHealth = enemy.GetComponent<MushroomHealth>();
    //                 SkeletonHealth skeletonHealth = enemy.GetComponent<SkeletonHealth>();

    //                 if (golemHealth != null) golemHealth.TakeDamage(attackDamage);
    //                 if (treeHealth != null) treeHealth.TakeDamage(attackDamage);
    //                 if (batHealth != null) batHealth.TakeDamage(attackDamage);
    //                 if (mushroomHealth != null) mushroomHealth.TakeDamage(attackDamage);
    //                 if (skeletonHealth != null) skeletonHealth.TakeDamage(attackDamage);

    //                 // Debug.Log("Hit " + enemy.name);
    //             }
    //         }
    //     }


    void ExitAttack()
    {
        if (Time.time - lastClickedTime > exitComboTime && inCombo)
        {
            ExitCombo();
        }
    }

    void ExitCombo()
    {
        // Debug.Log("ExitCombo");
        comboCounter = 0;
        lastComboEnd = Time.time;
        inCombo = false;
    }

    void OnDrawGizmosSelected()
    {
        // if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sm2Radius);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        HP -= damage;
        hpTargetFill = HP / initHP * hpFullFill;

        if (HP <= 0)
        {
            // Player Die
            isDead = true;
            anim.SetLayerWeight(1, 0);
            StartCoroutine(DieCoroutine());
        } else
        {
            anim.SetTrigger("Hurt");
        }
    }

    //THIS IS FOR THE POWER UP
    public void Heal(float amount)
    {
        if (isDead) return;

        HP += amount;
        if (HP > initHP) HP = initHP;

        // Update health bar
        hpTargetFill = HP / initHP * hpFullFill;
    }

    private IEnumerator DieCoroutine()
    {
        anim.SetTrigger("Die");
        if (deathUI != null)
            deathUI.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        Instantiate(deathParticles, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
}
