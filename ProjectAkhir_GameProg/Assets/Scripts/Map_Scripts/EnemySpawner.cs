using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int count = 5;
        public float spawnInterval = 1f;
        public int spawnPointIndex = 0;
    }

    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private bool playerInside = false;
    private bool isSpawning = false;
    private bool hasSpawned = false; // ✅ Prevents re-spawning

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSpawning && !hasSpawned)
        {
            Debug.Log($"Player entered enemy area: {gameObject.name}");
            playerInside = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player left enemy area: {gameObject.name}");
            playerInside = false;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        isSpawning = true;

        foreach (var group in enemyGroups)
        {
            Transform spawnPoint = spawnPoints[group.spawnPointIndex];

            for (int i = 0; i < group.count; i++)
            {
                if (!playerInside)
                {
                    Debug.Log("Player left area — stopping enemy spawn.");
                    isSpawning = false;
                    yield break;
                }

                Instantiate(group.enemyPrefab, spawnPoint.position, Quaternion.identity);
                Debug.Log($"Spawned enemy {i + 1}/{group.count} from {group.enemyPrefab.name}");
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        Debug.Log($"✅ All enemies spawned for area: {gameObject.name}");
        hasSpawned = true;  // ✅ Mark as completed
        isSpawning = false;
    }
}
