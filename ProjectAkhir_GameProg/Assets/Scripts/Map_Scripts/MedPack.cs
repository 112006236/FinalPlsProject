using UnityEngine;

public class MedPack : MonoBehaviour
{
    [Header("Healing Settings")]
    public float healAmount = 25f;

    [Header("Movement Settings")]
    public float attractionRange = 5f;  // How close the player needs to be for the medpack to start moving
    public float moveSpeed = 5f;        // How fast it moves toward the player

    [Header("Particle Effects")]
    public ParticleSystem idleEffect;   // Particle effect while waiting
    public ParticleSystem collectEffect; // Particle effect when collected

    private Transform targetPlayer;

    void Start()
    {
        // Play idle effect if assigned
        if (idleEffect != null)
        {
            idleEffect.Play();
        }
    }

    void Update()
    {
        if (targetPlayer != null)
        {
            // Move towards the player
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);

            // Check if close enough to heal
            if (Vector3.Distance(transform.position, targetPlayer.position) < 0.5f)
            {
                PlayerCombat playerCombat = targetPlayer.GetComponent<PlayerCombat>();
                if (playerCombat != null && !playerCombat.isDead)
                {
                    playerCombat.Heal(healAmount);
                }

                if (collectEffect != null)
                    Instantiate(collectEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
        else
        {
            // Find the player within attraction range
            Collider[] hits = Physics.OverlapSphere(transform.position, attractionRange);
            foreach (var hit in hits)
            {
                PlayerCombat playerCombat = hit.GetComponent<PlayerCombat>();
                if (playerCombat != null && !playerCombat.isDead)
                {
                    targetPlayer = playerCombat.transform;
                    break;
                }
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attractionRange);
    }
}
