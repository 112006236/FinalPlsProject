using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("UI")]
    public GameObject panel;
    public Button[] buttons;
    public Image[] icons;
    public TextMeshProUGUI[] titles;
    public TextMeshProUGUI[] descriptions;

    [Header("PowerUps Pool")]
    public List<PowerUpSO> allPowerUps;

    private PlayerCombat playerCombat;
    private List<PowerUpSO> currentChoices = new();

    private void Awake()
    {
        Debug.Log("SHOW SKILLS UI");
        Instance = this;
        panel.SetActive(false);
    }

    private void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    public void ShowChoices(bool levelWin)
    {
        Time.timeScale = 0f;

        panel.SetActive(true);

        currentChoices.Clear();

        if (allPowerUps.Count < 3)
        {
            Debug.LogError("Not enough PowerUps in the pool!");
            return;
        }

        int choiceCount = Mathf.Min(
            3,
            buttons.Length,
            icons.Length,
            titles.Length,
            descriptions.Length
        );
        choiceCount = 3;

        List<PowerUpSO> pool = new List<PowerUpSO>(allPowerUps);

        for (int i = 0; i < choiceCount; i++)
        {
            int index = Random.Range(0, pool.Count);
            PowerUpSO chosen = pool[index];
            pool.RemoveAt(index);
            currentChoices.Add(chosen);

            // icons[i].sprite = chosen.icon;
            titles[i].SetText(chosen.powerUpName);
            descriptions[i].SetText(chosen.description);

            int btnIndex = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => SelectPowerUp(btnIndex, levelWin));
        }
    }


    public void SelectPowerUp(int index, bool levelWin)
    {
        Debug.Log(currentChoices.Count);
        for (int i = 0; i < currentChoices.Count; i++)
        {
            Debug.Log(currentChoices[i].description);
        }
        ApplyPowerUp(currentChoices[index]);
        Close();

        if (levelWin)
        {
            SceneManager.LoadScene("Boss_Jor");
        }
    }

    void Close()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    void ApplyPowerUp(PowerUpSO powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.CritChance:
                playerCombat.critChance += powerUp.floatValue;
                break;

            case PowerUpType.AttackSpeed:
                playerCombat.timeBetweenAttacks *= powerUp.floatValue;
                break;

            case PowerUpType.Lifesteal:
                playerCombat.lifestealEnabled = true;
                playerCombat.lifestealPercent += powerUp.floatValue;
                break;

            case PowerUpType.DamageReduction:
                playerCombat.damageReductionWhileAttacking = true;
                playerCombat.attackDamageReduction = powerUp.floatValue;
                break;

            case PowerUpType.CooldownRefund:
                playerCombat.cooldownRefundOnKillEnabled = true;
                playerCombat.cooldownRefundAmount = powerUp.floatValue;
                break;

            case PowerUpType.ExtraSM1Waves:
                playerCombat.AddSM1Waves(powerUp.intValue);
                break;

            case PowerUpType.ExtraSM2Waves:
                playerCombat.extraSM2Waves += powerUp.intValue;
                break;

            case PowerUpType.SM2Stun:
                playerCombat.sm2StunEnabled = true;
                playerCombat.sm2StunDuration = powerUp.floatValue;
                break;
        }
    }
}
