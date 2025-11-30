using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public Transform spike;            // The spike object to move
    public float moveDistance = 1f;    // How high the spike moves
    public float launchSpeed = 20f;    // Speed at which spikes shoot up
    public float descendSpeed = 3f;    // Speed at which spikes descend
    public float delayBeforeLaunch = 0.5f; // Delay before spike shoots up
    public float pokeDuration = 1f;    // How long spike stays fully extended
    public float timeBetweenCycles = 2f;   // How long until the next spike cycle

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Coroutine spikeRoutine;

    void Start()
    {
        if (spike == null)
            spike = transform; // Use self if no spike assigned

        initialPosition = spike.localPosition;
        targetPosition = initialPosition + Vector3.up * moveDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && spikeRoutine == null)
        {
            spikeRoutine = StartCoroutine(SpikeCycle());
        }
    }

    private IEnumerator SpikeCycle()
    {
        // Optional delay before launching
        yield return new WaitForSeconds(delayBeforeLaunch);

        // Shoot spike up quickly
        while (spike.localPosition.y < targetPosition.y - 0.01f)
        {
            spike.localPosition += Vector3.up * launchSpeed * Time.deltaTime;
            yield return null;
        }
        spike.localPosition = targetPosition;

        // Keep spike fully extended for pokeDuration
        yield return new WaitForSeconds(pokeDuration);

        // Descend slowly over time
        while (spike.localPosition.y > initialPosition.y + 0.01f)
        {
            spike.localPosition = Vector3.Lerp(spike.localPosition, initialPosition, descendSpeed * Time.deltaTime);
            yield return null;
        }
        spike.localPosition = initialPosition;

        // Wait some time before the next cycle
        yield return new WaitForSeconds(timeBetweenCycles);

        // Reset coroutine so it can trigger again
        spikeRoutine = null;
    }
}
