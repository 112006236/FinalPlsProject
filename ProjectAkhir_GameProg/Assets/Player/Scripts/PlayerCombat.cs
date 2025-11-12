using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public float initHP = 100.0f;
    [System.NonSerialized] public float HP;
    [System.NonSerialized] public bool isDead;
    [SerializeField] private GameObject deathParticles;

    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    public float timeBetweenCombo = 0.5f;
    public float timeBetweenAttacks = 0.2f;
    public float exitComboTime = 0.5f;

    public Animator anim;
    public Transform attackPoint;       // Empty object in front of the sword
    [SerializeField] private Collider swordCollider;
    public float attackRange = 1.5f;    // How far the slash reaches
    public float attackDamage = 25f;    // Damage per hit

    PlayerInputActions inputs;
    private bool inCombo;
    private bool facingRight = true; // Track which way the player is facing

    void Start()
    {
        inputs = new PlayerInputActions();
        inputs.Player.Enable();
        inputs.Player.Attack.performed += Attack;

        inCombo = false;

        HP = initHP;
        isDead = false;
    }

    void Update()
    {
        ExitAttack();
        // Optional: Flip facing based on input

        swordCollider.enabled = inCombo;

        // Debug purposes only! 
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(40.0f);
        }

        
    }

    void Attack(InputAction.CallbackContext context)
    {
        inCombo = true;
        CancelInvoke("ExitCombo");

        if (Time.time - lastClickedTime >= timeBetweenAttacks)
        {
            // Debug.Log("Attack Pattern " + comboCounter);

            // Play the correct attack animation
            anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
            anim.Play("Attack", 0, 0);

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

    void DealDamage()
    {
        float radius = attackRange;
        Vector3 origin = attackPoint.position;

        Collider[] enemiesHit = Physics.OverlapSphere(origin, radius, LayerMask.GetMask("Enemy"));

        foreach (Collider enemy in enemiesHit)
        {
            Vector3 toEnemy = enemy.transform.position - transform.position;
            if (Vector3.Dot(toEnemy.normalized, transform.forward) > 0)
            {
                // Check for different health scripts
                GolemHealth golemHealth = enemy.GetComponent<GolemHealth>();
                TreeHealth treeHealth = enemy.GetComponent<TreeHealth>();
                BatHealth batHealth = enemy.GetComponent<BatHealth>();
                MushroomHealth mushroomHealth = enemy.GetComponent<MushroomHealth>();
                SkeletonHealth skeletonHealth = enemy.GetComponent<SkeletonHealth>();

                if (golemHealth != null) golemHealth.TakeDamage(attackDamage);
                if (treeHealth != null) treeHealth.TakeDamage(attackDamage);
                if (batHealth != null) batHealth.TakeDamage(attackDamage);
                if (mushroomHealth != null) mushroomHealth.TakeDamage(attackDamage);
                if (skeletonHealth != null) skeletonHealth.TakeDamage(attackDamage);

                // Debug.Log("Hit " + enemy.name);
            }
        }
    }


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
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;

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

    

    private IEnumerator DieCoroutine()
    {
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(2.0f);
        Instantiate(deathParticles, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
}
