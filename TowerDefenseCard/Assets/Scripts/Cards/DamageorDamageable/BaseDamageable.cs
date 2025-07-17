
using System;
using UnityEngine;

public abstract class BaseDamageable : BaseUpgradable, IDamageable
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

    private float currentHealth;
    public float CurrentHealth => currentHealth;
   
    public event Action<float> OnTakeDamage;
    public event Action OnDie;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void Die()
    {
        OnDie?.Invoke();
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnTakeDamage?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
