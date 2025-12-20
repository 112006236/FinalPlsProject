using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineEnemySpawner : MonoBehaviour
{
    [Header("Shrine Reference")]
    public CaptureShrine shrine;

    [Header("Spawn Settings")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnInterval = 2f;
    public int maxAliveEnemies = 5;

    [Header("Miniboss Settings")]
    public GameObject miniBossPrefab;
    public Transform miniBossSpawnPoint;
    public float miniBossDelay = 25f;

    private bool miniBossTimerStarted = false;

    private bool isSpawning = false;

    private void Start()
    {
        Debug.Log("start shrine spawner");
        if (shrine == null)
        {
            Debug.LogError("ShrineEnemySpawner: Shrine reference missing!");
            enabled = false;
            return;
        }

        // Stop spawner completely when shrine is captured
        shrine.OnShrineCaptured += StopSpawner;
    }

    private void OnDestroy()
    {
        if (shrine != null)
            shrine.OnShrineCaptured -= StopSpawner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSpawning)
        {
            if (!miniBossTimerStarted)
            {
                Debug.Log("start timer");
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
            isSpawning = false;
        }
    }

    private IEnumerator SpawnLoop()
    {
        isSpawning = true;

        while (true)
        {
            // Stop completely if shrine captured
            if (shrine.IsCaptured())
            {
                Debug.Log("Shrine captured — stopping spawner.");
                yield break;
            }

            // Pause spawning if player leaves
            if (!shrine.IsPlayerInside())
            {
                isSpawning = false;
                Debug.Log("Player left shrine area — pausing spawn.");
                yield break;
            }

            // Spawn new enemies if under max limit
            int aliveEnemies = CountAliveEnemies();
            if (aliveEnemies < maxAliveEnemies)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void StopSpawner()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

        Instantiate(prefab, point.position, point.rotation);
    }

    private IEnumerator SpawnMiniBoss()
    {
        yield return new WaitForSeconds(miniBossDelay);
        Debug.Log("start miniboss");

        Instantiate(miniBossPrefab,
            miniBossSpawnPoint.position,
            miniBossSpawnPoint.rotation);
    }

    private int CountAliveEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
