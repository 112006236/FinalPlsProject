using UnityEngine;

public class SkeletonFaceCamera : MonoBehaviour
{
    public Camera mainCamera;
    public bool lockYRotation = true;

    void LateUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 direction = mainCamera.transform.position - transform.position;

        if (lockYRotation)
        {
            direction.y = 0;
        }

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(-direction); 
    }
}
