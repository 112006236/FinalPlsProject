using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [Header("Arena Settings")]
    [SerializeField] private Vector2 arenaSize = new Vector2(28f, 28f); // XZ size of arena
    [SerializeField] private float cellSize = 7f; // distance between spawns
    [SerializeField] private Material floorMaterial; // optional material for floor

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnPrefabs; // prefabs to spawn in arena
    [SerializeField] private float[] spawnRates; // spawn chance weights
    [SerializeField] [Range(0f, 1f)] private float spawnChance = 0.6f; // how often to spawn

    [Header("Floor Object")]
    private GameObject floorObject;

    private void Start()
    {
        GenerateArena();
    }

    [ContextMenu("Generate Arena")]
    public void GenerateArena()
    {
        // Clear previous arena if re-generated
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // --- 1. Create Floor ---
        floorObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floorObject.name = "ArenaFloor";
        floorObject.transform.SetParent(transform);
        floorObject.transform.localPosition = Vector3.zero;
        floorObject.transform.localRotation = Quaternion.identity;

        // Adjust plane scale (default plane is 10x10 units)
        floorObject.transform.localScale = new Vector3(arenaSize.x / 10f, 1, arenaSize.y / 10f);

        // Apply custom material if assigned
        if (floorMaterial != null)
        {
            floorObject.GetComponent<Renderer>().material = floorMaterial;
        }

        // --- 2. Compute grid cells ---
        int gridX = Mathf.RoundToInt(arenaSize.x / cellSize);
        int gridZ = Mathf.RoundToInt(arenaSize.y / cellSize);

        Vector3 origin = new Vector3(-arenaSize.x / 2f, 0f, -arenaSize.y / 2f);

        Debug.Log($"Generating arena: {gridX}x{gridZ} cells");

        // --- 3. Spawn prefabs across the arena ---
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (Random.value > spawnChance)
                    continue; // skip some cells for variation

                Vector3 spawnPos = origin + new Vector3(x * cellSize, 0, z * cellSize);

                GameObject prefab = GetRandomPrefab();
                if (prefab != null)
                {
                    Instantiate(prefab, spawnPos, Quaternion.identity, transform);
                }
            }
        }

        // --- 4. Keep arena visually centered ---
        transform.position = Vector3.zero;
    }

    private GameObject GetRandomPrefab()
    {
        if (spawnPrefabs == null || spawnPrefabs.Length == 0)
            return null;

        if (spawnPrefabs.Length != spawnRates.Length)
            return spawnPrefabs[0];

        float total = 0f;
        foreach (float rate in spawnRates)
            total += rate;

        float rand = Random.value * total;

        for (int i = 0; i < spawnPrefabs.Length; i++)
        {
            if (rand < spawnRates[i])
                return spawnPrefabs[i];
            rand -= spawnRates[i];
        }

        return spawnPrefabs[spawnPrefabs.Length - 1];
    }
}
