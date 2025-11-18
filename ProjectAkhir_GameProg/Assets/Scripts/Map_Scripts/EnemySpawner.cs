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

        [HideInInspector] public int spawnedCount = 0;
        [HideInInspector] public int deadCount = 0;

        public GameObject minibossPrefab;
        [HideInInspector] public bool minibossSpawned = false;
    }

    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

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

            int spIndex = Mathf.Clamp(group.spawnPointIndex, 0, Mathf.Max(0, spawnPoints.Count - 1));
            Transform spawnPoint = spawnPoints.Count > 0 ? spawnPoints[spIndex] : transform;

            while (group.spawnedCount < group.count)
            {
                if (!playerInside)
                {
                    Debug.Log("Player left area — pausing enemy spawn. Progress preserved.");
                    isSpawning = false;
                    yield break;
                }

                var instance = Instantiate(group.enemyPrefab, spawnPoint.position, Quaternion.identity);

                var notifier = instance.GetComponent<EnemyDeathNotifier>();
                if (notifier != null)
                {
                    notifier.SetSpawner(this, g);
                }

                group.spawnedCount++;
                Debug.Log($"Spawned enemy {group.spawnedCount}/{group.count} from {group.enemyPrefab.name} (group {g})");

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        Debug.Log($"✅ All enemies spawned for area: {gameObject.name}");
        isSpawning = false;
    }

    public void NotifyEnemyDeath(int groupIndex, GameObject enemyGo = null)
    {
        if (groupIndex < 0 || groupIndex >= enemyGroups.Count) return;

        var group = enemyGroups[groupIndex];

        group.deadCount++;
        Debug.Log($"Enemy from group {groupIndex} died — {group.deadCount}/{group.count} dead.");

        // Spawn miniboss ONLY when all enemies are dead
        if (group.deadCount >= group.count &&
            !group.minibossSpawned &&
            group.minibossPrefab != null)
        {
            Transform spawnPoint = spawnPoints.Count > 0 ? spawnPoints[group.spawnPointIndex] : transform;

            Instantiate(group.minibossPrefab, spawnPoint.position, Quaternion.identity);
            group.minibossSpawned = true;

            Debug.Log($"🔥 All enemies from group {groupIndex} are dead — MINIBOSS SPAWNED!");
        }
    }
}
