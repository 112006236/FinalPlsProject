using UnityEngine;
using System.Linq;

public class PlayerAreaIndicator : MonoBehaviour
{
    [SerializeField] private string areaTag = "Area";
    [SerializeField] private float rotateSpeed = 8f;
    private bool isInsideArea = false;

    void Update()
    {
        // Hide indicator if inside any area
        if (isInsideArea)
        {
            gameObject.SetActive(false);
            return;
        }

        // Make sure the arrow is visible again
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        GameObject nearestArea = FindNearestArea();
        if (nearestArea == null) return;

        Vector3 dir = nearestArea.transform.position - transform.position;

        // Keep direction horizontal
        dir.y = 0;

        if (dir.sqrMagnitude > 0.01f)
        {
            // Find the target Y-Angle only
            float targetY = Quaternion.LookRotation(dir).eulerAngles.y;

            // Smoothly interpolate only the Y axis
            float newY = Mathf.LerpAngle(
                transform.eulerAngles.y,
                targetY,
                Time.deltaTime * rotateSpeed
            );

            // Apply ONLY Y rotation
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                newY,
                transform.eulerAngles.z
            );
        }
    }

    GameObject FindNearestArea()
    {
        GameObject[] areas = GameObject.FindGameObjectsWithTag(areaTag);
        if (areas.Length == 0)
            return null;

        return areas
            .OrderBy(a => Vector3.Distance(transform.position, a.transform.position))
            .FirstOrDefault();
    }

    // --- Player trigger detection ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(areaTag))
            isInsideArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(areaTag))
            isInsideArea = false;
    }
}
