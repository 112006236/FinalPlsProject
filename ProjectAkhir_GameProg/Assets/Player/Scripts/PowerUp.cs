using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    private PlayerCombat playerCombat;

    [Header("ATTACK Power Ups")]
    public float critChanceIncrease = 0.1f; // +10%
    public float attackSpeedMultiplier = 0.85f;
    public int extraWaves = 1; // Extra waves for SM1

    [Header("UTILITY Power Ups")]
    public float damageReductionPercent = 0.3f; // 30%
    public float cooldownRefundOnKill = 0.5f;    // seconds refunded
    public float lifestealAmount = 0.1f;

    [Header("CC Power Ups")]
    public float sm2StunDuration = 1.0f;
    public float sm2StaggerMultiplier = 1.0f;
    public int sm2ExtraWaves = 1; // Number of extra SM2 waves per power-up

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
            AddCritChance();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            IncreaseAttackSpeed();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            EnableLifesteal();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            EnableDamageReduction();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            EnableCooldownRefund();

        if (Input.GetKeyDown(KeyCode.Alpha8))
            EnableExtraWaves();

        if (Input.GetKeyDown(KeyCode.Alpha9))
            EnableSM2Waves();

        if (Input.GetKeyDown(KeyCode.Alpha0))
            EnableSM2Stun();
    }

    void AddCritChance()
    {
        playerCombat.critChance += critChanceIncrease;
        playerCombat.critChance = Mathf.Clamp01(playerCombat.critChance);
        Debug.Log($"Attack: Crit Chance Increased (+{critChanceIncrease * 100}%)");
    }

    void IncreaseAttackSpeed()
    {
        playerCombat.timeBetweenAttacks *= attackSpeedMultiplier;
        playerCombat.exitComboTime *= attackSpeedMultiplier;
        Debug.Log($"Attack: Attack Speed Increased (x{attackSpeedMultiplier})");
    }

    void EnableLifesteal()
    {
        playerCombat.lifestealEnabled = true;
        playerCombat.lifestealPercent += lifestealAmount;
        Debug.Log($"Attack: Lifesteal Enabled (+{lifestealAmount * 100}%)");
    }

    void EnableDamageReduction()
    {
        playerCombat.damageReductionWhileAttacking = true;
        playerCombat.attackDamageReduction = damageReductionPercent;
        Debug.Log($"Utility: Damage Reduction While Attacking Enabled ({damageReductionPercent * 100}%)");
    }

    void EnableCooldownRefund()
    {
        playerCombat.cooldownRefundOnKillEnabled = true;
        playerCombat.cooldownRefundAmount = cooldownRefundOnKill;
        Debug.Log($"Utility: Cooldown Refund on Kill Enabled (+{cooldownRefundOnKill}s)");
    }

    void EnableExtraWaves()
    {
        playerCombat.AddSM1Waves(extraWaves);
        Debug.Log($"Attack: Extra SM1 Waves Enabled (+{extraWaves})");
    }

    // Replaces pull effect with extra SM2 waves
    void EnableSM2Waves()
    {
        playerCombat.extraSM2Waves += sm2ExtraWaves;
        Debug.Log($"CC Power-Up: Extra SM2 Waves Enabled (+{sm2ExtraWaves})");
    }

    void EnableSM2Stun()
    {
        playerCombat.sm2StunEnabled = true;
        playerCombat.sm2StunDuration = sm2StunDuration;
        playerCombat.sm2StaggerMultiplier = sm2StaggerMultiplier;
        Debug.Log($"CC Power-Up: SM2 Stun Enabled | Duration {sm2StunDuration}s | Stagger x{sm2StaggerMultiplier}");
    }
}
