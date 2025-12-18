using System.Collections;
using UnityEngine;

public class BossMeteorAttack : MonoBehaviour
{
    [Header("Meteor Settings")]
    public GameObject meteorPrefab;
    public GameObject warningMarkerPrefab;
    public int meteorCount = 5;
    public float spawnRadius = 10f;
    public float spawnHeight = 20f;
    public float spawnInterval = 0f;
    public float warningDelay = 0.05f; // Time between marker and meteor impact

    [Header("References")]
    public Transform player;

    public void TriggerMeteorAttack()
    {
        StartCoroutine(SpawnMeteors());
    }

    private IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // Pick random position near player
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 groundPos = new Vector3(
                player.position.x + offset.x,
                player.position.y - 1,
                player.position.z + offset.y
            );

            if (i % 8 == 0)
            {
                groundPos = new Vector3(
                player.position.x,
                player.position.y - 1,
                player.position.z
            );
            }

            groundPos.y += 1;

            // Spawn warning marker
            GameObject marker = Instantiate(warningMarkerPrefab, groundPos, Quaternion.identity);

            // Wait before dropping meteor
            yield return new WaitForSeconds(warningDelay);

            // Spawn meteor above the marker
            Vector3 spawnPos = groundPos + Vector3.up * spawnHeight;
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            // Let the meteor know its marker (so it can destroy it on impact)
            Meteor meteorScript = meteor.GetComponent<Meteor>();
            if (meteorScript != null)
            {
                meteorScript.warningMarker = marker;
                // --- THE 30% CHANCE LOGIC ---
                // Random.value returns 0.0 to 1.0. 0.3f = 30%
                if (Random.value <= 0.3f) 
                {
                    meteorScript.isScatter = true;
                    // Optional: Change the color or scale of cluster meteors 
                    // so the player can tell the difference!
                    meteor.GetComponent<Renderer>().material.color = Color.red; 
                }
                else
                {
                    meteorScript.isScatter = false;
                }
            }
                

            spawnInterval = Random.Range(0f, 0.5f);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
