using UnityEngine;

public enum PowerUpType
{
    CritChance,
    AttackSpeed,
    Lifesteal,
    DamageReduction,
    CooldownRefund,
    ExtraSM1Waves,
    ExtraSM2Waves,
    SM2Stun
}

[CreateAssetMenu(menuName = "PowerUps/PowerUp")]
public class PowerUpSO : ScriptableObject
{
    public string powerUpName;
    public string description;
    public Sprite icon;
    public PowerUpType type;

    [Header("Values")]
    public float floatValue;
    public int intValue;
}