using UnityEngine;

public class CameraLockTrigger : MonoBehaviour
{
    public float zLockValue = -10f;

    private void OnTriggerEnter(Collider other)
    {
        var cam = Camera.main.GetComponent<CameraAxisLocker>();
        if (cam != null)
            cam.Enter2_5D(zLockValue);
    }

    private void OnTriggerExit(Collider other)
    {
        var cam = Camera.main.GetComponent<CameraAxisLocker>();
        if (cam != null)
            cam.Exit2_5D();
    }
}
