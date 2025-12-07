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
        // Cache components once
        PlayerCombat player = target.GetComponentInParent<PlayerCombat>();
        //why the hell does enemy doesnt get damagede
        EnemyStats enemy = target.GetComponent<EnemyStats>();
        if (enemy == null)
            enemy = target.GetComponentInParent<EnemyStats>();
        if (enemy == null)
            enemy = target.GetComponentInChildren<EnemyStats>();

        while (true)
        {
            if (target == null || (!player && !enemy)) // target destroyed or components gone
                yield break;

            if (player != null)
                player.TakeDamage(damagePerSecond);

            if (enemy != null || target.CompareTag("Enemy"))
                enemy.TakeDamage(damagePerSecond);

            yield return new WaitForSeconds(1f);
        }
    }
}
