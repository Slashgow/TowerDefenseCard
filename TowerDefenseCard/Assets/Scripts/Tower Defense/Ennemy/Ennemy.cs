using UnityEngine;

public class  Ennemy : Card, IDamageable
{
    [SerializeField, Range(0,300)] private int maxHealth = 20;

    private float currentHealth;
    public float CurrentHealth => currentHealth;

    protected override void OnEnable()
    {
        base.OnEnable();

        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
