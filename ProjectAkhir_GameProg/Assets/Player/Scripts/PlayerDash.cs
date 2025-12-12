using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    // public Rigidbody rb;
    public PlayerMovement playerMovement; // Link to movement 

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool canDash = true;

    private CharacterController controller;
    private float lastDashTime;
    [HideInInspector]
    public bool isDashing;

    [SerializeField] private Animator anim;
    [SerializeField] private Transform sprites;
    [SerializeField] private ParticleSystem cooldownVFX;
    private bool playedCooldownVFX;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        anim.SetLayerWeight(2, 0);
        anim.SetBool("Dashing", false);

        lastDashTime = Time.time;
        isDashing = false;
        playedCooldownVFX = false;
    }

    private void Update()
    {
        if (Time.time - lastDashTime > dashCooldown && !playedCooldownVFX)
        {
            cooldownVFX.Play();
            playedCooldownVFX = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash && Time.time - lastDashTime > dashCooldown)
        {
            lastDashTime = Time.time;
            isDashing = true;
        }

        if (isDashing)
        {
            sprites.localScale = new Vector3(
                1.5f * (playerMovement.facingRight ? 1 : -1),
                .5f,
                1
            );

            controller.Move(dashForce * Time.deltaTime * playerMovement.lastMoveDirection);
            if (Time.time - lastDashTime > dashDuration)
            {
                isDashing = false;
                lastDashTime = Time.time; // To perform correct calculation of cooldown
                playedCooldownVFX = false;

                sprites.localScale = new Vector3(
                    (playerMovement.facingRight ? 1 : -1),
                    1,
                    1
                );
            }
        }
    }
}
