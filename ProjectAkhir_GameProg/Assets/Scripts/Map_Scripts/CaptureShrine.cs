using UnityEngine;
using System;

public class CaptureShrine : MonoBehaviour
{
    [Header("Capture Settings")]
    public float captureTime = 5f;        // Total time required to capture
    public Collider captureTrigger;       // Trigger zone around shrine

    private float captureProgress = 0f;
    private bool playerInside = false;
    private bool shrineCaptured = false;

    public event Action OnShrineCaptured; // Event for other systems

    private void Start()
    {
        // Automatically register this shrine with the ArenaControl
        if (ArenaControl.Instance != null)
        {
            ArenaControl.Instance.RegisterShrine(this);
        }
    }

    private void Update()
    {
        if (shrineCaptured) return;

        if (playerInside)
        {
            captureProgress += Time.deltaTime;

            if (captureProgress >= captureTime)
            {
                shrineCaptured = true;
                Debug.Log($"🏆 Shrine captured: {gameObject.name}");

                OnShrineCaptured?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    // --- Helper Methods ---
    public bool IsPlayerInside() => playerInside;
    public bool IsCaptured() => shrineCaptured;
    public float GetCaptureProgress() => captureProgress;
    public float GetCapturePercentage() => captureProgress / captureTime;
}
