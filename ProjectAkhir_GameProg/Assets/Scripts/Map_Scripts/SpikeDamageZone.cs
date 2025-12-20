using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeDamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 5f;

    // Track active coroutines per target
    private Dictionary<Collider, Coroutine> activeCoroutines = new Dictionary<Collider, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (!activeCoroutines.ContainsKey(other))
        {
            Debug.Log("Spike touched: " + other.name);

            Coroutine c = StartCoroutine(DoDamage(other));
            activeCoroutines.Add(other, c);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (activeCoroutines.TryGetValue(other, out Coroutine c))
        {
            StopCoroutine(c);
            activeCoroutines.Remove(other);
        }
    }

    private IEnumerator DoDamage(Collider target)
    {
        PlayerCombat player = target.GetComponentInParent<PlayerCombat>();
        EnemyStats enemy = target.GetComponentInParent<EnemyStats>();

        if (player == null && enemy == null)
        {
            Debug.Log("Spike touched non-damageable object: " + target.name);
            yield break;
        }

        while (target != null)
        {
            if (player != null)
                player.TakeDamage(damagePerSecond);

            if (enemy != null)
                enemy.TakeDamage(damagePerSecond);

            yield return new WaitForSeconds(1f);
        }
    }


}
