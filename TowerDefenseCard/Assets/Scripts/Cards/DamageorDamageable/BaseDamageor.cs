using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public abstract class BaseDamageor : BaseUpgradable, IDamageor
{
    [SerializeField] private CardDamageorData cardDamageorData;
    public CardDamageorData CardDamageorData => cardDamageorData;

    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected GameObject impactEffectPrefab;

    protected Timer attackTimer;
    public event Action<BaseDamageable> OnAttackEvent;
    protected void OnAttack(BaseDamageable damageable) => OnAttackEvent?.Invoke(damageable);
  
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

    private bool canAttack = true;
    public bool CanAttack => canAttack;
    public void StartAttack() => canAttack = true;
    public void StopAttack() => canAttack = false;

    protected void OnEnable()
    {
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

                                     if(damageable.IsDead || !canAttack)
                                         return;

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

    public override void ApplyUpgrade(UpgradeData upgrade)
    {
        base.ApplyUpgrade(upgrade);

        if (attackTimer != null)
            attackTimer.Cancel();

        attackTimer = Timer.Register(1f / AttackSpeed, onComplete: () => Attack(), isLooped: true);
    }

    public override bool RemoveUpgrade(string upgradeName)
    {
        if (attackTimer != null)
            attackTimer.Cancel();

        attackTimer = Timer.Register(1f / AttackSpeed, onComplete: () => Attack(), isLooped: true);

        return base.RemoveUpgrade(upgradeName);
    }

    private void OnDrawGizmos()
    {
        if (cardDamageorData == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    protected virtual void OnDisable()
    {
        Timer.Cancel(attackTimer);
        foreach (var timer in activeDoTTimers.Values)
        {
            Timer.Cancel(timer);
        }
    }
}
