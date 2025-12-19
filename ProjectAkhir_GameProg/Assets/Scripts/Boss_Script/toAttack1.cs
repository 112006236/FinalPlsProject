using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toAttack1 : StateMachineBehaviour
{
    private float timer;
    private float attackDelay;

    private float goState2;
    private bool hasTransitioned; // NEW: Prevent multiple triggers
    
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        hasTransitioned = false; // Reset when entering state
        attackDelay = Random.Range(1.5f, 3f);
        goState2 = Random.Range(5f, 6f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        timer += Time.deltaTime;

        // Attack logic
        if (timer >= attackDelay && !hasTransitioned) 
        {
            animator.SetTrigger("Attack1");
            attackDelay = timer + Random.Range(1.5f, 3f);
        }


        

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("toWalk"); // Reset toWalk so it doesn't fire again
    }


}
