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
    public bool isDashing = false;
    public bool canNormalAttack = true;  // For normal attack combo
    public bool canDashAttack = true;    // For dash attack

    private int comboStep = 0;                 // Track normal attack combo

    private string currentAnimation = "";
    private float dashtolerance=3.2f;

    [Header("Spawn / Entry")]
    [SerializeField] private GameObject spawnCircleEffect;
    [SerializeField] private float spawnCircleRadius = 3f;
    [SerializeField] private float entryRiseHeight = 3f;
    [SerializeField] private float entryDuration = 1.0f;
    [SerializeField] private float spawnVFXDuration = 1.2f;

    private bool hasEntered = false;


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
        StartCoroutine(EntrySequence());
    }

    private IEnumerator EntrySequence()
    {
        Vector3 groundPos = new Vector3(transform.position.x, 0f, transform.position.z);

        // Start underground
        transform.position = groundPos - Vector3.up * entryRiseHeight;

        // Lock behavior
        isDashing = true;
        canNormalAttack = false;
        canDashAttack = false;

        PlayAnimOnce("idle");

        // Spawn circle FIRST
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

        // Rise up
        float t = 0f;
        while (t < entryDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(
                groundPos - Vector3.up * entryRiseHeight,
                groundPos,
                t / entryDuration
            );
            yield return null;
        }

        transform.position = groundPos;

        if (circle != null)
            Destroy(circle, spawnVFXDuration);

        // Unlock behavior
        isDashing = false;
        canNormalAttack = true;
        canDashAttack = true;
        hasEntered = true;
    }


    private void Update()
    {
        if (!hasEntered) return;

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

        if ((distance > attackRange && distance<=dashtolerance)||distance>dashAttackRange)
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
            else if (distance <= dashAttackRange && distance > attackRange && distance>dashtolerance && canDashAttack){
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
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            yield return new WaitForSeconds(0.45f);
            PlayerCombat pc = player.GetComponent<PlayerCombat>();
            if (pc != null && !pc.isDead && Vector3.Distance(transform.position, player.position) <= attackRange)
                pc.TakeDamage(dashAttackDamage);
        }
        yield return new WaitForSeconds(0.4f);
        PlayAnimOnce("fall");
        yield return new WaitForSeconds(0.3f);
        PlayAnimOnce("idle");

        // Dash cooldown
        yield return new WaitForSeconds(dashAttackCooldown - 0.7f); // remaining time
        canDashAttack = true;
    }




    IEnumerator NormalAttackCombo()
    {
        canNormalAttack = false;

        if (comboStep % 2 == 0){
            PlayAnimOnce("attack1");
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                yield return new WaitForSeconds(0.45f);
                PlayerCombat pc = player.GetComponent<PlayerCombat>();
                if (pc != null && !pc.isDead && Vector3.Distance(transform.position, player.position) <= attackRange)
                    pc.TakeDamage(normalAttackDamage);
            }
        }
        else{
            PlayAnimOnce("attack2");
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                yield return new WaitForSeconds(0.45f);
                PlayerCombat pc = player.GetComponent<PlayerCombat>();
                if (pc != null && !pc.isDead && Vector3.Distance(transform.position, player.position) <= attackRange)
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
