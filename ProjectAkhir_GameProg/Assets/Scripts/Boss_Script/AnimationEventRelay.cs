using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private BossMeteorAttack bossAttack;

    void Start()
    {
        bossAttack = GetComponentInParent<BossMeteorAttack>();
    }

    // Called by the animation event
    public void TriggerMeteorAttack()
    {
        if (bossAttack != null)
            bossAttack.TriggerMeteorAttack();
    }
}
