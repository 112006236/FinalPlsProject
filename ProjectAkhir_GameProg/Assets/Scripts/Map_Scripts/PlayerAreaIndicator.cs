using UnityEngine;
using System.Linq;

public class PlayerAreaIndicator : MonoBehaviour
{
    [Header("Area Tags (Customizable)")]
    public string shrineTag = "Shrine";
    public string enemyAreaTag = "Area";
    public string cageTag = "Princess";

    [Header("Rotation Settings")]
    public float rotateSpeed = 8f;

    private string currentTargetTag;  // tag chosen by button
    private bool isInsideArea = false;

    void Start()
    {
        // Default target so the arrow doesn't break before choosing
        currentTargetTag = shrineTag;
    }

    void Update()
    {
        if (isInsideArea)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        GameObject nearestArea = FindNearestArea(currentTargetTag);
        if (nearestArea == null) return;

        Vector3 dir = nearestArea.transform.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.01f)
        {
            float targetY = Quaternion.LookRotation(dir).eulerAngles.y;

            float newY = Mathf.LerpAngle(
                transform.eulerAngles.y,
                targetY,
                Time.deltaTime * rotateSpeed
            );

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                newY,
                transform.eulerAngles.z
            );
        }
    }

    // ------------------------------
    // FIND NEAREST AREA FOR A TAG
    // ------------------------------
    GameObject FindNearestArea(string tag)
    {
        GameObject[] areas = GameObject.FindGameObjectsWithTag(tag);
        if (areas.Length == 0)
            return null;

        return areas
            .OrderBy(a => Vector3.Distance(transform.position, a.transform.position))
            .FirstOrDefault();
    }

    // ------------------------------
    // BUTTON FUNCTIONS
    // ------------------------------
    public void PointToShrine()
    {
        currentTargetTag = shrineTag;
        isInsideArea = false;  // force arrow visible
    }

    public void PointToEnemyArea()
    {
        currentTargetTag = enemyAreaTag;
        isInsideArea = false;
    }

    public void PointToCage()
    {
        currentTargetTag = cageTag;
        isInsideArea = false;
    }
}
