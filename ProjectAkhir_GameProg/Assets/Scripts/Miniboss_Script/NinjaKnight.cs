using System.Collections;
using UnityEngine;

public class NinjaKnight : MonoBehaviour
{
    public Transform player;
    public Animator anim;
    private SpriteRenderer sr;

    [Header("Movement")]
    public float moveSpeed = 50f;
    public float dashSpeed = 70f;
    public float chaseRange = 55f;
    public float attackRange = 1.2f;      // Normal attack range
    public float dashAttackRange = 4f;    // Dash attack range
    public float normalAttackDamage=0.5f;
    public float dashAttackDamage=3f;

    [Header("Combat")]
    public float attackCooldown = 0.3f;       // Short cooldown for combo
    public float dashAttackCooldown = 0.7f;   // Longer cooldown for dash
    private bool canAttack = true;
    private bool isDashing = false;
    private bool canNormalAttack = true;  // For normal attack combo
    private bool canDashAttack = true;    // For dash attack

    private int comboStep = 0;                 // Track normal attack combo

    private string currentAnimation = "";

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("No GameObject with tag 'Player' found!");
        }
    }

    private void Update()
    {
        if (!player) return;

        FlipSprite();

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isDashing && distance <= chaseRange)
        {
            ChasePlayer(distance);
        }
    }

    void ChasePlayer(float distance)
    {
        Vector3 direction = (player.position - transform.position).normalized;

        // // Smoothly rotate to face player
        // if (direction != Vector3.zero)
        // {
        //     Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
        // }

        if (distance > attackRange && distance > dashAttackRange)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            PlayAnimOnce("run");
            Debug.Log("tessssss");
        }
        else
        {
            // Idle if inside attack range but on cooldown
            //PlayAnimOnce("idle");
            Debug.Log("masuk");
            // Normal attack
            if (distance <= attackRange && canNormalAttack)
                StartCoroutine(NormalAttackCombo());

            // Dash attack
            else if (distance <= dashAttackRange && distance > attackRange && canDashAttack){
                Debug.Log("tes");
                StartCoroutine(DashAttack(direction));
            }
        }
    }

    IEnumerator DashAttack(Vector3 direction)
    {
        canDashAttack = false;
        isDashing = true;

        float dashTime = 0.5f;
        float timer = 0f;

        while (timer < dashTime)
        {
            transform.position += direction * dashSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
            }

            PlayAnimOnce("run");
            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        // Attack2 + fall
        PlayAnimOnce("attack2");
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            yield return new WaitForSeconds(0.45f);
            PlayerCombat pc = player.GetComponent<PlayerCombat>();
            if (pc != null && !pc.isDead)
                pc.TakeDamage(dashAttackDamage);
        }
        yield return new WaitForSeconds(0.5f);
        PlayAnimOnce("fall");
        yield return new WaitForSeconds(0.3f);
        PlayAnimOnce("idle");

        // Dash cooldown
        yield return new WaitForSeconds(dashAttackCooldown - 0.8f); // remaining time
        canDashAttack = true;
    }




    IEnumerator NormalAttackCombo()
    {
        canNormalAttack = false;

        if (comboStep % 2 == 0){
            PlayAnimOnce("attack1");
            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                yield return new WaitForSeconds(0.45f);
                PlayerCombat pc = player.GetComponent<PlayerCombat>();
                if (pc != null && !pc.isDead)
                    pc.TakeDamage(normalAttackDamage);
            }
        }
        else{
            PlayAnimOnce("attack2");
            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                yield return new WaitForSeconds(0.45f);
                PlayerCombat pc = player.GetComponent<PlayerCombat>();
                if (pc != null && !pc.isDead)
                    pc.TakeDamage(normalAttackDamage);
            }
        }

        comboStep++;
        yield return new WaitForSeconds(attackCooldown);

        canNormalAttack = true;
    }


    void PlayAnimOnce(string animName)
    {
        if (currentAnimation != animName)
        {
            anim.Play(animName);
            currentAnimation = animName;
        }
    }

    private void FlipSprite()
    {
        if (player == null) return;

        // Check if player is to the left of the NinjaKnight
        bool isPlayerLeft = player.position.x < transform.position.x;

        // Flip sprite to face left if player is left
        sr.flipX = isPlayerLeft;
    }

}
