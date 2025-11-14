using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Animator bossAnimator;   // Reference to the boss's Animator
    public float phase2Time = 2f;  // Time until switching to state 2
    private float timer = 0f;
    private bool hasEnteredPhase2 = false;

    public bool isFlipped = false;

    public Transform player;

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
    
    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 flipped = transform.localScale;
            flipped.z *= -1f;

            if (transform.position.x > player.position.x && isFlipped)
            {
                transform.localScale = flipped;
                transform.Rotate(0f, 180f, 0f);
                isFlipped = false;
            }
            else if (transform.position.x < player.position.x && !isFlipped) {
                transform.localScale = flipped;
                transform.Rotate(0f, 180f, 0f);
                isFlipped = true;
            }
        }
    }
}
