using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public List<GameObject> enemyPrefabs = new List<GameObject>();
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

    public bool AreaCleared { get; private set; } = false;

    private void Start()
    {
        // 🔹 Register this area with ArenaControl dynamically
        if (ArenaControl.Instance != null)
        {
            ArenaControl.Instance.RegisterArea(this);
        }
    }

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
                    Debug.Log("Player left area — pausing spawn. Progress kept.");
                    isSpawning = false;
                    yield break;
                }

                if (group.enemyPrefabs.Count == 0)
                {
                    Debug.LogWarning("EnemyGroup has no enemy prefabs!");
                    break;
                }

                int index = group.spawnedCount % group.enemyPrefabs.Count;
                GameObject prefab = group.enemyPrefabs[index];

                var enemyGO = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

                if (spawnEffectPrefab != null)
                    Instantiate(spawnEffectPrefab, spawnPoint.position, Quaternion.identity);

                var stats = enemyGO.GetComponent<EnemyStats>();
                if (stats != null)
                {
                    int capturedIndex = g;
                    stats.OnDeath += (e) => NotifyEnemyDeath(capturedIndex);
                }

                group.spawnedCount++;
                Debug.Log($"Spawned enemy {group.spawnedCount}/{group.count} (Group {g})");

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        Debug.Log($"✔ All enemies spawned for area: {gameObject.name}");
        isSpawning = false;
    }

    public void NotifyEnemyDeath(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= enemyGroups.Count) return;

        var group = enemyGroups[groupIndex];
        group.deadCount++;

        if (group.deadCount >= group.count)
        {
            Debug.Log($"🔥 All enemies in group {groupIndex} are dead!");
        }

        bool allGroupsCleared = true;
        foreach (var g in enemyGroups)
        {
            if (g.deadCount < g.count)
            {
                allGroupsCleared = false;
                break;
            }
        }

        if (allGroupsCleared && !AreaCleared)
        {
            AreaCleared = true;
            Debug.Log($"✔ AREA CLEARED: {gameObject.name}");

            // 🔥 Notify ArenaControl
            if (ArenaControl.Instance != null)
                ArenaControl.Instance.NotifyAreaCleared(this);
        }
    }
}
