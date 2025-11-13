using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Animator bossAnimator;   // Reference to the boss's Animator
    public float phase2Time = 5f;  // Time until switching to state 2
    private float timer = 0f;
    private bool hasEnteredPhase2 = false;

    void Start()
    {
        bossAnimator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        // Keep counting up time
        timer += Time.deltaTime;

        // After 15 seconds, tell the Animator to go to state 2
        if (!hasEnteredPhase2 && timer >= phase2Time)
        {
            bossAnimator.SetTrigger("toWalk"); 
            hasEnteredPhase2 = true; 
        }
    }
}
