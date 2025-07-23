using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "William/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string UpgradeName; // e.g., "MATCHA", "RAMEN"

    [SerializeField, Range(0f, 100f)] private float damageBonusFlat; 
    [SerializeField] private bool damageBonusPercent; 
    [SerializeField, Range(0f, 100f)] private float damageBonusPercentValue; 

    [SerializeField, Range(0f, 100f)] private float attackRangeBonusFlat; 
    [SerializeField] private bool attackRangeBonusPercent; 
    [SerializeField, Range(0f, 100f)] private float attackRangeBonusPercentValue; 

    [SerializeField, Range(0f, 100f)] private float attackSpeedBonusFlat; 
    [SerializeField] private bool attackSpeedBonusPercent; 
    [SerializeField, Range(0f, 100f)] private float attackSpeedBonusPercentValue; 

    [SerializeField, Range(0f, 100f)] private float attackAreaBonusFlat; 
    [SerializeField] private bool attackAreaBonusPercent; 
    [SerializeField, Range(0f, 100f)] private float attackAreaBonusPercentValue;

    [SerializeField, Range(0f, 10f)] private float dotBonusFlat; // Flat DoT increase (e.g., +1 for SPICY_RAMEN)
    [SerializeField] private bool dotBonusPercent; // True for percentage, false for flat
    [SerializeField, Range(0f, 100f)] private float dotBonusPercentValue; // Percentage increase for DoT

    [SerializeField, Range(0f, 10f)] private float dotDurationBonusFlat; // Flat duration increase (e.g., +1s)
    [SerializeField] private bool dotDurationBonusPercent; // True for percentage, false for flat
    [SerializeField, Range(0f, 100f)] private float dotDurationBonusPercentValue; // Percentage increase for duration

    [SerializeField, Range(0f, 100f)] private float healthBonusFlat;
    [SerializeField] private bool healthBonusPercent;
    [SerializeField, Range(0f,100f)] private float healthBonusPercentValue;

    [SerializeField, Range(0f, 100f)] private float healBonusFlat;
    [SerializeField] private bool healBonusPercent;
    [SerializeField, Range(0f, 100f)] private float healBonusPercentValue;

    public float DamageBonusFlat => damageBonusFlat;
    public bool DamageBonusIsPercent => damageBonusPercent;
    public float DamageBonusPercentValue => damageBonusPercentValue;

    public float AttackRangeBonusFlat => attackRangeBonusFlat;
    public bool AttackRangeBonusIsPercent => attackRangeBonusPercent;
    public float AttackRangeBonusPercentValue => attackRangeBonusPercentValue;

    public float AttackSpeedBonusFlat => attackSpeedBonusFlat;
    public bool AttackSpeedBonusIsPercent => attackSpeedBonusPercent;
    public float AttackSpeedBonusPercentValue => attackSpeedBonusPercentValue;

    public float AttackAreaBonusFlat => attackAreaBonusFlat;
    public bool AttackAreaBonusIsPercent => attackAreaBonusPercent;
    public float AttackAreaBonusPercentValue => attackAreaBonusPercentValue;

    public float DoTBonusFlat => dotBonusFlat;
    public bool DoTBonusIsPercent => dotBonusPercent;
    public float DoTBonusPercentValue => dotBonusPercentValue;

    public float DoTDurationBonusFlat => dotDurationBonusFlat;
    public bool DoTDurationBonusIsPercent => dotDurationBonusPercent;
    public float DoTDurationBonusPercentValue => dotDurationBonusPercentValue;

    public float HealthBonusFlat => healthBonusFlat;
    public bool HealthBonusIsPercent => healthBonusPercent;
    public float HealthBonusPercentValue => healthBonusPercentValue;

    public float HealBonusFlat => healBonusFlat;
    public bool HealBonusIsPercent => healBonusPercent;
    public float HealBonusPercentValue => healBonusPercentValue;


    public float GetDamageBonus(float baseValue)
    {
        return damageBonusPercent ? baseValue * (damageBonusPercentValue / 100f) : damageBonusFlat;
    }

    public float GetAttackRangeBonus(float baseValue)
    {
        return attackRangeBonusPercent ? baseValue * (attackRangeBonusPercentValue / 100f) : attackRangeBonusFlat;
    }

    public float GetAttackSpeedBonus(float baseValue)
    {
        return attackSpeedBonusPercent ? baseValue * (attackSpeedBonusPercentValue / 100f) : attackSpeedBonusFlat;
    }

    public float GetAttackAreaBonus(float baseValue)
    {
        return attackAreaBonusPercent ? baseValue * (attackAreaBonusPercentValue / 100f) : attackAreaBonusFlat;
    }
    public float GetDoTBonus(float baseValue)
    {
        return DoTBonusIsPercent ? baseValue * (DoTBonusPercentValue / 100f) : DoTBonusFlat;
    }

    public float GetDoTDurationBonus(float baseValue)
    {
        return DoTDurationBonusIsPercent ? baseValue * (DoTDurationBonusPercentValue / 100f) : DoTDurationBonusFlat;
    }

    public float GetHealthBonus(float baseValue)
    {
        return HealthBonusIsPercent ? baseValue * (HealthBonusPercentValue / 100f) : HealthBonusFlat;
    }
    public float GetHealBonus(float baseValue)
    {
        return HealBonusIsPercent ? baseValue * (HealBonusPercentValue / 100f) : HealBonusFlat;
    }
}