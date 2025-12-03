using UnityEngine;
using System.Collections;

public class SpeedBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    public float speedIncrease = 5f;      // How much speed to add
    public float duration = 5f;           // How long the boost lasts

    [Header("Pickup Settings")]
    public float pickupRange = 2f;        // Radius to detect the player
    public float moveSpeed = 8f;          // Speed the power-up moves toward the player

    private Transform targetPlayer;

    void Update()
    {
        if (targetPlayer != null)
        {
            // Move toward player
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPlayer.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            // Look for player within pickup range
            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
            foreach (var hit in hits)
            {
                PlayerMovement movement = hit.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    targetPlayer = movement.transform;
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            StartCoroutine(ApplySpeedBoost(movement));
            Destroy(gameObject); // Remove the pickup object
        }
    }

    private IEnumerator ApplySpeedBoost(PlayerMovement movement)
    {
        float originalSpeed = movement.moveSpeed;

        movement.moveSpeed += speedIncrease; // Apply the buff

        yield return new WaitForSeconds(duration);

        movement.moveSpeed = originalSpeed;  // Reset back to normal
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
