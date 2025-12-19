using UnityEngine;

public class HealAreaVisual : MonoBehaviour
{
    public float duration = 1.5f;

    private Material mat;
    private float timer;

    void Start()
    {
        mat = GetComponentInChildren<Renderer>().material;
        Color c = mat.color;
        c.a = 0f;
        mat.color = c;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        Color c = mat.color;
        c.a = Mathf.Sin(t * Mathf.PI) * 0.5f; // smooth fade in/out
        mat.color = c;

        if (timer >= duration)
            Destroy(gameObject);
    }
}
