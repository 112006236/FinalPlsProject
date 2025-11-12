using UnityEngine;

public class TreeFaceCamera : MonoBehaviour
{
    public Camera mainCamera;
    public bool lockYRotation = true; // lock upright rotation

    void LateUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 direction = mainCamera.transform.position - transform.position;

        if (lockYRotation)
        {
            // ignore vertical difference so sprite stays upright
            direction.y = 0;
        }

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(-direction); // look at camera
    }
}
