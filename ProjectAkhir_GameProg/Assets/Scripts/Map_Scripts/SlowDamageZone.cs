using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 5f;

    // Keep track of objects currently being damaged
    private Dictionary<Collider, Coroutine> damageCoroutines = new Dictionary<Collider, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        // If we are already damaging this object, skip
        if (damageCoroutines.ContainsKey(other))
            return;

        // Start a new coroutine for this object
        Coroutine coroutine = StartCoroutine(DoDamage(other));
        damageCoroutines.Add(other, coroutine);
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop the coroutine for this specific object
        if (damageCoroutines.TryGetValue(other, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            damageCoroutines.Remove(other);
        }
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
