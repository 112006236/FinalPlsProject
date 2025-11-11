using System.Collections.Generic;
using UnityEngine;

public class ArenaGridManager : MonoBehaviour
{
    [Header("Arena Ground")]
    [SerializeField] private Transform ground; // The ground/plane
    [SerializeField] private float cellSize = 10f; // Distance between each spawn

    [Header("Enemy Area Prefabs")]
    [SerializeField] private GameObject[] enemyAreaPrefabs; // Different enemy area prefabs
    [SerializeField] private float[] spawnRates;            // Corresponding spawn rates (weights)

    private void Start()
    {
        GenerateGrid();
    }

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (ground == null || enemyAreaPrefabs.Length == 0)
        {
            Debug.LogError("Ground or EnemyArea prefabs not assigned!");
            return;
        }

        // Clear previous generated objects
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        Renderer groundRenderer = ground.GetComponent<Renderer>();
        if (groundRenderer == null)
        {
            Debug.LogError("Ground must have a Renderer component!");
            return;
        }

        // Measure ground bounds
        Vector3 groundSize = groundRenderer.bounds.size;
        Vector3 origin = ground.position - new Vector3(groundSize.x / 2f, 0, groundSize.z / 2f);

        int gridX = Mathf.FloorToInt(groundSize.x / cellSize);
        int gridZ = Mathf.FloorToInt(groundSize.z / cellSize);

        Debug.Log($"Generating grid: {gridX}x{gridZ}");

        // Generate grid and spawn prefabs
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                Vector3 spawnPos = origin + new Vector3((x + 0.5f) * cellSize, 0, (z + 0.5f) * cellSize);
                GameObject selectedPrefab = GetRandomEnemyArea();

                if (selectedPrefab != null)
                {
                    GameObject area = Instantiate(selectedPrefab, spawnPos, Quaternion.identity, transform);
                    area.name = $"EnemyArea_{x}_{z}";
                }
            }
        }
    }

    private GameObject GetRandomEnemyArea()
    {
        if (enemyAreaPrefabs.Length == 0)
            return null;

        // Safety check: match array sizes
        if (enemyAreaPrefabs.Length != spawnRates.Length)
        {
            Debug.LogWarning("Spawn rates count does not match prefab count!");
            return enemyAreaPrefabs[0];
        }

        // Weighted random selection
        float totalWeight = 0f;
        foreach (float rate in spawnRates)
            totalWeight += rate;

        float randomValue = Random.value * totalWeight;

        for (int i = 0; i < enemyAreaPrefabs.Length; i++)
        {
            if (randomValue < spawnRates[i])
                return enemyAreaPrefabs[i];

            randomValue -= spawnRates[i];
        }

        // Fallback
        return enemyAreaPrefabs[enemyAreaPrefabs.Length - 1];
    }
}
