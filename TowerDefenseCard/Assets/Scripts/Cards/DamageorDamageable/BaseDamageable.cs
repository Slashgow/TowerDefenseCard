
using System;
using UnityEngine;

public abstract class BaseDamageable : BaseUpgradable, IDamageable, IHealable
{
    [SerializeField, Range(0, 300)] private float maxHealth = 20;
    public float MaxHealth
    {
        get
        {
            float baseMaxHealth = maxHealth;
            float multiplier = GetTotalUpgradeMultiplier(baseMaxHealth, u => 1f + (u.HealthBonusIsPercent ? u.HealthBonusPercentValue / 100f : 0f));
            float flatBonus = GetTotalUpgradeFlatBonus(baseMaxHealth, u => u.HealthBonusFlat);
            return baseMaxHealth * multiplier + flatBonus;
        }
    }

    protected float currentHealth;
    public float CurrentHealth => currentHealth;

    private bool isDead = false;
    public bool IsDead => isDead;

    public static event Action<float, Vector3> OnAnyDamageableTakeDamage;
    public static event Action<float, Vector3> OnAnyHealableHealed;
    public event Action<float> OnTakeDamage;
    public event Action OnDie;
    public event Action<float> OnHeal;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void Die()
    {
        isDead = true;
        OnDie?.Invoke();
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnTakeDamage?.Invoke(currentHealth);
        OnAnyDamageableTakeDamage?.Invoke(damage, this.transform.position);
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public override void ApplyUpgrade(UpgradeData upgrade)
    {
        float maxHealthBeforeUpgrade = MaxHealth;
        base.ApplyUpgrade(upgrade);
        currentHealth = currentHealth + MaxHealth - maxHealthBeforeUpgrade;
    }

    public override bool RemoveUpgrade(string upgradeName)
    {
        float maxHealthWithUpgrade = MaxHealth;
        bool canRemove = base.RemoveUpgrade(upgradeName);
        currentHealth = currentHealth - (maxHealthWithUpgrade - MaxHealth);
        return canRemove;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
        OnHeal?.Invoke(currentHealth);
        OnAnyHealableHealed?.Invoke(amount, this.transform.position);
    }
}
