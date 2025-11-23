using System.Collections;
using UnityEngine;

public class PlayerSpawnDrop : MonoBehaviour
{
    private bool isDropping = false;

    public void StartDrop(Vector3 targetPosition, float duration)
    {
        if (!isDropping)
            StartCoroutine(DropRoutine(targetPosition, duration));
    }

    private IEnumerator DropRoutine(Vector3 targetPosition, float duration)
    {
        isDropping = true;

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Smooth falling
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isDropping = false;
    }
}
