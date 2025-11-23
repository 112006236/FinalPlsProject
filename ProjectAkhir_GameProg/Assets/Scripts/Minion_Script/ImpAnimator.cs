using UnityEngine;
using UnityEngine.AI;

public class ImpAnimator : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        HandleMovementAnimation();
    }

    void HandleMovementAnimation()
    {
        if (anim == null || agent == null)
            return;

        bool isRunning = agent.velocity.magnitude > 0.1f;
        anim.SetBool("IsRunning", isRunning);
    }
}
