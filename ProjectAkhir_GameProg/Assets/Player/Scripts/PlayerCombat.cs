using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    public float timeBetweenCombo = 0.5f;
    public float timeBetweenAttacks = 0.2f;
    public float exitComboTime = 0.5f;

    public Animator anim;
    PlayerInputActions inputs;

    private bool inCombo;

    // Start is called before the first frame update
    void Start()
    {
        inputs = new PlayerInputActions();
        inputs.Player.Enable();
        inputs.Player.Attack.performed += Attack;

        inCombo = false;
        // comboCounter = 0;
        // lastComboEnd = Time.time;
        // lastClickedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ExitAttack();
    }

    void Attack(InputAction.CallbackContext context)
    {
        // if (Time.time - lastComboEnd > timeBetweenCombo && comboCounter < combo.Count)
        // {

        // } 

        inCombo = true;
        
        CancelInvoke("EndCombo");

        if (Time.time - lastClickedTime >= timeBetweenAttacks)
        {
            Debug.Log("Attack Pattern " + comboCounter);

            anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
            anim.Play("Attack", 0, 0);
            //#: Modify damage
            comboCounter++;
            lastClickedTime = Time.time;

            if (comboCounter >= combo.Count)
            {
                comboCounter = 0;
            }
        } else
        {
            Debug.Log("Waiting attack cooldown...");
        }
    }

    void ExitAttack()
    {
        if (
            // anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
            // anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            Time.time - lastClickedTime > exitComboTime
            )
        {
            // if (inCombo) Invoke("ExitCombo", 1); //# Experiment what if we call ExitCombo directly
            if (inCombo) ExitCombo();
        }
    }
    
    void ExitCombo()
    {
        Debug.Log("ExitCombo");
        comboCounter = 0;
        lastComboEnd = Time.time;
        inCombo = false;
    }
}
