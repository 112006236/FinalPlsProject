using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public List<GameObject> enemyPrefabs = new List<GameObject>(); // 🔥 multiple enemies
        public int count = 5;
        public float spawnInterval = 1f;
        public int spawnPointIndex = 0;

        [HideInInspector] public int spawnedCount = 0;
        [HideInInspector] public int deadCount = 0;
    }

    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Effects")]
    [SerializeField] private GameObject spawnEffectPrefab;

    private bool playerInside = false;
    private bool isSpawning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSpawning)
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

        for (int g = 0; g < enemyGroups.Count; g++)
        {
            EnemyGroup group = enemyGroups[g];

            // Safe spawnpoint
            int spIndex = Mathf.Clamp(group.spawnPointIndex, 0, Mathf.Max(0, spawnPoints.Count - 1));
            Transform spawnPoint = spawnPoints.Count > 0 ? spawnPoints[spIndex] : transform;

            while (group.spawnedCount < group.count)
            {
                if (!playerInside)
                {
                    Debug.Log("Player left area — pausing spawn. Progress kept.");
                    isSpawning = false;
                    yield break;
                }

                if (group.enemyPrefabs.Count == 0)
                {
                    Debug.LogWarning("EnemyGroup has no enemy prefabs!");
                    break;
                }

                // Choose enemy (sequential)
                int index = group.spawnedCount % group.enemyPrefabs.Count;
                GameObject prefab = group.enemyPrefabs[index];

                // Spawn enemy
                var enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

                // Spawn effect
                if (spawnEffectPrefab != null)
                    Instantiate(spawnEffectPrefab, spawnPoint.position, Quaternion.identity);

                // Death notifier support
                var notifier = enemy.GetComponent<EnemyDeathNotifier>();
                if (notifier != null)
                    notifier.SetSpawner(this, g);

                group.spawnedCount++;
                Debug.Log($"Spawned enemy {group.spawnedCount}/{group.count} (Group {g})");

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        Debug.Log($"✔ All enemies spawned for area: {gameObject.name}");
        isSpawning = false;
    }

    public void NotifyEnemyDeath(int groupIndex, GameObject enemy = null)
    {
        if (groupIndex < 0 || groupIndex >= enemyGroups.Count) return;

        var group = enemyGroups[groupIndex];
        group.deadCount++;

        Debug.Log($"Enemy died from group {groupIndex} — {group.deadCount}/{group.count} dead.");
    }
}
