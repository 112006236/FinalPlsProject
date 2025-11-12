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

    [Header("Spawn Point Area")]
    [SerializeField] private GameObject spawnPointPrefab;   // The special spawn point area (spawns once in center)

    private int[,] placedPrefabIndices; // To remember which prefab was placed

    private void Start()
    {
        GenerateGrid();
    }

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (ground == null)
        {
            Debug.LogError("Ground not assigned!");
            return;
        }

        if (enemyAreaPrefabs.Length == 0)
        {
            Debug.LogError("EnemyArea prefabs not assigned!");
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
        placedPrefabIndices = new int[gridX, gridZ];

        // Center tile indices
        int middleX = gridX / 2;
        int middleZ = gridZ / 2;

        Debug.Log($"Generating grid: {gridX}x{gridZ}");

        // Generate grid and spawn prefabs
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                Vector3 spawnPos = origin + new Vector3((x + 0.5f) * cellSize, 0, (z + 0.5f) * cellSize);

                GameObject prefabToSpawn;

                // Center cell → spawn the special area
                if (x == middleX && z == middleZ && spawnPointPrefab != null)
                {
                    prefabToSpawn = spawnPointPrefab;
                }
                else
                {
                    int selectedIndex = GetNonRepeatingRandomIndex(x, z);
                    placedPrefabIndices[x, z] = selectedIndex;
                    prefabToSpawn = enemyAreaPrefabs[selectedIndex];
                }

                GameObject area = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
                area.name = $"Area_{x}_{z}";
            }
        }
    }

    private int GetNonRepeatingRandomIndex(int x, int z)
    {
        int maxTries = 10; // Prevent infinite loops
        int selectedIndex = GetWeightedRandomIndex();

        for (int i = 0; i < maxTries; i++)
        {
            if (!IsSameAsNeighbor(x, z, selectedIndex))
                break;

            // Retry if it's the same as a neighbor
            selectedIndex = GetWeightedRandomIndex();
        }

        return selectedIndex;
    }

    private bool IsSameAsNeighbor(int x, int z, int prefabIndex)
    {
        // Check left and top neighbors
        if (x > 0 && placedPrefabIndices[x - 1, z] == prefabIndex)
            return true;
        if (z > 0 && placedPrefabIndices[x, z - 1] == prefabIndex)
            return true;
        return false;
    }

    private int GetWeightedRandomIndex()
    {
        if (enemyAreaPrefabs.Length == 0)
            return 0;

        if (enemyAreaPrefabs.Length != spawnRates.Length)
        {
            Debug.LogWarning("Spawn rates count does not match prefab count!");
            return 0;
        }

        float totalWeight = 0f;
        foreach (float rate in spawnRates)
            totalWeight += rate;

        float randomValue = Random.value * totalWeight;

        for (int i = 0; i < spawnRates.Length; i++)
        {
            if (randomValue < spawnRates[i])
                return i;
            randomValue -= spawnRates[i];
        }

        return spawnRates.Length - 1;
    }
}
