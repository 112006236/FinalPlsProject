using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private BossMeteorAttack bossAttack;
    private BossManager bossManager;

    void Start()
    {
        bossAttack = GetComponentInParent<BossMeteorAttack>();
        bossManager = GetComponentInParent<BossManager>();
    }

    // Called by the animation event
    public void TriggerMeteorAttack()
    {
        if (bossAttack != null)
            bossAttack.TriggerMeteorAttack();
    }

    public void TriggerSlash()
    {
        if (bossManager != null)
        {
            bossManager.SpawnSlash();
        }
    }


}
