using UnityEngine;

public class GroundTileManager : MonoBehaviour
{
    [Header("Assign your GameObjects")]
    [SerializeField] private GameObject groundObject;
    [SerializeField] private GameObject tileObject;

    [Header("Ground Info (Read Only)")]
    [SerializeField] private Vector3 groundSize;

    void Start()
    {
        if (groundObject != null)
        {
            // Try to get the Renderer bounds or Collider size
            Renderer groundRenderer = groundObject.GetComponent<Renderer>();
            Collider groundCollider = groundObject.GetComponent<Collider>();

            if (groundRenderer != null)
            {
                groundSize = groundRenderer.bounds.size;
            }
            else if (groundCollider != null)
            {
                groundSize = groundCollider.bounds.size;
            }
            else
            {
                Debug.LogWarning("Ground object has no Renderer or Collider to measure size.");
                groundSize = Vector3.zero;
            }

            Debug.Log($"Ground size detected: {groundSize}");
        }
        else
        {
            Debug.LogError("Please assign the Ground object in the Inspector.");
        }

        if (tileObject == null)
        {
            Debug.LogWarning("No tile object assigned yet.");
        }
    }
}
