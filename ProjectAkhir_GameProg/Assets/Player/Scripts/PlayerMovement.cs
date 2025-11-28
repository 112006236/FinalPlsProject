using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float gravity = -10f;
    public float attackSlowDownFactor = 0.4f;
    public float camHeight;
    public float cameraRadius = 10f;
    public float mouseSensitivity = 0.1f;
    private CharacterController controller;
    private Animator animator;
    public Transform sprites;

    [HideInInspector]
    public Vector3 lastMoveDirection;
    [HideInInspector]
    public bool facingRight;
    private PlayerDash playerDash;

    PlayerInputActions inputActions;

    // Get isDead variable from PlayerCombat
    private PlayerCombat combat;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        combat = GetComponent<PlayerCombat>();
        playerDash = GetComponent<PlayerDash>();

        lastMoveDirection = Vector2.zero;
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!combat.isDead)
        {
            if (!playerDash.isDashing) Movement();
            RotateCam();
        } else
        {
            gravity = 0f;
        }
    }

    private void RotateCam()
    {
        float mouseDeltaX = inputActions.Player.MouseLook.ReadValue<Vector2>().x;
        transform.Rotate(transform.up, mouseDeltaX * mouseSensitivity);
    }

    private void Movement()
    {
        float finalMoveSpeed = moveSpeed;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2"))
        {
            Debug.Log("Slowdown");
            finalMoveSpeed *= attackSlowDownFactor;
        }

        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>().normalized; // 8-direction movement
        Vector3 moveVector = transform.right * inputVector.x + transform.forward * inputVector.y;

        // Update lastMoveDirection for dashing
        if (inputVector.magnitude > 0.1f)
        {
            lastMoveDirection = moveVector;
            // lastMoveDirection.y = 0;
        }
        
        // Gravity
        moveVector.y = gravity;

        controller.Move(finalMoveSpeed * Time.deltaTime * moveVector);

        animator.SetBool("Walking", inputVector.magnitude > 0.1f);

        // Handle flipping sprite
        if (inputVector.x != 0 && inputVector.x > 0.1f && !combat.inCombo)
        {
            facingRight = true;
            sprites.localScale = new Vector3(1, 1, 1);
        }
        else if (inputVector.x != 0 && inputVector.x < -0.1f && !combat.inCombo)
        {
            facingRight = false;
            sprites.localScale = new Vector3(-1, 1, 1);
        }
    }
}
