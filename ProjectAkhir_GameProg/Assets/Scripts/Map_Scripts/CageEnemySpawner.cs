using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageEnemySpawner : MonoBehaviour
{
    [Header("Cage Settings")]
    public MultiHealthObject cage;      
    public bool stopWhenCageDestroyed = true;

    [Header("Spawn Settings")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();     // Multiple enemies
    public List<Transform> spawnPoints = new List<Transform>();        // Multiple spawn points

    public float spawnInterval = 2f;
    public int maxAliveEnemies = 5;

    [Header("Miniboss Settings")]
    public GameObject miniBossPrefab;
    public Transform miniBossSpawnPoint;
    public float miniBossDelay = 160f;

    private bool miniBossTimerStarted = false;

    [Header("Trigger Settings")]
    public Collider triggerArea;    // Set a BoxCollider (IsTrigger = true)

    private bool playerInside = false;
    private bool isSpawning = false;

    private void Start()
    {
        if (cage == null)
        {
            Debug.LogError("CageEnemySpawner: Cage reference not assigned!");
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSpawning)
        {
            Debug.Log("KONTOOLLLLL");
            playerInside = true;
            if (!miniBossTimerStarted)
            {
                miniBossTimerStarted = true;
                StartCoroutine(SpawnMiniBoss());
            }
            StartCoroutine(SpawnLoop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private IEnumerator SpawnLoop()
    {
        isSpawning = true;

        while (true)
        {
            // --- STOP WHEN CAGE IS DESTROYED ---
            if (stopWhenCageDestroyed && cage.GetCurrentHealth() <= 0)
            {
                Debug.Log("Cage destroyed — stopping enemy spawn.");
                yield break;
            }

            // --- PAUSE WHEN PLAYER LEAVES ---
            if (!playerInside)
            {
                Debug.Log("Player left area — pausing spawn.");
                isSpawning = false;
                yield break;
            }

            // Spawn only if under limit
            int alive = CountAliveEnemies();

            if (alive < maxAliveEnemies)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0)
            return;

        // Pick a random enemy
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        // Pick a random spawn point
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

        Instantiate(prefab, point.position, point.rotation);
    }

    private int CountAliveEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    private IEnumerator SpawnMiniBoss()
    {
        yield return new WaitForSeconds(miniBossDelay);

        Instantiate(miniBossPrefab,
            miniBossSpawnPoint.position,
            miniBossSpawnPoint.rotation);
    }
}