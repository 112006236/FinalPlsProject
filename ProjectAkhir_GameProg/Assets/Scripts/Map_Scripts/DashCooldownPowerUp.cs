using UnityEngine;
using System.Collections;

public class DashCooldownPowerUp : MonoBehaviour
{
    [Header("Dash Cooldown Settings")]
    public float cooldownReduction = 0.3f;  // Amount to reduce dash cooldown
    public float buffDuration = 5f;         // Duration of the temporary buff in seconds

    [Header("Movement Settings")]
    public float attractionRange = 5f;
    public float moveSpeed = 5f;

    [Header("Particle Effects")]
    public ParticleSystem idleEffect;
    public ParticleSystem collectEffect;

    private Transform targetPlayer;

    void Start()
    {
        if (idleEffect != null)
            idleEffect.Play();
    }

    void Update()
    {
        if (targetPlayer != null)
        {
            // Move toward the player
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // When close enough: apply cooldown buff
            if (Vector3.Distance(transform.position, targetPlayer.position) < 0.5f)
            {
                PlayerDash dash = targetPlayer.GetComponent<PlayerDash>();
                if (dash != null)
                {
                    StartCoroutine(ApplyTemporaryCooldown(dash));
                }

                if (collectEffect != null)
                    Instantiate(collectEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
        else
        {
            // Find player within attraction range
            Collider[] hits = Physics.OverlapSphere(transform.position, attractionRange);
            foreach (var hit in hits)
            {
                PlayerDash dash = hit.GetComponent<PlayerDash>();
                if (dash != null)
                {
                    targetPlayer = dash.transform;
                    break;
                }
            }
        }
    }

    private IEnumerator ApplyTemporaryCooldown(PlayerDash dash)
    {
        float originalCooldown = dash.dashCooldown;
        dash.dashCooldown = Mathf.Max(0.1f, dash.dashCooldown - cooldownReduction);

        yield return new WaitForSeconds(buffDuration);

        // Restore original cooldown
        dash.dashCooldown = originalCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attractionRange);
    }
}
