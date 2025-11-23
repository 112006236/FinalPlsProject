using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Sprite Flipping")]
    public Transform spriteTransform; // Assign the Imp sprite or graphics object here
    public bool flipInsteadOfRotate = true; // If true: flip scale. If false: rotate Y.

    [Header("Particle Effect")]
    public ParticleSystem attackEffect; // Particle effect to play when attacking

    private Transform player;
    private PlayerCombat playerCombat;
    private float lastAttackTime;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerCombat = playerObj.GetComponent<PlayerCombat>();
        }

        if (spriteTransform == null)
        {
            spriteTransform = transform; // fallback to this object
        }
    }

    void Update()
    {
        if (player == null || playerCombat == null || playerCombat.isDead)
            return;

        HandleSpriteFlip();

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
        }
    }

    void HandleSpriteFlip()
    {
        if (player == null) return;

        // If the player is to the RIGHT of enemy
        if (player.position.x > transform.position.x)
        {
            if (flipInsteadOfRotate)
                spriteTransform.localScale = new Vector3(1, 1, 1);
            else
                spriteTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            if (flipInsteadOfRotate)
                spriteTransform.localScale = new Vector3(-1, 1, 1);
            else
                spriteTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void Attack()
    {
        // Deal damage to the player
        playerCombat.TakeDamage(damage);

        // Play particle effect if assigned
        if (attackEffect != null)
        {
            // Scale up the particle effect
            attackEffect.transform.localScale = new Vector3(2f, 2f, 2f); // Adjust these numbers as needed
            attackEffect.Play();
            Debug.Log("FIREE");
        }

        lastAttackTime = Time.time;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
