using UnityEngine;

public class CameraAxisLocker : MonoBehaviour
{
    public bool in2_5DMode = false;

    public float lockedZ = -10f;                // Set in Inspector if you want
    public Quaternion sideViewRotation;         // Rotation for 2.5D mode
    public float rotationLerpSpeed = 5f;

    private float originalZ;
    private Quaternion originalRotation;

    void Start()
    {
        originalZ = transform.position.z;
        originalRotation = transform.rotation;

        // Default side view: facing the +X direction
        sideViewRotation = Quaternion.Euler(0f, 90f, 0f);
    }

    void LateUpdate()
    {
        if (in2_5DMode)
        {
            // Lock the depth axis
            Vector3 p = transform.position;
            p.z = lockedZ;
            transform.position = p;

            // Rotate into side view
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                sideViewRotation,
                Time.deltaTime * rotationLerpSpeed
            );
        }
        else
        {
            // Restore free camera rotation
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                originalRotation,
                Time.deltaTime * rotationLerpSpeed
            );
        }
    }

    public void Enter2_5D(float zValue)
    {
        in2_5DMode = true;
        lockedZ = zValue;
        originalZ = transform.position.z;
    }

    public void Exit2_5D()
    {
        in2_5DMode = false;
    }
}
