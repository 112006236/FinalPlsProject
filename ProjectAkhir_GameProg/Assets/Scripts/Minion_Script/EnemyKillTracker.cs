using UnityEngine;

public class EnemyKillTracker : MonoBehaviour
{
    public static EnemyKillTracker Instance;

    [SerializeField] private int targetKills = 10;
    private int currentKills = 0;

    private void Awake()
    {
        // Singleton setup so other scripts can access this easily
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Called whenever an enemy dies
    public void AddKill()
    {
        currentKills++;

        Debug.Log("Enemy killed! Total: " + currentKills);

        if (currentKills == targetKills)
        {
            Debug.Log(targetKills + "enemies killed!");
        }
    }

    public int GetKillCount()
    {
        return currentKills;
    }
}
