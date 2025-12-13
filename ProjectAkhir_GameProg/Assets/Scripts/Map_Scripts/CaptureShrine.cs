using UnityEngine;
using UnityEngine.UI;
using System;

public class CaptureShrine : MonoBehaviour
{
    [Header("Capture Settings")]
    public float captureTime = 5f;
    public Collider captureTrigger;

    [Header("UI Settings")]
    public Image captureBarForeground;
    public Image captureBarBackground;

    [Header("Rune Settings")]
    public Transform runeQuad;           // Assign your circular rune quad
    public float rotationSpeed = 90f;     // Degrees per second
    public Color capturedColor = Color.red;

    private float captureProgress = 0f;
    private bool playerInside = false;
    private bool shrineCaptured = false;

    private Renderer runeRenderer;

    public event Action OnShrineCaptured;

    private void Start()
    {
        if (ArenaControl.Instance != null)
        {
            ArenaControl.Instance.RegisterShrine(this);
        }

        if (captureBarForeground != null)
            captureBarForeground.fillAmount = 0f;

        if (runeQuad != null)
            runeRenderer = runeQuad.GetComponent<Renderer>();
    }

    private void Update()
    {
        if (shrineCaptured) return;

        if (playerInside)
        {
            // Rotate rune while capturing
            RotateRune();

            captureProgress += Time.deltaTime;

            if (captureProgress >= captureTime)
            {
                captureProgress = captureTime;
                shrineCaptured = true;

                SetRuneCapturedColor();

                Debug.Log($"🏆 Shrine captured: {gameObject.name}");
                OnShrineCaptured?.Invoke();
            }

            UpdateCaptureBar();
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

    // ---------- Helpers ----------

    private void RotateRune()
    {
        if (runeQuad != null)
        {
            runeQuad.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void SetRuneCapturedColor()
    {
        if (runeRenderer != null)
        {
            runeRenderer.material.color = capturedColor;
        }
    }

    private void UpdateCaptureBar()
    {
        if (captureBarForeground != null)
        {
            captureBarForeground.fillAmount = captureProgress / captureTime;
        }
    }

    public bool IsPlayerInside() => playerInside;
    public bool IsCaptured() => shrineCaptured;
    public float GetCapturePercentage() => captureProgress / captureTime;
}
