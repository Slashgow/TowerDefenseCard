using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public abstract class BaseHealer : BaseUpgradable, IHealer
{
    [Header("Healer Settings")]
    [SerializeField] private Logger logger;
    [SerializeField] private CardHealerData cardHealerData;
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] protected LayerMask healableLayer = -1;

    protected Timer cooldownTimer;
    protected bool canHealNow = true;

    public float HealAmount
    {
        get
        {
            float baseMaxHealth = cardHealerData.HealAmount;
            float multiplier = GetTotalUpgradeMultiplier(baseMaxHealth, u => 1f + (u.HealBonusIsPercent ? u.HealBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseMaxHealth, u => u.HealBonusFlat);
            return baseMaxHealth * multiplier + flatBonus;
        }
    }
    public float HealRange
    {
        get
        {
            float baseMaxHealth = cardHealerData.HealRange;
            float multiplier = GetTotalUpgradeMultiplier(baseMaxHealth, u => 1f + (u.AttackRangeBonusIsPercent ? u.AttackRangeBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseMaxHealth, u => u.AttackRangeBonusFlat);
            return baseMaxHealth * multiplier + flatBonus;
        }
    }

    public float HealCooldown => cardHealerData.HealCooldown;
    public bool IsCooldownActive => cooldownTimer != null && !cooldownTimer.isCompleted;

    public static event Action<float> OnAnyHealPerformed;

    protected virtual void Start()
    {
        canHealNow = true;
    }

    public abstract bool CanHeal();

    public virtual void PerformHeal(IHealable target)
    {
        if (target != null && target.IsAlive && CanHeal())
        {
            target.Heal(HealAmount);
            StartCooldown();
            OnHealPerformed(target);

            if (healEffectPrefab != null && target is MonoBehaviour targetMono)
                Instantiate(healEffectPrefab, targetMono.transform.position, Quaternion.identity);
        }
    }

    protected virtual void StartCooldown()
    {
        canHealNow = false;
        cooldownTimer = Timer.Register(HealCooldown, onComplete: () => canHealNow = true);
    }

    protected bool IsCooldownReady() => canHealNow;

    protected virtual void OnHealPerformed(IHealable target)
    {
        OnAnyHealPerformed?.Invoke(HealAmount);
        logger.Log($"{gameObject.name} healed {((MonoBehaviour)target).gameObject.name} for {HealAmount} HP", this);
    }

    protected IHealable[] FindHealableTargetsInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, HealRange, healableLayer);
        var healables = new List<IHealable>();

        foreach (var collider in colliders)
        {
            var healable = collider.GetComponent<IHealable>();
            if (healable != null && healable.IsAlive)
            {
                healables.Add(healable);
            }
        }

        return healables.ToArray();
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, HealRange);
    }
}
