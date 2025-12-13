using UnityEngine;

public class ShrineIndicator : MonoBehaviour
{
    [Header("Candle Settings")]
    public GameObject candlePrefab;       // Assign your candle prefab
    public int numberOfCandles = 5;       // How many candles to spawn
    public float radius = 2f;             // Circle radius

    private CaptureShrine shrine;
    private GameObject[] candles;

    private void Start()
    {
        shrine = GetComponent<CaptureShrine>();
        if (shrine == null)
        {
            Debug.LogError("ShrineIndicator requires a CaptureShrine component on the same object.");
            return;
        }

        SpawnCandles();
    }

    private void Update()
    {
        if (shrine == null || candles == null) return;

        float capturePercent = Mathf.Clamp01(shrine.GetCapturePercentage());
        int litCandles = Mathf.FloorToInt(capturePercent * numberOfCandles);

        for (int i = 0; i < numberOfCandles; i++)
        {
            // Assume your candle prefab has a method SetLit(bool) or a script to change its state
            var candle = candles[i].GetComponent<Candle>();
            if (candle != null)
            {
                candle.SetLit(i < litCandles);
            }
        }
    }

    private void SpawnCandles()
    {
        candles = new GameObject[numberOfCandles];
        float angleStep = 360f / numberOfCandles;

        for (int i = 0; i < numberOfCandles; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            GameObject candle = Instantiate(candlePrefab, transform.position + offset, Quaternion.identity, transform);
            
            // Rotate candle to face center
            candle.transform.LookAt(transform.position);
            candles[i] = candle;
        }
    }
}
