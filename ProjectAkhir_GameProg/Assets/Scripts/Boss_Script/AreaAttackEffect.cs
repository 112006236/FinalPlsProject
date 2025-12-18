using UnityEngine;

public class AreaAttackEffect : MonoBehaviour
{
    public float damageAmount = 20f;
    public LayerMask playerLayer;

    // This matches your 20x, 0.05y, 20z prefab dimensions
    // We use current scale so the "hitbox" matches the "visual"
    public void TriggerDamage()
    {
            // Make the hitbox taller (e.g., 6 units high) to ensure it hits 
        // the middle of the Character Controller capsule
        Vector3 boxSize = new Vector3(transform.localScale.x, 6f, transform.localScale.z);
        
        // Position the box slightly above the ground (since the circle is at y - 0.7)
        Vector3 boxCenter = transform.position + Vector3.up * 2f;

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, transform.rotation, playerLayer);

        Debug.Log($"Scanning... Found {hitColliders.Length} colliders.");

        foreach (var hit in hitColliders)
        {
            Debug.Log("Hit detected on: " + hit.name);
            
            // CharacterController counts as a collider, so this should work
            if (hit.TryGetComponent(out PlayerCombat combat))
            {
                combat.TakeDamage(damageAmount);
                Debug.Log("Damage Applied to Player!");
            }
            else
            {
                // If the script is on a parent/child, check there too
                PlayerCombat parentCombat = hit.GetComponentInParent<PlayerCombat>();
                if (parentCombat != null)
                {
                    parentCombat.TakeDamage(damageAmount);
                    Debug.Log("Damage Applied to Player via Parent!");
                }
            }
        }
    }

    // This helps you see the hitbox in the Scene View while testing
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); // Transparent red
        Vector3 gizmoScale = transform.localScale;
        gizmoScale.y = 2f; 
        Gizmos.DrawCube(transform.position, gizmoScale);
    }
}