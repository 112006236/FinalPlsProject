using UnityEngine;
using TMPro; // Uncomment if you want to use TextMeshPro

public class PlayerHealthDebugUI : MonoBehaviour
{
    [Header("References")]
    public PlayerCombat player;
    public TextMeshProUGUI healthText; // Uncomment if using TMP

    void Update()
    {
        if (player == null || healthText == null) return;

        // Update the text to show current health
        healthText.text = "HP: " + Mathf.RoundToInt(player.HP).ToString();
    }
}
//FOR DEBUG PURPOSES ONLY