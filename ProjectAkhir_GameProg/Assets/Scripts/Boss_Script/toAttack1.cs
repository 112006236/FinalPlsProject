using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toAttack1 : StateMachineBehaviour
{
    private float timer;
    private float attackDelay;
    
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackDelay = Random.Range(1.5f, 3f);
        timer = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        timer += Time.deltaTime;

        // When time exceeds the random delay, trigger the attack
        if (timer >= attackDelay)
        {
            animator.SetTrigger("Attack1");
            timer = 0f; // reset if Idle is entered again later
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack1");
    }


}
