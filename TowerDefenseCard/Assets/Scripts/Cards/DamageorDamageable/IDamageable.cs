using System;

public interface IDamageable
{
    public int MaxHealth { get; }
    public float CurrentHealth { get; }
    public void TakeDamage(float damage);
    public void Die();

    public event Action<float> OnTakeDamage;
    public event Action OnDie;
}