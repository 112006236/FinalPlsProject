using System.Collections;
using UnityEngine;

public class BossMeteorAttack : MonoBehaviour
{
    [Header("Meteor Settings")]
    public GameObject meteorPrefab;
    public int meteorCount = 5;           // Number of meteors to spawn
    public float spawnRadius = 10f;       // Around player
    public float spawnHeight = 25f;       // How high meteors spawn above ground
    public float spawnInterval = 0.2f;    // Time between each meteor spawn

    [Header("References")]
    public Transform player;              // Assign in Inspector or dynamically

    // Called by animation event
    public void TriggerMeteorAttack()
    {
        if (meteorPrefab == null || player == null)
        {
            Debug.LogWarning("MeteorAttack: Missing meteorPrefab or player reference!");
            return;
        }

        StartCoroutine(SpawnMeteors());
    }

    private IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // Random position near the player
            Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(
                player.position.x + randomOffset.x,
                player.position.y + spawnHeight,
                player.position.z + randomOffset.y
            );

            // Instantiate meteor
            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
