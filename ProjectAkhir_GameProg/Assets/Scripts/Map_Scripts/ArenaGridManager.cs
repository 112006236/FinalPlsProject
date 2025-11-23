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

    [Header("Normal Area Prefabs")]
    [SerializeField] private GameObject[] normalAreaPrefabs;
    [SerializeField] private float[] normalSpawnRates;

    [Header("Spawn Point Area")]
    [SerializeField] private GameObject spawnPointPrefab;

    [Header("Outer Walls")]
    [SerializeField] private GameObject wallPrefab;

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

        // --- Randomly place enemy areas ---
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

            // Enemy prefab stays with its normal rotation
            Instantiate(enemyAreaPrefabs[enemyIndex], spawnPos, Quaternion.identity, transform);
            gridFlags[pos.x, pos.y] = enemyIndex;

            // Create buffer around it
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

        // --- Fill remaining cells with normal areas (random rotation) ---
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (gridFlags[x, z] == -1)
                {
                    int normalIndex = GetWeightedRandomIndex(normalSpawnRates);
                    Vector3 spawnPos = origin + new Vector3((x + 0.5f) * cellSize, 0, (z + 0.5f) * cellSize);

                    // Random rotation for normal areas only
                    Quaternion rot = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);

                    Instantiate(normalAreaPrefabs[normalIndex], spawnPos, rot, transform);
                    gridFlags[x, z] = normalIndex;
                }
            }
        }

        // --- Spawn walls on outermost edges ---
        if (wallPrefab != null)
        {
            for (int x = 0; x < gridX; x++)
            {
                SpawnWall(origin, x, 0);
                SpawnWall(origin, x, gridZ - 1);
            }
            for (int z = 0; z < gridZ; z++)
            {
                SpawnWall(origin, 0, z);
                SpawnWall(origin, gridX - 1, z);
            }
        }
    }

    private void SpawnWall(Vector3 origin, int x, int z)
    {
        Vector3 pos = origin + new Vector3((x + 0.5f) * cellSize, 0, (z + 0.5f) * cellSize);
        Instantiate(wallPrefab, pos, Quaternion.identity, transform);
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
