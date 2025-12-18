using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossWalk : StateMachineBehaviour
{

    Rigidbody rb;
    Transform player;

    public float speed = 5f;

    BossManager bossManager;
    private Transform bosstransform;
    private Transform bossParentTransform;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossManager = animator.GetComponentInParent<BossManager>();
        bosstransform = animator.GetComponentInParent<Transform>();
        bossParentTransform = animator.transform.parent;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (rb.IsSleeping()) rb.WakeUp(); 
        // Vector3 target = new Vector3(player.position.x, rb.position.y, player.position.z);
        // Vector3 newPos = Vector3.MoveTowards(rb.position, target, speed*Time.deltaTime);
        

        // float distance = Vector3.Distance(bosstransform.position, target);

        // if (distance > 1.5f)
        // {
        //     //bosstransform.position = Vector3.MoveTowards(bosstransform.position, target, speed * Time.deltaTime);
        //     rb.MovePosition(newPos);
        // }

// Calculate target at the same height as the boss
        Vector3 target = new Vector3(player.position.x, bossParentTransform.position.y, player.position.z);
        
        float distance = Vector3.Distance(bossParentTransform.position, target);

        if (distance > 1.5f)
        {
            // Directly move the parent transform
            bossParentTransform.position = Vector3.MoveTowards(
                bossParentTransform.position, 
                target, 
                speed * Time.deltaTime
            );
        }


    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
