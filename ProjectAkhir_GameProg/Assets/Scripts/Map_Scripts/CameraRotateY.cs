using UnityEngine;

public class CameraRotateY : MonoBehaviour
{
    public float rotationSpeed = 50f; // degrees per second

    void Update()
    {
        // Rotate around Y axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
