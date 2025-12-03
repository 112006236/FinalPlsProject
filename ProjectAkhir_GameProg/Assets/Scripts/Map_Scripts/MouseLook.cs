using UnityEngine;

public class MouseLook : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks cursor to center
        Cursor.visible = false;                   // Hides cursor
    }

    void Update()
    {
        // Press ESC to show cursor again
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; // Unlock
            Cursor.visible = true;                  // Show
        }
    }
}
