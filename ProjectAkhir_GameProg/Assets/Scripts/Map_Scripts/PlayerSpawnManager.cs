using UnityEngine;
using Cinemachine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject playerPrefab;      // The player prefab
    public Transform spawnPoint;         // Where the player should spawn

    [Header("Cinemachine Camera")]
    public CinemachineVirtualCamera vCam; // Virtual camera to follow the player

    [Header("Drop Settings")]
    public float dropDuration = 1f;       // Duration of the slam-drop effect
    public Vector3 dropOffset = new Vector3(0, 10f, 0); // Start height above spawn point

    private GameObject spawnedPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("PlayerPrefab or SpawnPoint not assigned!");
            return;
        }

        // --- Step 1: Instantiate player slightly above spawn point ---
        Vector3 startPos = spawnPoint.position + dropOffset;
        spawnedPlayer = Instantiate(playerPrefab, startPos, spawnPoint.rotation);

        // --- Step 2: Trigger drop effect if available ---
        PlayerSpawnDrop dropEffect = spawnedPlayer.GetComponent<PlayerSpawnDrop>();
        if (dropEffect != null)
        {
            dropEffect.StartDrop(spawnPoint.position, dropDuration);
        }

        // --- Step 3: Assign Cinemachine Virtual Camera ---
        if (vCam != null)
        {
            vCam.Follow = spawnedPlayer.transform;
            vCam.LookAt = spawnedPlayer.transform;
        }
    }
}
