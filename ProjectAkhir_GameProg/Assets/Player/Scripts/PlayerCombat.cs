using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Stats")]
    public float initHP = 100.0f;
    [System.NonSerialized] public float HP;
    [System.NonSerialized] public bool isDead;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private Transform healthBarFill;
    private float hpTargetAngle;
    private float smoothTime = 0.25f;
    private float currVelocity;

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
    [SerializeField] private GameObject sm1Projectile;
    [SerializeField] private GameObject sm1ProjectileSpawnEffect;
    [SerializeField] private float radiusFromPlayer = 1f;
    [SerializeField] private int projectilePerWave = 6;
    [SerializeField] private int waveCount = 2;
    [SerializeField] private float waveDuration = 0.5f;
    [SerializeField] private float waveDetaTime = 0.8f;
    [SerializeField] private float waveDeltaAngle = 15f;
    

    [Header("Animation")]
    public Animator anim;
    public List<ParticleSystem> slashVFXs;
    public float slashVFXScale = 2.0f;
    public Transform attackPoint;       // Empty object in front of the sword
    [SerializeField] private Collider swordCollider;

    

    PlayerInputActions inputs;
    [HideInInspector] public bool inCombo;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        hpTargetAngle = 0;

        inputs = new PlayerInputActions();
        inputs.Player.Enable();
        inputs.Player.Attack.performed += Attack;
        inputs.Player.SM1.performed += SM1;

        inCombo = false;

        HP = initHP;
        isDead = false;
    }

    void Update()
    {
        ExitAttack();
        // Optional: Flip facing based on input

        swordCollider.enabled = inCombo;

        // Update health bar fill        
        float hpZRot = Mathf.SmoothDampAngle(healthBarFill.eulerAngles.z, hpTargetAngle, ref currVelocity, smoothTime);
        healthBarFill.rotation = Quaternion.Euler(0, 0, hpZRot);

        // Debug purposes only! 
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(40.0f);
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
        StartCoroutine(PerformSM1());
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
            Vector3 spawnPoint = transform.position + radiusFromPlayer * forwards[i];

            Instantiate(sm1ProjectileSpawnEffect, spawnPoint, Quaternion.Euler(0, 0, 0));

            GameObject proj = Instantiate(sm1Projectile, spawnPoint, Quaternion.Euler(0, 0, 0));
            proj.transform.up = forwards[i];
            // Add delay between projectiles
            yield return new WaitForSeconds(waveDuration / projectilePerWave);
        }   
    }

    void SM2(InputAction.CallbackContext context)
    {

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
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        HP -= damage;
        hpTargetAngle = (1 - HP / initHP) * 156.0f;

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
        if (isDead) return;               // don't heal dead players (change if you want revive behavior)
        HP += amount;
        if (HP > initHP) HP = initHP;     // clamp

        // update the health-bar target rotation (keeps the UI consistent)
        hpTargetAngle = (1 - HP / initHP) * 156.0f;

    }

    private IEnumerator DieCoroutine()
    {
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(2.0f);
        Instantiate(deathParticles, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
}
