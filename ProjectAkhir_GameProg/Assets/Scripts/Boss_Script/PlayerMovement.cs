using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;          // Movement speed
    public float rotationSpeed = 720f;    // Degrees per second for rotation
    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get input from WASD / Arrow keys
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Direction relative to world space
        moveInput = new Vector3(moveX, 0, moveZ).normalized;

        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.MovePosition(rb.position + moveVelocity*Time.deltaTime);

    }

    void FixedUpdate()
    {
        // Move the player using Rigidbody
        
    }
}
