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

    [Header("Border Offset (push rocks outward)")]
    public float borderPushOut = 0.5f;

    private float width;
    private float height;
    private Bounds groundBounds;

    // Track already used positions to prevent duplicates
    private HashSet<Vector2> usedPositions = new HashSet<Vector2>();

    void Start()
    {
        if (!groundObject)
        {
            Debug.LogError("No Ground Object assigned!");
            return;
        }

        groundBounds = GetObjectBounds(groundObject);
        width = groundBounds.size.x;
        height = groundBounds.size.z;

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

        float left = groundBounds.min.x - borderPushOut;
        float right = groundBounds.max.x + borderPushOut;
        float bottom = groundBounds.min.z - borderPushOut;
        float top = groundBounds.max.z + borderPushOut;

        usedPositions.Clear();

        // X-axis borders (bottom & top)
        for (float x = left; x <= right; x += rockSpacing)
        {
            Vector2 bottomKey = new Vector2(x, bottom);
            Vector2 topKey = new Vector2(x, top);

            if (!usedPositions.Contains(bottomKey))
            {
                SpawnRock(new Vector3(x, 0, bottom));
                usedPositions.Add(bottomKey);
            }

            if (!usedPositions.Contains(topKey))
            {
                SpawnRock(new Vector3(x, 0, top));
                usedPositions.Add(topKey);
            }
        }

        // Z-axis borders (left & right)
        for (float z = bottom; z <= top; z += rockSpacing)
        {
            Vector2 leftKey = new Vector2(left, z);
            Vector2 rightKey = new Vector2(right, z);

            if (!usedPositions.Contains(leftKey))
            {
                SpawnRock(new Vector3(left, 0, z));
                usedPositions.Add(leftKey);
            }

            if (!usedPositions.Contains(rightKey))
            {
                SpawnRock(new Vector3(right, 0, z));
                usedPositions.Add(rightKey);
            }
        }
    }

    void SpawnRock(Vector3 pos)
    {
        pos += new Vector3(
            Random.Range(-randomOffset, randomOffset),
            heightOffset,
            Random.Range(-randomOffset, randomOffset)
        );

        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
        GameObject rock = Instantiate(prefab, pos, Quaternion.identity);

        if (randomRotation)
            rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        rock.transform.localScale = rock.transform.localScale * scaleMultiplier;

        rock.transform.SetParent(transform);
    }
}
