using UnityEngine;
using System.Collections;

public class SpikeDamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 5f;

    private void OnTriggerEnter(Collider other)
    {
        // Start damage coroutine for this object
        StartCoroutine(DoDamage(other));
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop all coroutines affecting exiting object
        StopAllCoroutines();
    }

    private IEnumerator DoDamage(Collider target)
    {
        while (true)
        {
            // Damage Player
            PlayerCombat player = target.GetComponentInParent<PlayerCombat>();
            if (player != null)
            {
                player.TakeDamage(damagePerSecond);
            }

            // Damage any enemy using EnemyStats
            EnemyStats enemy = target.GetComponentInParent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerSecond);
            }

            yield return new WaitForSeconds(1f); // damage per second
        }
    }
}
