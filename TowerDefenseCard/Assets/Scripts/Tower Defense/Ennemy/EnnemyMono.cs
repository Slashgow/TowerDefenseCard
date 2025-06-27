using System;
using UnityEngine;
using UnityTimer;

public class EnnemyMono : MonoDamageor, IDamageable
{
    [SerializeField, Range(0,300)] private int maxHealth = 20;
    public int MaxHealth => maxHealth;


    private float currentHealth;

    public event Action<float> OnTakeDamage;
    public event Action OnDie;

    public float CurrentHealth => currentHealth;


    protected override void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        currentHealth = maxHealth;
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

    public void Die()
    {
        Timer.Cancel(attackTimer);
        OnDie?.Invoke(); 
        Destroy(gameObject);
    }

}
