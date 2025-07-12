using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTimer;

public abstract class CardBaseDamageor : Card, IDamageor, IUpgradable
{
    [SerializeField] private CardDamageorData cardDamageorData;
    public CardDamageorData CardDamageorData => cardDamageorData;

    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected GameObject impactEffectPrefab;

    protected Timer attackTimer;

    private UpgradeData[] appliedUpgrades = new UpgradeData[0];
    private Dictionary<GameObject, Timer> activeDoTTimers = new Dictionary<GameObject, Timer>(); // Track DoT per enemy

    public float AttackRange
    {
        get
        {
            float baseRange = cardDamageorData.AttackRange;
            float multiplier = GetTotalUpgradeMultiplier(baseRange, u => 1f + (u.AttackRangeBonusIsPercent ? u.AttackRangeBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseRange, u => u.AttackRangeBonusFlat);
            return baseRange * multiplier + flatBonus;
        }
    }

    public float AttackSpeed
    {
        get
        {
            float baseSpeed = cardDamageorData.AttackSpeed;
            float multiplier = GetTotalUpgradeMultiplier(baseSpeed, u => 1f + (u.AttackSpeedBonusIsPercent ? u.AttackSpeedBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseSpeed, u => u.AttackSpeedBonusFlat);
            return baseSpeed * multiplier + flatBonus;
        }
    }

    public float AttackArea
    {
        get
        {
            float baseArea = cardDamageorData.AttackArea;
            float multiplier = GetTotalUpgradeMultiplier(baseArea, u => 1f + (u.AttackAreaBonusIsPercent ? u.AttackAreaBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseArea, u => u.AttackAreaBonusFlat);
            return baseArea * multiplier + flatBonus;
        }
    }

    public DamageType DamageType => cardDamageorData.DamageType;

    public float Damage
    {
        get
        {
            float baseDamage = cardDamageorData.Damage;
            float multiplier = GetTotalUpgradeMultiplier(baseDamage, u => 1f + (u.DamageBonusIsPercent ? u.DamageBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseDamage, u => u.DamageBonusFlat);
            return baseDamage * multiplier + flatBonus;
        }
    }

    public float DoT
    {
        get
        {
            float baseDoT = cardDamageorData.DoT; // Assume CardDamageorData has a DoT field
            float multiplier = GetTotalUpgradeMultiplier(baseDoT, u => 1f + (u.DoTBonusIsPercent ? u.DoTBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseDoT, u => u.DoTBonusFlat);
            return baseDoT * multiplier + flatBonus;
        }
    }

    public float DoTDuration
    {
        get
        {
            float baseDuration = cardDamageorData.DoTDuration; // Assume CardDamageorData has a DoTDuration field
            float multiplier = GetTotalUpgradeMultiplier(baseDuration, u => 1f + (u.DoTDurationBonusIsPercent ? u.DoTDurationBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseDuration, u => u.DoTDurationBonusFlat);
            return baseDuration * multiplier + flatBonus;
        }
    }

    protected override void Start()
    {
        base.Start();
        attackTimer = Timer.Register(1f / AttackSpeed, onComplete: () => Attack(), isLooped: true); // TO DO : Only attack during defense phase
    }

    protected abstract void Attack();

    protected void ApplyDoT(GameObject enemy)
    {
        if (DoT > 0 && DoTDuration > 0)
        {
            if (activeDoTTimers.ContainsKey(enemy))
            {
                activeDoTTimers[enemy].Cancel(); // Refresh DoT if already applied
            }
    
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float totalTimeElapsed = 0f;
                Timer doTTimer = Timer.Register(1f, onComplete: () =>
                                 {
                                     totalTimeElapsed += 1f;
                                     damageable.TakeDamage(DoT);
                                     Instantiate(impactEffectPrefab, enemy.transform.position, Quaternion.identity);
                                 }, 
                                 onUpdate: elapsedTime =>
                                 {
                                     //Debug.Log($"dot time {elapsedTime}");
                                     if (totalTimeElapsed >= DoTDuration)
                                     {
                                         activeDoTTimers[enemy].Cancel();
                                         activeDoTTimers.Remove(enemy);
                                     }
                                 }, isLooped: true);
    
                activeDoTTimers[enemy] = doTTimer;
                
            }
        }
    }

    private float GetTotalUpgradeMultiplier(float baseValue, System.Func<UpgradeData, float> getMultiplier)
    {
        return appliedUpgrades.Aggregate(1f, (acc, u) => acc * getMultiplier(u));
    }

    private float GetTotalUpgradeFlatBonus(float baseValue, System.Func<UpgradeData, float> getBonus)
    {
        return appliedUpgrades.Sum(u => getBonus(u));
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (CanApplyUpgrade(upgrade))
        {
            System.Array.Resize(ref appliedUpgrades, appliedUpgrades.Length + 1);
            appliedUpgrades[appliedUpgrades.Length - 1] = upgrade;
            // Update attack timer with new AttackSpeed
            if (attackTimer != null) 
                attackTimer.Cancel();

            attackTimer = Timer.Register(1f / AttackSpeed, onComplete: () => Attack(), isLooped: true);
            Debug.Log($"Applied upgrade {upgrade.UpgradeName} to {gameObject.name}");
        }
    }

    public bool CanApplyUpgrade(UpgradeData upgrade)
    {
        // Prevent duplicate upgrades (simplistic check; enhance as needed)
        return !System.Array.Exists(appliedUpgrades, u => u.UpgradeName == upgrade.UpgradeName);
    }

    public bool RemoveUpgrade(string upgradeName)
    {
        int index = System.Array.FindIndex(appliedUpgrades, u => u.UpgradeName == upgradeName);
        if (index >= 0)
        {
            UpgradeData[] newUpgrades = new UpgradeData[appliedUpgrades.Length - 1];
            System.Array.Copy(appliedUpgrades, 0, newUpgrades, 0, index);
            System.Array.Copy(appliedUpgrades, index + 1, newUpgrades, index, appliedUpgrades.Length - index - 1);
            appliedUpgrades = newUpgrades;
            // Update attack timer with new AttackSpeed
            if (attackTimer != null) 
                attackTimer.Cancel();

            attackTimer = Timer.Register(1f / AttackSpeed, onComplete: () => Attack(), isLooped: true);
            Debug.Log($"Removed upgrade {upgradeName} from {gameObject.name}");
            return true;
        }
        return false;
    }

    public UpgradeData[] GetAppliedUpgrades()
    {
        return appliedUpgrades;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
