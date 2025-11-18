using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    private EnemySpawner spawner;
    private int groupIndex = -1;
    private bool reported = false;

    public void SetSpawner(EnemySpawner s, int index)
    {
        spawner = s;
        groupIndex = index;
    }

    public void ReportDeath()
    {
        if (reported) return;
        reported = true;

        if (spawner != null)
        {
            spawner.NotifyEnemyDeath(groupIndex, gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!reported && spawner != null)
        {
            spawner.NotifyEnemyDeath(groupIndex, gameObject);
        }
    }
}
