using UnityEngine;
using System.Linq;

public class Ztargeting : MonoBehaviour
{
    [Header("Settings")]
    public string enemyTag = "Enemy";
    public KeyCode toggleKey = KeyCode.Mouse1;

    private Transform currentTarget;
    private bool lockedOn = false;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (!lockedOn)
                LockOnNearestEnemy();
            else
                Unlock();
        }
    }

    void LockOnNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
            return;

        currentTarget = enemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .First()
            .transform;

        lockedOn = true;
    }

    void Unlock()
    {
        lockedOn = false;
        currentTarget = null;
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    public bool IsLockedOn()
    {
        return lockedOn && currentTarget != null;
    }
}
