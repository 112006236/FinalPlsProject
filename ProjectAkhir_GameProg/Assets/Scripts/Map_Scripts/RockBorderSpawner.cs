using System.Collections.Generic;
using UnityEngine;

public class RockBorderSpawner : MonoBehaviour
{
    [Header("Ground Object (auto-detect size)")]
    public GameObject groundObject;

    [Header("Border Settings")]
    public float rockSpacing = 1.5f;
    public float randomOffset = 0.5f;
    public float heightOffset = 0f;
    public bool randomRotation = true;

    [Header("Rock Prefabs")]
    public GameObject[] rockPrefabs;

    [Header("Rock Scale")]
    public float scaleMultiplier = 1f;

    private float width;
    private float height;

    // Track already used positions to prevent duplicates
    private HashSet<Vector2> usedPositions = new HashSet<Vector2>();

    void Start()
    {
        if (!groundObject)
        {
            Debug.LogError("No Ground Object assigned!");
            return;
        }

        Bounds bounds = GetObjectBounds(groundObject);
        width = bounds.size.x;
        height = bounds.size.z;

        GenerateBorder();
    }

    Bounds GetObjectBounds(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend) return rend.bounds;

        Collider col = obj.GetComponent<Collider>();
        if (col) return col.bounds;

        Debug.LogError("Ground object needs a Renderer or Collider!");
        return new Bounds(Vector3.zero, Vector3.zero);
    }

    void GenerateBorder()
    {
        if (rockPrefabs.Length == 0)
        {
            Debug.LogWarning("No rock prefabs assigned!");
            return;
        }

        float left = -width / 2f;
        float right = width / 2f;
        float bottom = -height / 2f;
        float top = height / 2f;

        // Clear used positions before spawning
        usedPositions.Clear();

        // X-axis borders (left & right)
        for (float x = left; x <= right; x += rockSpacing)
        {
            Vector2 leftPosKey = new Vector2(x, bottom);
            Vector2 rightPosKey = new Vector2(x, top);

            if (!usedPositions.Contains(leftPosKey))
            {
                SpawnRock(new Vector3(x, 0, bottom));
                usedPositions.Add(leftPosKey);
            }

            if (!usedPositions.Contains(rightPosKey))
            {
                SpawnRock(new Vector3(x, 0, top));
                usedPositions.Add(rightPosKey);
            }
        }

        // Z-axis borders (top & bottom)
        for (float z = bottom; z <= top; z += rockSpacing)
        {
            Vector2 bottomPosKey = new Vector2(left, z);
            Vector2 topPosKey = new Vector2(right, z);

            if (!usedPositions.Contains(bottomPosKey))
            {
                SpawnRock(new Vector3(left, 0, z));
                usedPositions.Add(bottomPosKey);
            }

            if (!usedPositions.Contains(topPosKey))
            {
                SpawnRock(new Vector3(right, 0, z));
                usedPositions.Add(topPosKey);
            }
        }
    }

    void SpawnRock(Vector3 pos)
    {
        // small random placement variation
        pos += new Vector3(
            Random.Range(-randomOffset, randomOffset),
            heightOffset,
            Random.Range(-randomOffset, randomOffset)
        );

        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
        GameObject rock = Instantiate(prefab, pos, Quaternion.identity);

        if (randomRotation)
            rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // apply scale multiplier
        rock.transform.localScale = rock.transform.localScale * scaleMultiplier;

        rock.transform.SetParent(transform);
    }
}
