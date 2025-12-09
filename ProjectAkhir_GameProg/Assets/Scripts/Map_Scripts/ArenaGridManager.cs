using System.Collections.Generic;
using UnityEngine;

public class ArenaGridManager : MonoBehaviour
{
    [Header("Arena Ground")]
    [SerializeField] private Transform ground;
    [SerializeField] private float cellSize = 10f;

    [Header("Enemy Area Prefabs")]
    [SerializeField] private GameObject[] enemyAreaPrefabs;
    [SerializeField] private int enemyAreaCount = 5;

    [Header("Shrine & Cage Prefabs")]
    [SerializeField] private GameObject shrinePrefab;
    [SerializeField] private int shrineCount = 2;
    [SerializeField] private GameObject cagePrefab;
    [SerializeField] private int cageCount = 2;

    [Header("Normal Area Prefabs")]
    [SerializeField] private GameObject[] normalAreaPrefabs;
    [SerializeField] private float[] normalSpawnRates;

    [Header("Spawn Point Area")]
    [SerializeField] private GameObject spawnPointPrefab;

    private int[,] gridFlags; // -1 empty, -2 buffer, >=0 used

    private void Start()
    {
        GenerateGrid();
    }

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (!ground)
        {
            Debug.LogError("Ground not assigned!");
            return;
        }

        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        Renderer groundRenderer = ground.GetComponent<Renderer>();
        Vector3 groundSize = groundRenderer.bounds.size;
        Vector3 origin = ground.position - new Vector3(groundSize.x / 2f, 0, groundSize.z / 2f);

        int gridX = Mathf.FloorToInt(groundSize.x / cellSize);
        int gridZ = Mathf.FloorToInt(groundSize.z / cellSize);

        gridFlags = new int[gridX, gridZ];

        for (int x = 0; x < gridX; x++)
            for (int z = 0; z < gridZ; z++)
                gridFlags[x, z] = -1;

        int middleX = gridX / 2;
        int middleZ = gridZ / 2;

        // --- Spawn center ---
        if (spawnPointPrefab != null)
        {
            Vector3 spawnPos = origin + new Vector3((middleX + 0.5f) * cellSize, 0, (middleZ + 0.5f) * cellSize);
            Instantiate(spawnPointPrefab, spawnPos, Quaternion.identity, transform);
            gridFlags[middleX, middleZ] = -2;
        }

        // --- Randomly place enemy areas (with buffer) ---
        List<Vector2Int> available = new List<Vector2Int>();
        for (int x = 0; x < gridX; x++)
            for (int z = 0; z < gridZ; z++)
                if (gridFlags[x, z] == -1)
                    available.Add(new Vector2Int(x, z));

        for (int i = 0; i < Mathf.Min(enemyAreaCount, available.Count); i++)
        {
            int r = Random.Range(0, available.Count);
            Vector2Int pos = available[r];
            available.RemoveAt(r);

            int enemyIndex = Random.Range(0, enemyAreaPrefabs.Length);
            Vector3 spawnPos = origin + new Vector3((pos.x + 0.5f) * cellSize, 0, (pos.y + 0.5f) * cellSize);

            Instantiate(enemyAreaPrefabs[enemyIndex], spawnPos, Quaternion.identity, transform);
            gridFlags[pos.x, pos.y] = enemyIndex;

            // Create buffer around enemy areas
            for (int dx = -1; dx <= 1; dx++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = pos.x + dx;
                    int nz = pos.y + dz;
                    if (nx >= 0 && nx < gridX && nz >= 0 && nz < gridZ && gridFlags[nx, nz] == -1)
                    {
                        gridFlags[nx, nz] = -2;
                        available.RemoveAll(p => p.x == nx && p.y == nz);
                    }
                }
        }

        // --- Place Shrine areas (no buffer) ---
        for (int i = 0; i < shrineCount; i++)
        {
            Vector2Int pos = GetRandomEmptyCell(gridX, gridZ);
            if (pos.x == -1) break; // no empty cell found

            Vector3 spawnPos = origin + new Vector3((pos.x + 0.5f) * cellSize, 0, (pos.y + 0.5f) * cellSize);
            Instantiate(shrinePrefab, spawnPos, Quaternion.identity, transform);
            gridFlags[pos.x, pos.y] = -3; // optional special flag
        }

        // --- Place Cage areas (no buffer) ---
        for (int i = 0; i < cageCount; i++)
        {
            Vector2Int pos = GetRandomEmptyCell(gridX, gridZ);
            if (pos.x == -1) break;

            Vector3 spawnPos = origin + new Vector3((pos.x + 0.5f) * cellSize, 0, (pos.y + 0.5f) * cellSize);
            Instantiate(cagePrefab, spawnPos, Quaternion.identity, transform);
            gridFlags[pos.x, pos.y] = -4; // optional special flag
        }

        // --- Fill remaining cells with normal areas (random rotation) ---
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (gridFlags[x, z] == -1)
                {
                    int normalIndex = GetWeightedRandomIndex(normalSpawnRates);
                    Vector3 spawnPos = origin + new Vector3((x + 0.5f) * cellSize, 0, (z + 0.5f) * cellSize);

                    Quaternion rot = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);

                    Instantiate(normalAreaPrefabs[normalIndex], spawnPos, rot, transform);
                    gridFlags[x, z] = normalIndex;
                }
            }
        }
    }

    private Vector2Int GetRandomEmptyCell(int gridX, int gridZ)
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();
        for (int x = 0; x < gridX; x++)
            for (int z = 0; z < gridZ; z++)
                if (gridFlags[x, z] == -1)
                    emptyCells.Add(new Vector2Int(x, z));

        if (emptyCells.Count == 0) return new Vector2Int(-1, -1);

        int r = Random.Range(0, emptyCells.Count);
        return emptyCells[r];
    }

    private int GetWeightedRandomIndex(float[] rates)
    {
        float total = 0f;
        foreach (var r in rates) total += r;

        float rnd = Random.value * total;
        for (int i = 0; i < rates.Length; i++)
        {
            if (rnd < rates[i])
                return i;
            rnd -= rates[i];
        }
        return rates.Length - 1;
    }
}
