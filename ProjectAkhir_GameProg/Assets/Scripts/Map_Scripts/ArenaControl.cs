using UnityEngine;
using System.Collections.Generic;

public class ArenaControl : MonoBehaviour
{
    public static ArenaControl Instance;

    private List<EnemySpawner> enemyAreas = new List<EnemySpawner>();
    private int clearedAreas = 0;

    private void Awake()
    {
        Instance = this;
    }

    // EnemySpawner calls this when it is spawned
    public void RegisterArea(EnemySpawner spawner)
    {
        if (!enemyAreas.Contains(spawner))
        {
            enemyAreas.Add(spawner);
            Debug.Log($"✔ Registered Enemy Area: {spawner.gameObject.name}. Total areas: {enemyAreas.Count}");
        }
    }

    public void NotifyAreaCleared(EnemySpawner area)
    {
        clearedAreas++;
        Debug.Log($"🔥 Area cleared! {clearedAreas}/{enemyAreas.Count}");

        if (clearedAreas >= enemyAreas.Count)
        {
            SpawnFinalBoss();
        }
    }

    private void SpawnFinalBoss()
    {
        Debug.Log("💀 FINAL BOSS SPAWN WOULD BE SPAWNED HERE!");
    }
}
