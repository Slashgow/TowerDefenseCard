using System;
using UnityEngine;

public class CardPlayerHealth : Card, IDamageable
{
    [SerializeField, Range(0, 300)] private int maxHealth = 20;
    public int MaxHealth => maxHealth;

    private float currentHealth;
    public float CurrentHealth => currentHealth;

    public event Action<float> OnTakeDamage;
    public event Action OnDie;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentHealth = maxHealth;
    }

    public void Die()
    {
        OnDie?.Invoke();
    }

    public void TakeDamage(float damage)
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
